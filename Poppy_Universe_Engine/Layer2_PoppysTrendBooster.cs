using System;
using System.Collections.Generic;
using System.Linq;

namespace Poppy_Universe_Engine
{
    public class Layer_2_PoppysTrendBooster
    {
        private const double INTERACTION_WEIGHT = 0.6;
        private const double TRENDING_WEIGHT = 0.4;
        private const double MAX_BOOST_RATIO = 0.25; // 25% of the maximum possible score

        private double ComputeInteractionScore(Interaction_Object entry, double maxTotalInteractions)
        {
            if (maxTotalInteractions == 0) return 0;
            return entry.total_interactions / maxTotalInteractions;
        }

        public List<T> BoostScores<T>(
            List<T> layer1Objects,
            List<Interaction_Object> interactions,
            Func<T, int> getObjectId,
            Func<T, double> getLayer1Score,
            Func<T, double> getLayer1MatchPct,
            Action<T, double> setMatchPct,
            Action<T, double> setFinalScore,
            Func<T, double> getFinalScore,
            int topN = 10
        )
        {
            if (layer1Objects == null || layer1Objects.Count == 0)
                return new List<T>();

            if (interactions == null || interactions.Count == 0)
            {
                // No interactions at all - clear BoostDescription and return empty list
                foreach (var obj in layer1Objects)
                {
                    (obj as dynamic).BoostDescription = "No boost";
                }
                return new List<T>();
            }

            double maxPossibleScore = 0;
            foreach (var obj in layer1Objects)
            {
                double score = getLayer1Score(obj);
                double matchPct = getLayer1MatchPct(obj);

                if (matchPct > 0 && score > 0)
                {
                    double calculatedMax = score / (matchPct / 100.0);
                    if (calculatedMax > maxPossibleScore)
                    {
                        maxPossibleScore = calculatedMax;
                    }
                }
            }

            if (maxPossibleScore <= 0) maxPossibleScore = 100;

            double maxBoostAllowed = maxPossibleScore * MAX_BOOST_RATIO;
            double maxInteractions = interactions.Max(i => i.total_interactions);
            var interactionLookup = interactions.ToDictionary(i => i.Object_ID, i => i);

            foreach (var obj in layer1Objects)
            {
                int id = getObjectId(obj);
                double baseScore = getLayer1Score(obj);

                if (!interactionLookup.TryGetValue(id, out var trend))
                {
                    setFinalScore(obj, 0);
                    setMatchPct(obj, 0);
                    (obj as dynamic).BoostDescription = "No boost";
                    continue;
                }

                double interactionScore = ComputeInteractionScore(trend, maxInteractions);
                double trendingNormalized = Math.Max(0, Math.Min(100, trend.trending_score)) / 100.0;
                double boostFactor = (interactionScore * INTERACTION_WEIGHT) + (trendingNormalized * TRENDING_WEIGHT);
                double boostValue = maxBoostAllowed * boostFactor;

                double boostedScore = baseScore + boostValue;
                double maxAllowedForThisObject = baseScore + maxBoostAllowed;

                if (boostedScore > maxAllowedForThisObject)
                    boostedScore = maxAllowedForThisObject;

                if (boostedScore > maxPossibleScore)
                    boostedScore = maxPossibleScore;

                boostedScore = Math.Round(boostedScore, 2);
                setFinalScore(obj, boostedScore);

                double newMatchPercentage = Math.Round((boostedScore / maxPossibleScore) * 100.0, 2);
                setMatchPct(obj, newMatchPercentage);

                // Set BoostDescription
                int boostPercent = (int)Math.Round((boostedScore - baseScore) / maxPossibleScore * 100);
                (obj as dynamic).BoostDescription = boostPercent > 0
                    ? $"Boosted by {boostPercent}"
                    : "No boost";
            }

            return layer1Objects
                .Where(obj => getFinalScore(obj) > 0)
                .OrderByDescending(obj => getFinalScore(obj))
                .ThenByDescending(obj => getLayer1Score(obj))
                .Take(topN)
                .ToList();
        }

        internal void BoostAll(
            List<Star_View> stars,
            List<Planet_View> planets,
            List<Moon_View> moons,
            List<Interaction_Object> interactions,
            int topPerType = 5
        )
        {
            // If 0 or negative, use default of 5
            if (topPerType <= 0) topPerType = 5;

            stars ??= new List<Star_View>();
            planets ??= new List<Planet_View>();
            moons ??= new List<Moon_View>();
            interactions ??= new List<Interaction_Object>();

            var boostedStars = BoostScores(
                stars,
                interactions.Where(i => i.Object_Type == "Star").ToList(),
                s => s.Star.Id,
                s => s.Score,
                s => s.MatchPercentage,
                (s, pct) => s.MatchPercentage = pct,
                (s, score) => s.Score = score,
                s => s.Score,
                topPerType
            );

            var boostedPlanets = BoostScores(
                planets,
                interactions.Where(i => i.Object_Type == "Planet").ToList(),
                p => p.Planet.Id,
                p => p.Score,
                p => p.MatchPercentage,
                (p, pct) => p.MatchPercentage = pct,
                (p, score) => p.Score = score,
                p => p.Score,
                topPerType
            );

            var boostedMoons = BoostScores(
                moons,
                interactions.Where(i => i.Object_Type == "Moon").ToList(),
                m => m.Moon.Id,
                m => m.Score,
                m => m.MatchPercentage,
                (m, pct) => m.MatchPercentage = pct,
                (m, score) => m.Score = score,
                m => m.Score,
                topPerType
            );

            stars.Clear(); stars.AddRange(boostedStars);
            planets.Clear(); planets.AddRange(boostedPlanets);
            moons.Clear(); moons.AddRange(boostedMoons);
        }

        internal List<object> GetCombinedTopResults(
            List<Star_View> stars,
            List<Planet_View> planets,
            List<Moon_View> moons,
            int topN = 15
        )
        {
            var combined = new List<(object obj, double score)>();

            if (stars != null)
                combined.AddRange(stars.Select(s => ((object)s, s.Score)));

            if (planets != null)
                combined.AddRange(planets.Select(p => ((object)p, p.Score)));

            if (moons != null)
                combined.AddRange(moons.Select(m => ((object)m, m.Score)));

            return combined
                .OrderByDescending(x => x.score)
                .Take(topN)
                .Select(x => x.obj)
                .ToList();
        }
    }
}