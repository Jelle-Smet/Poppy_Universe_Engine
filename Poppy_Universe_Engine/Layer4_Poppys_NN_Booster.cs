using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poppy_Universe_Engine
{
    /// <summary>
    /// Holds the results after applying the Layer 4 (Neural Network Prediction) boost operation.
    /// </summary>
    public class Layer4_Boost_Result
    {
        public List<Star_View> RecommendedStars { get; set; }
        public List<Planet_View> RecommendedPlanets { get; set; }
        public List<Moon_View> RecommendedMoons { get; set; }
    }

    // Internal class (if needed for structure, though the public one below is used)
    internal class Layer4_Poppys_NN_Booster
    {
        // Placeholder or unused internal declaration.
    }

    /// <summary>
    /// Applies a highly influential boost to object scores based on learned user preferences 
    /// derived from a Neural Network (NN) model.
    /// </summary>
    public class Layer_4_Poppys_NN_Booster
    {
        // Constants reflecting the higher confidence placed in NN predictions
        private const double PREF_WEIGHT = 0.7;	    // Higher personalization influence (was 0.6 in L3)
        private const double BASE_WEIGHT = 0.3;	    // Lower base relevance (was 0.4 in L3)
        private const double MAX_BOOST_RATIO = 0.75; // Higher cap at 75% (was 0.5 in L3)

        /// <summary>
        /// Normalizes an input value (v) against a maximum of 10 to produce a factor (0.0-1.0).
        /// </summary>
        private double Normalize(double v) => Math.Max(0, Math.Min(10, v)) / 10.0;

        /// <summary>
        /// Calculates a star's preference score (0.0-1.0) based on its spectral type 
        /// and the NN's learned preferences.
        /// </summary>
        private double ComputeStarScore(Star_View s, Layer4_User_NN_Object prefs)
        {
            string type = s.Star.SpectralType.ToUpper();
            // Match the star's spectral type to the NN-derived preference value
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

        /// <summary>
        /// Calculates a planet's preference score (0.0-1.0) based on its category 
        /// and the NN's learned preferences.
        /// </summary>
        private double ComputePlanetScore(Planet_View p, Layer4_User_NN_Object prefs)
        {
            string cat = p.Planet.Type;
            // Match the planet's type to the NN-derived preference value
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

        /// <summary>
        /// Calculates a moon's preference score (0.0-1.0) based on its parent planet 
        /// and the NN's learned preferences.
        /// </summary>
        private double ComputeMoonScore(Moon_View m, Layer4_User_NN_Object prefs)
        {
            string parent = m.Parent;
            // Match the moon's parent planet to the NN-derived preference value
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

        /// <summary>
        /// Generic method to apply the Neural Network Prediction boost.
        /// </summary>
        /// <typeparam name="T">The type of the astronomical object (Star_View, Planet_View, etc.).</typeparam>
        /// <param name="layerObjects">Input list of objects (results from L3 or previous layers).</param>
        /// <param name="prefs">The user's NN preference predictions.</param>
        /// <param name="getPreviousScore">Delegate to get the score from the previous layer.</param>
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
                    // Reverse-calculate the max possible score
                    double calculatedMax = score / (matchPct / 100.0);
                    if (calculatedMax > maxPossibleScore)
                    {
                        maxPossibleScore = calculatedMax;
                    }
                }
            }

            if (maxPossibleScore <= 0) maxPossibleScore = 100;

            // Define the maximum possible boost value
            double maxBoostAllowed = maxPossibleScore * MAX_BOOST_RATIO;

            foreach (var obj in currentLayerObjects)
            {
                double baseScore = getPreviousScore(obj);
                double prefScore = getPreferenceScore(obj); // Normalized preference score (0.0-1.0)

                // Apply non-linear boost to emphasize strong preferences predicted by the NN
                double normalizedPref = Math.Pow(prefScore, 1.2); // Slightly exponential boost
                double boostValue = maxBoostAllowed * normalizedPref;

                // Weighted combination favoring NN predictions more than previous layers
                // The original logic `(baseScore * BASE_WEIGHT) + (baseScore * PREF_WEIGHT)` simplifies to `baseScore`
                // as BASE_WEIGHT (0.3) + PREF_WEIGHT (0.7) = 1.0.
                // The boostedScore calculation is effectively: boostedScore = baseScore + boostValue
                double boostedScore = (baseScore * BASE_WEIGHT) + (baseScore * PREF_WEIGHT) + boostValue;

                // Cap 1: Ensure the score doesn't exceed the max allowed ratio
                double maxAllowedForThisObject = baseScore + maxBoostAllowed;
                if (boostedScore > maxAllowedForThisObject)
                    boostedScore = maxAllowedForThisObject;

                // Cap 2: Global cap against the theoretical maximum
                if (boostedScore > maxPossibleScore)
                    boostedScore = maxPossibleScore;

                // Round the final score
                boostedScore = Math.Round(boostedScore, 2);
                setFinalScore(obj, boostedScore);

                // Calculate and set the new match percentage
                double newMatchPercentage = Math.Round((boostedScore / maxPossibleScore) * 100.0, 2);

                // Explicitly cap the displayed percentage
                if (newMatchPercentage > 100.00)
                    newMatchPercentage = 100.00;

                setMatchPct(obj, newMatchPercentage);

                // Set BoostDescription with NN indicator
                int boostPercent = (int)Math.Round((boostedScore - baseScore) / maxPossibleScore * 100);
                (obj as dynamic).BoostDescription = boostPercent > 0
                    ? $"NN Boosted by {boostPercent}%"
                    : "No NN boost";
            }

            // Filter, sort by boosted score, then by previous score (tie-breaker), and take top N
            return currentLayerObjects
                .Where(obj => getFinalScore(obj) > 0)
                .OrderByDescending(obj => getFinalScore(obj))
                .ThenByDescending(obj => getPreviousScore(obj))
                .Take(topN)
                .ToList();
        }

        /// <summary>
        /// Coordinates the Layer 4 Neural Network boosting process for all object types.
        /// </summary>
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

            // The 'getPreviousScore' delegates retrieve the score from the previous layer (L3 or L2).

            // --- Boost Stars ---
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

            // --- Boost Planets ---
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

            // --- Boost Moons ---
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

        /// <summary>
        /// Combines the top recommended objects from all three categories into a single, 
        /// globally ranked list based on the final boosted score, favoring diversity in the final rank.
        /// </summary>
        /// <returns>A list of the top N objects, regardless of type, sorted by final score.</returns>
        public List<object> GetCombinedTopResults(
            List<Star_View> stars,
            List<Planet_View> planets,
            List<Moon_View> moons,
            int topN = 15
        )
        {
            // Tuple: (object, score, type string)
            var combined = new List<(object obj, double score, string type)>();

            if (stars != null)
                combined.AddRange(stars.Select(s => ((object)s, s.Score, "star")));

            if (planets != null)
                combined.AddRange(planets.Select(p => ((object)p, p.Score, "planet")));

            if (moons != null)
                combined.AddRange(moons.Select(m => ((object)m, m.Score, "moon")));

            // Enhanced sorting: primary by score, secondary by type (for consistent tie-breaking/diversity)
            return combined
                .OrderByDescending(x => x.score)
                .ThenBy(x => x.type) // Used as a simple tie-breaker / diversity mechanism
                .Take(topN)
                .Select(x => x.obj)
                .ToList();
        }

        /// <summary>
        /// Calculates the overall confidence/distinction of the NN's predictions 
        /// by analyzing the standard deviation of its preference matrix values.
        /// </summary>
        /// <returns>A normalized confidence value (0.0 to 1.0).</returns>
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
            // Variance and Standard Deviation (measures how spread out the predictions are)
            double variance = allPrefs.Select(p => Math.Pow(p - avgStrength, 2)).Average();
            double stdDev = Math.Sqrt(variance);

            // Higher standard deviation means the NN has more distinct (less flat) preferences, 
            // implying higher confidence in its specific predictions.
            // Normalized to a 0-1 range (assuming max stdDev is around 2.0 based on 0-10 input)
            return Math.Min(1.0, stdDev / 2.0);
        }
    }
}