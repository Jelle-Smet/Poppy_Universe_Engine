using System;
using System.Collections.Generic;
using System.Linq;

namespace Poppy_Universe_Engine
{
    // New class to hold the results from the boost operation
    public class Layer2_Boost_Result
    {
        public List<Star_View> RecommendedStars { get; set; }
        public List<Planet_View> RecommendedPlanets { get; set; }
        public List<Moon_View> RecommendedMoons { get; set; }
    }

    public class Layer_2_Poppys_Trend_Booster
    {
        private const double INTERACTION_WEIGHT = 0.6;
        private const double TRENDING_WEIGHT = 0.4;
        private const double MAX_BOOST_RATIO = 0.25; // 25% of the maximum possible score

        private double ComputeInteractionScore(Layer2_Interaction_Object entry, double maxTotalInteractions)
        {
            if (maxTotalInteractions == 0) return 0;
            return entry.total_interactions / maxTotalInteractions;
        }

        public List<T> BoostScores<T>(
            List<T> layer1Objects,
            List<Layer2_Interaction_Object> interactions,
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

            // **CRITICAL CHANGE: To get a new list, we must clone the layer1Objects**
            // The original objects themselves are still modified (in-place) as a side effect
            // of using Action<T, double> delegates (setMatchPct, setFinalScore).
            // Since Layer 2 is meant to build on Layer 1, modifying the object's score/match is often desired.
            // If you need the Layer 1 list to remain ABSOLUTELY unchanged (without the L2 score/match), 
            // you'd need to deep copy the objects *before* boosting. 
            // Assuming you want the L2 scores calculated on a NEW LIST for sorting/filtering:
            List<T> layer2Objects = layer1Objects.ToList();

            if (interactions == null || interactions.Count == 0)
            {
                // No interactions at all - clear BoostDescription and return empty list
                foreach (var obj in layer2Objects)
                {
                    (obj as dynamic).BoostDescription = "No boost";
                }
                // Return an empty list of recommendations if no interactions are available, 
                // as the list of objects is now effectively filtered by interaction availability.
                return new List<T>();
            }

            double maxPossibleScore = 0;
            foreach (var obj in layer2Objects)
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

            foreach (var obj in layer2Objects)
            {
                int id = getObjectId(obj);
                double baseScore = getLayer1Score(obj);

                if (!interactionLookup.TryGetValue(id, out var trend))
                {
                    // If no trend interaction, set score/match to 0 for filtering purposes
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

            return layer2Objects
                .Where(obj => getFinalScore(obj) > 0)
                .OrderByDescending(obj => getFinalScore(obj))
                .ThenByDescending(obj => getLayer1Score(obj))
                .Take(topN)
                .ToList();
        }

        // Changed from internal void to public Layer2_Boost_Result
        public Layer2_Boost_Result BoostAll(
            List<Star_View> stars,
            List<Planet_View> planets,
            List<Moon_View> moons,
            List<Layer2_Interaction_Object> interactions,
            int topPerType = 5
        )
        {
            // If 0 or negative, use default of 5
            if (topPerType <= 0) topPerType = 5;

            stars ??= new List<Star_View>();
            planets ??= new List<Planet_View>();
            moons ??= new List<Moon_View>();
            interactions ??= new List<Layer2_Interaction_Object>();

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

            // Removed the lines that clear and re-add to the original lists.
            // stars.Clear(); stars.AddRange(boostedStars);
            // planets.Clear(); planets.AddRange(boostedPlanets);
            // moons.Clear(); moons.AddRange(boostedMoons);

            // Return a new object containing the boosted lists
            return new Layer2_Boost_Result
            {
                RecommendedStars = boostedStars,
                RecommendedPlanets = boostedPlanets,
                RecommendedMoons = boostedMoons
            };
        }

        // GetCombinedTopResults remains the same, but it's not strictly necessary 
        // to return the results to your calling code, so I'll keep it as-is.
        public List<object> GetCombinedTopResults(
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