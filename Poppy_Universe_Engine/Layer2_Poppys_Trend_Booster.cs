using System;
using System.Collections.Generic;
using System.Linq;

namespace Poppy_Universe_Engine
{
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
        private const double MAX_BOOST_RATIO = 0.35;
        private const double HALO_BOOST_MULTIPLIER = 0.45;

        // ✨ Static random to ensure different seeds across calls
        private static readonly Random _random = new Random();

        private double ComputeInteractionScore(Layer2_Interaction_Object entry, double maxTotalInteractions)
        {
            if (maxTotalInteractions == 0) return 0;
            return (double)entry.total_interactions / maxTotalInteractions;
        }

        public List<T> BoostScores<T>(
            List<T> layer1Objects,
            List<Layer2_Interaction_Object> interactions,
            Func<T, string> getObjectId,
            Func<T, string> getCategory,
            Func<T, double> getLayer1Score,
            Func<T, double> getLayer1MatchPct,
            Action<T, double> setMatchPct,
            Action<T, double> setFinalScore,
            Func<T, double> getFinalScore,
            int topN = 10
        )
        {
            if (layer1Objects == null || layer1Objects.Count == 0) return new List<T>();

            List<T> layer2Objects = layer1Objects.ToList();
            if (interactions == null || interactions.Count == 0)
                return layer2Objects.OrderByDescending(o => getLayer1Score(o)).Take(topN).ToList();

            // 1. Calculate theoretical max for normalization
            double maxPossibleScore = 0;
            foreach (var obj in layer2Objects)
            {
                double score = getLayer1Score(obj);
                double matchPct = getLayer1MatchPct(obj);
                if (matchPct > 0)
                {
                    double calcMax = score / (matchPct / 100.0);
                    if (calcMax > maxPossibleScore) maxPossibleScore = calcMax;
                }
            }
            if (maxPossibleScore <= 0) maxPossibleScore = 100;

            double maxBoostAllowed = maxPossibleScore * MAX_BOOST_RATIO;
            double maxInteractions = interactions.Max(i => i.total_interactions);

            // 2. Build Lookups
            var idLookup = interactions.ToDictionary(i => i.Object_ID.ToString(), i => i);

            // 🌌 Pre-calculate Category Halo Boosts
            var categoryAverages = interactions
                .GroupBy(i => i.Object_Type)
                .ToDictionary(
                    g => g.Key,
                    g => g.Average(i => (ComputeInteractionScore(i, maxInteractions) * INTERACTION_WEIGHT) +
                                       ((Math.Max(0, Math.Min(100, i.trending_score)) / 100.0) * TRENDING_WEIGHT))
                );

            // 3. Apply Logic
            foreach (var obj in layer2Objects)
            {
                string id = getObjectId(obj);
                string cat = getCategory(obj);

                double originalMatchPct = getLayer1MatchPct(obj);
                double baseScore = getLayer1Score(obj);
                double boostFactor = 0;

                if (idLookup.TryGetValue(id, out var trend))
                {
                    // 🎯 Direct Match: Full Boost
                    boostFactor = (ComputeInteractionScore(trend, maxInteractions) * INTERACTION_WEIGHT) +
                                  ((Math.Max(0, Math.Min(100, trend.trending_score)) / 100.0) * TRENDING_WEIGHT);
                }
                else if (!string.IsNullOrEmpty(cat) && categoryAverages.TryGetValue(cat, out var haloFactor))
                {
                    // 💡 Halo Match: Category average + tiny variance based on L1 rank
                    double variance = (baseScore / maxPossibleScore) * 0.05;

                    // 🎲 ADDING THE RANDOMNESS HERE
                    // Generates a subtle "Interest Nudge" between 0.9 and 1.1
                    double randomJitter = 0.9 + (_random.NextDouble() * 0.2);

                    boostFactor = ((haloFactor + variance) * HALO_BOOST_MULTIPLIER) * randomJitter;
                }

                double boostValue = maxBoostAllowed * boostFactor;
                double boostedScore = Math.Round(Math.Min(maxPossibleScore, baseScore + boostValue), 2);

                setFinalScore(obj, boostedScore);

                double newMatchPercentage = Math.Round((boostedScore / maxPossibleScore) * 100.0, 2);
                if (newMatchPercentage > 100.00) newMatchPercentage = 100.00;

                setMatchPct(obj, newMatchPercentage);

                double boostDiff = (newMatchPercentage - originalMatchPct);
                string finalBoostVal = boostDiff > 0.01 ? Math.Round(boostDiff, 2).ToString() : "0";

                if (obj is Star_View s) s.BoostDescription = finalBoostVal;
                else if (obj is Planet_View p) p.BoostDescription = finalBoostVal;
                else if (obj is Moon_View m) m.BoostDescription = finalBoostVal;
            }

            return layer2Objects
                .OrderByDescending(obj => getFinalScore(obj))
                .Take(topN)
                .ToList();
        }

        public Layer2_Boost_Result BoostAll(
            List<Star_View> stars,
            List<Planet_View> planets,
            List<Moon_View> moons,
            List<Layer2_Interaction_Object> interactions,
            int topPerType = 5
        )
        {
            if (topPerType <= 0) topPerType = 5;

            var boostedStars = BoostScores(
                stars,
                interactions.Where(i => categoryMatch(i, "Star")).ToList(),
                s => s.Star.Id.ToString(),
                s => "Star",
                s => s.Score,
                s => s.MatchPercentage,
                (s, pct) => s.MatchPercentage = pct,
                (s, score) => s.Score = score,
                s => s.Score,
                topPerType
            );

            var boostedPlanets = BoostScores(
                planets,
                interactions.Where(i => categoryMatch(i, "Planet")).ToList(),
                p => p.Planet.Id.ToString(),
                p => p.Planet.Type,
                p => p.Score,
                p => p.MatchPercentage,
                (p, pct) => p.MatchPercentage = pct,
                (p, score) => p.Score = score,
                p => p.Score,
                topPerType
            );

            var boostedMoons = BoostScores(
                moons,
                interactions.Where(i => categoryMatch(i, "Moon")).ToList(),
                m => m.Moon.Id.ToString(),
                m => "Moon",
                m => m.Score,
                m => m.MatchPercentage,
                (m, pct) => m.MatchPercentage = pct,
                (m, score) => m.Score = score,
                m => m.Score,
                topPerType
            );

            return new Layer2_Boost_Result
            {
                RecommendedStars = boostedStars,
                RecommendedPlanets = boostedPlanets,
                RecommendedMoons = boostedMoons
            };
        }

        private bool categoryMatch(Layer2_Interaction_Object i, string type)
        {
            return i.Object_Type.IndexOf(type, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}