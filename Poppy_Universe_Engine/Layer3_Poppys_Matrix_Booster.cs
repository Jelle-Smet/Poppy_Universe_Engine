using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poppy_Universe_Engine
{
    /// <summary>
    /// Holds the results after applying the Layer 3 (Matrix Preference) boost operation.
    /// </summary>
    public class Layer3_Boost_Result
    {
        public List<Star_View> RecommendedStars { get; set; }
        public List<Planet_View> RecommendedPlanets { get; set; }
        public List<Moon_View> RecommendedMoons { get; set; }
    }

    /// <summary>
    /// Applies a detailed preference boost to astronomical object scores using a user-specific 
    /// preference matrix (Layer 3).
    /// </summary>
    public class Layer_3_Poppys_Matrix_Booster
    {
        // Constants defining how the Layer 3 preference score influences the final score
        private const double PREF_WEIGHT = 0.6; // Weight applied to the preference score boost
        private const double BASE_WEIGHT = 0.4; // Weight applied to the existing base score (L1 or L2)
        // Maximum amount the score can be boosted, relative to the max possible base score
        private const double MAX_BOOST_RATIO = 0.5; // Boost is capped at 50% of the max possible score

        /// <summary>
        /// Normalizes an input value (v) against a maximum of 10.
        /// Used to convert raw matrix preference scores (0-10) into a normalized boost factor (0.0-1.0).
        /// </summary>
        private double Normalize(double v) => Math.Max(0, Math.Min(10, v)) / 10.0;

        /// <summary>
        /// Calculates a star's preference score (0.0-1.0) based on its spectral type 
        /// and the user's matrix preferences. 
        /// </summary>
        private double ComputeStarScore(Star_View s, Layer3_User_Matrix_Object prefs)
        {
            string type = s.Star.SpectralType.ToUpper();
            // Match the star's spectral type to the corresponding preference value in the matrix
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
        /// (Terrestrial, Giant, Dwarf) and the user's matrix preferences.
        /// </summary>
        private double ComputePlanetScore(Planet_View p, Layer3_User_Matrix_Object prefs)
        {
            string cat = p.Planet.Type;
            // Match the planet's type to the corresponding preference value
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
        /// and the user's matrix preferences.
        /// </summary>
        private double ComputeMoonScore(Moon_View m, Layer3_User_Matrix_Object prefs)
        {
            string parent = m.Parent;
            // Match the moon's parent planet to the corresponding preference value
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
        /// Generic method to apply the Matrix Preference boost to a list of astronomical objects.
        /// This method takes a base score (from L1 or L2) and adds a boost proportional 
        /// to the user's preference for the object's specific category (type/parent).
        /// </summary>
        /// <typeparam name="T">The type of the astronomical object (Star_View, Planet_View, etc.).</typeparam>
        public List<T> BoostScores<T>(
            List<T> layerObjects, // Input list of objects (L1 or L2 results)
            Layer3_User_Matrix_Object prefs,
            Func<T, double> getLayer1Score,
            Func<T, double> getLayer1MatchPct,
            Func<T, double> getPreferenceScore, // Method call to ComputeStar/Planet/MoonScore
            Action<T, double> setMatchPct,
            Action<T, double> setFinalScore,
            Func<T, double> getFinalScore,
            int topN = 10
        )
        {
            if (layerObjects == null || layerObjects.Count == 0)
                return new List<T>();

            // Create a copy of the list to ensure the input list is not modified during sorting/filtering.
            List<T> currentLayerObjects = layerObjects.ToList();

            // --- 1. Calculate the Theoretical Maximum Possible Score (for normalization) ---
            double maxPossibleScore = 0;
            foreach (var obj in currentLayerObjects)
            {
                double score = getLayer1Score(obj);
                double matchPct = getLayer1MatchPct(obj);

                if (matchPct > 0 && score > 0)
                {
                    // Reverse-calculate the max possible score from the highest scored object
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

            // --- 2. Apply Boost to Each Object ---
            foreach (var obj in currentLayerObjects)
            {
                double baseScore = getLayer1Score(obj);
                double prefScore = getPreferenceScore(obj); // Normalized preference score (0.0-1.0)

                // Calculate the value of the boost based on the max boost allowed and the normalized preference
                double boostValue = maxBoostAllowed * prefScore;

                // Apply the boost: Final Score = Base Score + Boost Value
                double boostedScore = baseScore + boostValue;

                // --- 3. Cap Score ---
                double maxAllowedForThisObject = baseScore + maxBoostAllowed;

                // Cap 1: Ensure the score doesn't exceed the max boost ratio on top of the base score
                if (boostedScore > maxAllowedForThisObject)
                    boostedScore = maxAllowedForThisObject;

                // Cap 2: Ensure the total score doesn't exceed the theoretical maximum possible score
                if (boostedScore > maxPossibleScore)
                    boostedScore = maxPossibleScore;

                // Finalize and set the boosted score and match percentage
                boostedScore = Math.Round(boostedScore, 2);
                setFinalScore(obj, boostedScore);

                double newMatchPercentage = Math.Round((boostedScore / maxPossibleScore) * 100.0, 2);

                // Explicitly cap the displayed percentage
                if (newMatchPercentage > 100.00)
                    newMatchPercentage = 100.00;

                setMatchPct(obj, newMatchPercentage);

                // Set BoostDescription for UI feedback
                int boostPercent = (int)Math.Round((boostedScore - baseScore) / maxPossibleScore * 100);
                (obj as dynamic).BoostDescription = boostPercent > 0
                    ? $"Boosted by {boostPercent}%"
                    : "No boost";
            }

            // --- 4. Filter and Sort Results ---
            return currentLayerObjects
                // Only return objects that have a final score > 0 (i.e., were visible)
                .Where(obj => getFinalScore(obj) > 0)
                // Sort by boosted score (Primary)
                .OrderByDescending(obj => getFinalScore(obj))
                // Sort by original score (Tie-breaker)
                .ThenByDescending(obj => getLayer1Score(obj))
                // Take only the top N results
                .Take(topN)
                .ToList();
        }

        /// <summary>
        /// Coordinates the Layer 3 boosting process for all object types (Stars, Planets, Moons).
        /// </summary>
        /// <param name="stars">Input list of Star_View objects (L1 or L2 results).</param>
        /// <param name="planets">Input list of Planet_View objects (L1 or L2 results).</param>
        /// <param name="moons">Input list of Moon_View objects (L1 or L2 results).</param>
        /// <param name="prefs">The user's preference matrix for boosting categories.</param>
        /// <param name="topPerType">The number of top results to keep for each object type (default 5).</param>
        /// <returns>A Layer3_Boost_Result object containing the three lists of boosted and ranked objects.</returns>
        public Layer3_Boost_Result BoostAll(
            List<Star_View> stars,
            List<Planet_View> planets,
            List<Moon_View> moons,
            Layer3_User_Matrix_Object prefs,
            int topPerType = 5
        )
        {
            if (topPerType <= 0) topPerType = 5;

            stars ??= new List<Star_View>();
            planets ??= new List<Planet_View>();
            moons ??= new List<Moon_View>();

            // The 'getLayer1Score' and 'getLayer1MatchPct' delegates here refer to the scores 
            // set by the PREVIOUS layer (Layer 1 or Layer 2), which are treated as the 'base score' for L3.

            // --- Boost Stars ---
            var boostedStars = BoostScores(
                stars,
                prefs,
                s => s.Score,
                s => s.MatchPercentage,
                s => ComputeStarScore(s, prefs), // Get L3 preference score
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
                p => ComputePlanetScore(p, prefs), // Get L3 preference score
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
                m => ComputeMoonScore(m, prefs), // Get L3 preference score
                (m, pct) => m.MatchPercentage = pct,
                (m, score) => m.Score = score,
                m => m.Score,
                topPerType
            );

            // Return a new object containing the final boosted lists
            return new Layer3_Boost_Result
            {
                RecommendedStars = boostedStars,
                RecommendedPlanets = boostedPlanets,
                RecommendedMoons = boostedMoons
            };
        }

        /// <summary>
        /// Combines the top recommended objects from all three categories into a single, 
        /// globally ranked list based on the final boosted score.
        /// </summary>
        /// <returns>A list of the top N objects, regardless of type, sorted by final score.</returns>
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

            // Combine, sort by score descending, and take the top N
            return combined
                .OrderByDescending(x => x.score)
                .Take(topN)
                .Select(x => x.obj)
                .ToList();
        }
    }
}