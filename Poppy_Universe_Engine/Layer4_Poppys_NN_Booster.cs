using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poppy_Universe_Engine
{
    // New class to hold the results from the boost operation (Layer 4)
    public class Layer4_Boost_Result
    {
        public List<Star_View> RecommendedStars { get; set; }
        public List<Planet_View> RecommendedPlanets { get; set; }
        public List<Moon_View> RecommendedMoons { get; set; }
    }
    internal class Layer4_Poppys_NN_Booster
    {
        
    }
    public class Layer_4_Poppys_NN_Booster
    {
        // Increased weights since NN predictions are more sophisticated
        private const double PREF_WEIGHT = 0.7;  // Higher personalization influence (was 0.6)
        private const double BASE_WEIGHT = 0.3;  // Lower base relevance (was 0.4)
        private const double MAX_BOOST_RATIO = 0.85; // Higher cap at 85% (was 0.75) - NN is more confident

        private double Normalize(double v) => Math.Max(0, Math.Min(10, v)) / 10.0;

        private double ComputeStarScore(Star_View s, Layer4_User_NN_Object prefs)
        {
            string type = s.Star.SpectralType.ToUpper();
            double pref = type switch
            {
                "A" => prefs.A,
                "B" => prefs.B,
                "F" => prefs.F,
                "G" => prefs.G,
                "K" => prefs.K,
                "M" => prefs.M,
                "O" => prefs.O,
                _ => 0
            };
            return Normalize(pref);
        }

        private double ComputePlanetScore(Planet_View p, Layer4_User_NN_Object prefs)
        {
            string cat = p.Planet.Type;
            double pref = cat switch
            {
                "Dwarf Planet" => prefs.DwarfPlanet,
                "Gas Giant" => prefs.GasGiant,
                "Ice Giant" => prefs.IceGiant,
                "Terrestrial" => prefs.Terrestrial,
                _ => 0
            };
            return Normalize(pref);
        }

        private double ComputeMoonScore(Moon_View m, Layer4_User_NN_Object prefs)
        {
            string parent = m.Parent;
            double pref = parent switch
            {
                "Earth" => prefs.Earth,
                "Eris" => prefs.Eris,
                "Haumea" => prefs.Haumea,
                "Jupiter" => prefs.Jupiter,
                "Makemake" => prefs.Makemake,
                "Mars" => prefs.Mars,
                "Neptune" => prefs.Neptune,
                "Pluto" => prefs.Pluto,
                "Saturn" => prefs.Saturn,
                "Uranus" => prefs.Uranus,
                _ => 0
            };
            return Normalize(pref);
        }

        public List<T> BoostScores<T>(
            List<T> layerObjects, // Input from Layer 3 (or Layer 2 if Layer 3 is skipped)
            Layer4_User_NN_Object prefs,
            Func<T, double> getPreviousScore,
            Func<T, double> getPreviousMatchPct,
            Func<T, double> getPreferenceScore,
            Action<T, double> setMatchPct,
            Action<T, double> setFinalScore,
            Func<T, double> getFinalScore,
            int topN = 10
        )
        {
            if (layerObjects == null || layerObjects.Count == 0)
                return new List<T>();

            // Create a copy to avoid modifying input list
            List<T> currentLayerObjects = layerObjects.ToList();

            // Calculate max possible score from previous layer
            double maxPossibleScore = 0;
            foreach (var obj in currentLayerObjects)
            {
                double score = getPreviousScore(obj);
                double matchPct = getPreviousMatchPct(obj);

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

            foreach (var obj in currentLayerObjects)
            {
                double baseScore = getPreviousScore(obj);
                double prefScore = getPreferenceScore(obj);

                // NN-based boost: more aggressive since predictions are learned patterns
                // Apply non-linear boost to emphasize strong preferences
                double normalizedPref = Math.Pow(prefScore, 1.2); // Slightly exponential boost
                double boostValue = maxBoostAllowed * normalizedPref;

                // Weighted combination favoring NN predictions more
                double boostedScore = (baseScore * BASE_WEIGHT) + (baseScore * PREF_WEIGHT) + boostValue;

                // Cap to max allowed for this object
                double maxAllowedForThisObject = baseScore + maxBoostAllowed;
                if (boostedScore > maxAllowedForThisObject)
                    boostedScore = maxAllowedForThisObject;

                if (boostedScore > maxPossibleScore)
                    boostedScore = maxPossibleScore;

                boostedScore = Math.Round(boostedScore, 2);
                setFinalScore(obj, boostedScore);

                double newMatchPercentage = Math.Round((boostedScore / maxPossibleScore) * 100.0, 2);
                setMatchPct(obj, newMatchPercentage);

                // Set BoostDescription with NN indicator
                int boostPercent = (int)Math.Round((boostedScore - baseScore) / maxPossibleScore * 100);
                (obj as dynamic).BoostDescription = boostPercent > 0
                    ? $"NN Boosted by {boostPercent}%"
                    : "No NN boost";
            }

            return currentLayerObjects
                .Where(obj => getFinalScore(obj) > 0)
                .OrderByDescending(obj => getFinalScore(obj))
                .ThenByDescending(obj => getPreviousScore(obj))
                .Take(topN)
                .ToList();
        }

        public Layer4_Boost_Result BoostAll(
            List<Star_View> stars,
            List<Planet_View> planets,
            List<Moon_View> moons,
            Layer4_User_NN_Object prefs,
            int topPerType = 5
        )
        {
            if (topPerType <= 0) topPerType = 5;

            stars ??= new List<Star_View>();
            planets ??= new List<Planet_View>();
            moons ??= new List<Moon_View>();

            var boostedStars = BoostScores(
                stars,
                prefs,
                s => s.Score,
                s => s.MatchPercentage,
                s => ComputeStarScore(s, prefs),
                (s, pct) => s.MatchPercentage = pct,
                (s, score) => s.Score = score,
                s => s.Score,
                topPerType
            );

            var boostedPlanets = BoostScores(
                planets,
                prefs,
                p => p.Score,
                p => p.MatchPercentage,
                p => ComputePlanetScore(p, prefs),
                (p, pct) => p.MatchPercentage = pct,
                (p, score) => p.Score = score,
                p => p.Score,
                topPerType
            );

            var boostedMoons = BoostScores(
                moons,
                prefs,
                m => m.Score,
                m => m.MatchPercentage,
                m => ComputeMoonScore(m, prefs),
                (m, pct) => m.MatchPercentage = pct,
                (m, score) => m.Score = score,
                m => m.Score,
                topPerType
            );

            // Return new result object with boosted lists
            return new Layer4_Boost_Result
            {
                RecommendedStars = boostedStars,
                RecommendedPlanets = boostedPlanets,
                RecommendedMoons = boostedMoons
            };
        }

        public List<object> GetCombinedTopResults(
            List<Star_View> stars,
            List<Planet_View> planets,
            List<Moon_View> moons,
            int topN = 15
        )
        {
            var combined = new List<(object obj, double score, string type)>();

            if (stars != null)
                combined.AddRange(stars.Select(s => ((object)s, s.Score, "star")));

            if (planets != null)
                combined.AddRange(planets.Select(p => ((object)p, p.Score, "planet")));

            if (moons != null)
                combined.AddRange(moons.Select(m => ((object)m, m.Score, "moon")));

            // Enhanced sorting: primary by score, secondary by diversity
            // This ensures we get a good mix of types in top results
            return combined
                .OrderByDescending(x => x.score)
                .ThenBy(x => x.type) // Adds slight diversity preference
                .Take(topN)
                .Select(x => x.obj)
                .ToList();
        }

        // Additional helper method for NN confidence scoring
        public double CalculateNNConfidence(Layer4_User_NN_Object prefs)
        {
            // Calculate average prediction strength across all categories
            var allPrefs = new List<double>
        {
            // Stars
            prefs.A, prefs.B, prefs.F, prefs.G, prefs.K, prefs.M, prefs.O,
            // Planets
            prefs.DwarfPlanet, prefs.GasGiant, prefs.IceGiant, prefs.Terrestrial,
            // Moons
            prefs.Earth, prefs.Eris, prefs.Haumea, prefs.Jupiter,
            prefs.Makemake, prefs.Mars, prefs.Neptune, prefs.Pluto,
            prefs.Saturn, prefs.Uranus
        };

            double avgStrength = allPrefs.Average();
            double variance = allPrefs.Select(p => Math.Pow(p - avgStrength, 2)).Average();
            double stdDev = Math.Sqrt(variance);

            // Higher standard deviation = more distinct preferences = higher confidence
            // Normalized to 0-1 range
            return Math.Min(1.0, stdDev / 2.0);
        }
    }
}
