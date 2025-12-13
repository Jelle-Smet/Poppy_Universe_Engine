using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poppy_Universe_Engine
{
    /// <summary>
    /// Holds the results after applying the trending boost operation,
    /// ensuring a clean separation of the final, ranked object lists.
    /// </summary>
    public class Layer2_Boost_Result
    {
        public List<Star_View> RecommendedStars { get; set; }
        public List<Planet_View> RecommendedPlanets { get; set; }
        public List<Moon_View> RecommendedMoons { get; set; }
    }

    /// <summary>
    /// Applies a trend-based score boost to astronomical objects from Layer 1 
    /// using real-time interaction data from Layer 2.
    /// </summary>
    public class Layer_2_Poppys_Trend_Booster
    {
        // Constants defining how the trend score is calculated
        private const double INTERACTION_WEIGHT = 0.6; // Weight applied to raw interaction volume
        private const double TRENDING_WEIGHT = 0.4;    // Weight applied to the pre-calculated trending score
        // Maximum amount the score can be boosted, relative to the max possible base score
        private const double MAX_BOOST_RATIO = 0.25; // Boost is capped at 25% of the max possible score

        /// <summary>
        /// Normalizes the total interaction count against the highest total interaction count observed.
        /// </summary>
        private double ComputeInteractionScore(Layer2_Interaction_Object entry, double maxTotalInteractions)
        {
            if (maxTotalInteractions == 0) return 0;
            // Returns a normalized score between 0 and 1.0
            return entry.total_interactions / maxTotalInteractions;
        }

        /// <summary>
        /// Generic method to apply trend boosting to a list of astronomical objects.
        /// </summary>
        /// <typeparam name="T">The type of the astronomical object (Star_View, Planet_View, etc.).</typeparam>
        /// <param name="layer1Objects">The input list of objects with calculated Layer 1 (user preference) scores.</param>
        /// <param name="interactions">The list of Layer 2 interaction data.</param>
        /// <param name="getObjectId">Delegate to get the unique ID of the object.</param>
        /// <param name="getLayer1Score">Delegate to get the initial Layer 1 score.</param>
        /// <param name="getLayer1MatchPct">Delegate to get the initial Layer 1 match percentage.</param>
        /// <param name="setMatchPct">Delegate to set the final (boosted) match percentage.</param>
        /// <param name="setFinalScore">Delegate to set the final (boosted) score.</param>
        /// <param name="getFinalScore">Delegate to get the final (boosted) score.</param>
        /// <param name="topN">The maximum number of top results to return.</param>
        /// <returns>A new, ranked list of boosted objects (up to topN).</returns>
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

            // Create a new list to avoid side effects on the input list reference itself,
            // while still allowing the objects *within* the list to be modified in-place 
            // via the delegates (which is the intended behavior for applying the L2 score).
            List<T> layer2Objects = layer1Objects.ToList();

            if (interactions == null || interactions.Count == 0)
            {
                // If no interaction data is available, skip boosting and return an empty list
                // as recommendations rely on interaction filtering.
                foreach (var obj in layer2Objects)
                {
                    (obj as dynamic).BoostDescription = "No boost"; // Cast to dynamic to access BoostDescription
                }
                
                return new List<T>();
            }

            // --- 1. Calculate the Theoretical Maximum Possible Score (for normalization) ---
            double maxPossibleScore = 0;
            foreach (var obj in layer2Objects)
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

            if (maxPossibleScore <= 0) maxPossibleScore = 100; // Default to 100 if unable to calculate

            // --- 2. Pre-calculate Limits and Lookups ---
            double maxBoostAllowed = maxPossibleScore * MAX_BOOST_RATIO;
            double maxInteractions = interactions.Max(i => i.total_interactions);
            // Create a dictionary for O(1) lookup of interaction data by object ID
            var interactionLookup = interactions.ToDictionary(i => i.Object_ID, i => i);

            // --- 3. Apply Boost to Each Object ---
            foreach (var obj in layer2Objects)
            {
                int id = getObjectId(obj);
                double baseScore = getLayer1Score(obj);

                // Skip objects with no corresponding interaction data
                if (!interactionLookup.TryGetValue(id, out var trend))
                {
                    // Objects without trend data are filtered out later, but explicitly setting score to 0
                    // ensures they don't influence the final ranked list.
                    setFinalScore(obj, 0);
                    setMatchPct(obj, 0);
                    (obj as dynamic).BoostDescription = "No boost";
                    continue;
                }

                // Calculate the final boost factor (0.0 to 1.0)
                double interactionScore = ComputeInteractionScore(trend, maxInteractions);
                // Normalize trending_score (assumed to be 0-100) to 0.0-1.0
                double trendingNormalized = Math.Max(0, Math.Min(100, trend.trending_score)) / 100.0;

                // Weighted average of interaction volume and pre-calculated trend score
                double boostFactor = (interactionScore * INTERACTION_WEIGHT) + (trendingNormalized * TRENDING_WEIGHT);

                // Calculate the raw boost value (capped by maxBoostAllowed)
                double boostValue = maxBoostAllowed * boostFactor;

                // --- 4. Apply Boost and Cap Score ---
                double boostedScore = baseScore + boostValue;
                double maxAllowedForThisObject = baseScore + maxBoostAllowed;

                // Cap the boost to ensure it doesn't exceed the max allowed ratio
                if (boostedScore > maxAllowedForThisObject)
                    boostedScore = maxAllowedForThisObject;

                // Cap the total score to the theoretical maximum possible score
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

            // --- 5. Filter and Sort Results ---
            return layer2Objects
                // Only return objects that have a final score > 0 (i.e., were visible and had trend data)
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
        /// Coordinates the boosting process for all object types (Stars, Planets, Moons).
        /// </summary>
        /// <param name="stars">Initial list of Star_View objects.</param>
        /// <param name="planets">Initial list of Planet_View objects.</param>
        /// <param name="moons">Initial list of Moon_View objects.</param>
        /// <param name="interactions">Full list of interaction data for all object types.</param>
        /// <param name="topPerType">The number of top results to keep for each object type (default 5).</param>
        /// <returns>A Layer2_Boost_Result object containing the three lists of boosted and ranked objects.</returns>
        public Layer2_Boost_Result BoostAll(
            List<Star_View> stars,
            List<Planet_View> planets,
            List<Moon_View> moons,
            List<Layer2_Interaction_Object> interactions,
            int topPerType = 5
        )
        {
            // Ensure parameters are non-null and handle default topN
            if (topPerType <= 0) topPerType = 5;

            stars ??= new List<Star_View>();
            planets ??= new List<Planet_View>();
            moons ??= new List<Moon_View>();
            interactions ??= new List<Layer2_Interaction_Object>();

            // --- Boost Stars ---
            var boostedStars = BoostScores(
                stars,
                // Filter interactions specific to Stars
                interactions.Where(i => i.Object_Type == "Star").ToList(),
                s => s.Star.Id,
                s => s.Score,
                s => s.MatchPercentage,
                (s, pct) => s.MatchPercentage = pct, // Set new MatchPercentage
                (s, score) => s.Score = score,       // Set new Score
                s => s.Score,
                topPerType
            );

            // --- Boost Planets ---
            var boostedPlanets = BoostScores(
                planets,
                // Filter interactions specific to Planets
                interactions.Where(i => i.Object_Type == "Planet").ToList(),
                p => p.Planet.Id,
                p => p.Score,
                p => p.MatchPercentage,
                (p, pct) => p.MatchPercentage = pct,
                (p, score) => p.Score = score,
                p => p.Score,
                topPerType
            );

            // --- Boost Moons ---
            var boostedMoons = BoostScores(
                moons,
                // Filter interactions specific to Moons
                interactions.Where(i => i.Object_Type == "Moon").ToList(),
                m => m.Moon.Id,
                m => m.Score,
                m => m.MatchPercentage,
                (m, pct) => m.MatchPercentage = pct,
                (m, score) => m.Score = score,
                m => m.Score,
                topPerType
            );// Return a new object containing the boosted lists
            return new Layer2_Boost_Result
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

            // Combine, sort by score descending, and take the top N
            return combined
                .OrderByDescending(x => x.score)
                .Take(topN)
                .Select(x => x.obj)
                .ToList();
        }
    }
}