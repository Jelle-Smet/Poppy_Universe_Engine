using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poppy_Universe_Engine
{
    // New class to hold the results from the boost operation (Layer 3)
    public class Layer3_Boost_Result
    {
        public List<Star_View> RecommendedStars { get; set; }
        public List<Planet_View> RecommendedPlanets { get; set; }
        public List<Moon_View> RecommendedMoons { get; set; }
    }

    public class Layer_3_Poppys_Matrix_Booster
    {
        private const double PREF_WEIGHT = 0.6; // Personalization influence
        private const double BASE_WEIGHT = 0.4; // Layer 1 base relevance
        private const double MAX_BOOST_RATIO = 0.5; // Cap at 50% of theoretical max

        private double Normalize(double v) => Math.Max(0, Math.Min(10, v)) / 10.0;

        private double ComputeStarScore(Star_View s, Layer3_User_Matrix_Object prefs)
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

        private double ComputePlanetScore(Planet_View p, Layer3_User_Matrix_Object prefs)
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

        private double ComputeMoonScore(Moon_View m, Layer3_User_Matrix_Object prefs)
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
            List<T> layerObjects, // Renamed for clarity on current layer input
            Layer3_User_Matrix_Object prefs,
            Func<T, double> getLayer1Score,
            Func<T, double> getLayer1MatchPct,
            Func<T, double> getPreferenceScore,
            Action<T, double> setMatchPct,
            Action<T, double> setFinalScore,
            Func<T, double> getFinalScore,
            int topN = 10
        )
        {
            if (layerObjects == null || layerObjects.Count == 0)
                return new List<T>();

            // **CRITICAL CHANGE: Create a copy of the list to work with.**
            // This ensures the input list (L1 or L2 results) is not sorted/filtered in place.
            List<T> currentLayerObjects = layerObjects.ToList();

            // Calculate max possible score from Layer 1/previous layer
            double maxPossibleScore = 0;
            foreach (var obj in currentLayerObjects)
            {
                double score = getLayer1Score(obj);
                double matchPct = getLayer1MatchPct(obj);

                if (matchPct > 0 && score > 0)
                {
                    // This calculation is the source of the precision issue, 
                    // as it uses existing rounded scores/percentages to determine the new max.
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
                double baseScore = getLayer1Score(obj);
                double prefScore = getPreferenceScore(obj);

                // Blend base score with preference-driven boost
                double boostValue = maxBoostAllowed * prefScore;

                // Original calculation: boostedScore = (baseScore * BASE_WEIGHT) + (baseScore * PREF_WEIGHT) + boostValue;
                // Since BASE_WEIGHT + PREF_WEIGHT = 1.0, this simplifies to: baseScore + boostValue
                // Let's use the simpler form, which represents the current score plus the preference boost.
                double boostedScore = baseScore + boostValue;

                // Cap to max allowed
                double maxAllowedForThisObject = baseScore + maxBoostAllowed;
                if (boostedScore > maxAllowedForThisObject)
                    boostedScore = maxAllowedForThisObject;

                if (boostedScore > maxPossibleScore)
                    boostedScore = maxPossibleScore;

                boostedScore = Math.Round(boostedScore, 2);
                setFinalScore(obj, boostedScore);

                double newMatchPercentage = Math.Round((boostedScore / maxPossibleScore) * 100.0, 2);

                // FIX: Explicitly cap the displayed percentage to prevent floating-point overflow
                if (newMatchPercentage > 100.00)
                    newMatchPercentage = 100.00;

                setMatchPct(obj, newMatchPercentage);

                // Set BoostDescription
                int boostPercent = (int)Math.Round((boostedScore - baseScore) / maxPossibleScore * 100);
                (obj as dynamic).BoostDescription = boostPercent > 0
                    ? $"Boosted by {boostPercent}%"
                    : "No boost";
            }

            return currentLayerObjects
                .Where(obj => getFinalScore(obj) > 0)
                .OrderByDescending(obj => getFinalScore(obj))
                .ThenByDescending(obj => getLayer1Score(obj))
                .Take(topN)
                .ToList();
        }

        // Changed from internal void to public Layer3_Boost_Result
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

            // Removed the lines that clear and re-add to the original lists.
            // stars.Clear(); stars.AddRange(boostedStars);
            // planets.Clear(); planets.AddRange(boostedPlanets);
            // moons.Clear(); moons.AddRange(boostedMoons);

            // Return a new object containing the boosted lists
            return new Layer3_Boost_Result
            {
                RecommendedStars = boostedStars,
                RecommendedPlanets = boostedPlanets,
                RecommendedMoons = boostedMoons
            };
        }

        // GetCombinedTopResults remains the same.
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