using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poppy_Universe_Engine
{
    public class Layer3_Boost_Result
    {
        public List<Star_View> RecommendedStars { get; set; }
        public List<Planet_View> RecommendedPlanets { get; set; }
        public List<Moon_View> RecommendedMoons { get; set; }
    }

    public class Layer_3_Poppys_Matrix_Booster
    {
        private const double PREF_WEIGHT = 0.6;
        private const double BASE_WEIGHT = 0.4;
        private const double MAX_BOOST_RATIO = 0.5; // Capped at 50% boost

        private double Normalize(double v) => Math.Max(0, Math.Min(10, v)) / 10.0;

        private double ComputeStarScore(Star_View s, Layer3_User_Matrix_Object prefs)
        {
            if (string.IsNullOrEmpty(s.Star.SpectralType)) return 0;
            string type = s.Star.SpectralType.Substring(0, 1).ToUpper();
            double pref = type switch
            {
                "A" => prefs.A,
                "B" => prefs.B,
                "F" => prefs.F,
                "G" => prefs.G,
                "K" => prefs.K,
                "M" => prefs.M,
                "O" => prefs.O,
                _ => 5.0
            };
            return Normalize(pref);
        }

        private double ComputePlanetScore(Planet_View p, Layer3_User_Matrix_Object prefs)
        {
            string cat = p.Planet.Type ?? "";
            double pref = 5.0;
            if (cat.Contains("Dwarf Planet")) pref = prefs.DwarfPlanet;
            else if (cat.Contains("Gas Giant")) pref = prefs.GasGiant;
            else if (cat.Contains("Ice Giant")) pref = prefs.IceGiant;
            else if (cat.Contains("Terrestrial")) pref = prefs.Terrestrial;
            return Normalize(pref);
        }

        private double ComputeMoonScore(Moon_View m, Layer3_User_Matrix_Object prefs)
        {
            string parent = m.Parent ?? "";
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
                _ => 5.0
            };
            return Normalize(pref);
        }

        public List<T> BoostScores<T>(
            List<T> layerObjects,
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
            if (layerObjects == null || layerObjects.Count == 0) return new List<T>();

            List<T> currentLayerObjects = layerObjects.ToList();

            double maxPossibleScore = 0;
            foreach (var obj in currentLayerObjects)
            {
                double score = getLayer1Score(obj);
                double matchPct = getLayer1MatchPct(obj);
                if (matchPct > 0 && score > 0)
                {
                    double calculatedMax = score / (matchPct / 100.0);
                    if (calculatedMax > maxPossibleScore) maxPossibleScore = calculatedMax;
                }
            }
            if (maxPossibleScore <= 0) maxPossibleScore = 100;

            double maxBoostAllowed = maxPossibleScore * MAX_BOOST_RATIO;

            foreach (var obj in currentLayerObjects)
            {
                double baseScore = getLayer1Score(obj);
                double originalMatchPct = getLayer1MatchPct(obj);
                double prefScore = getPreferenceScore(obj); // Range 0.0 to 1.0

                // ✨ PURE BOOST LOGIC: 
                // Any matrix value 0-5 results in 0 boost. 
                // Any value 5-10 scales from 0 to maxBoostAllowed.
                double boostMultiplier = Math.Max(0, (prefScore - 0.5) * 2);
                double boostValue = maxBoostAllowed * boostMultiplier;

                // Final Score is ALWAYS >= Base Score. No demotions.
                double boostedScore = Math.Round(Math.Min(maxPossibleScore, baseScore + boostValue), 2);
                setFinalScore(obj, boostedScore);

                double newMatchPercentage = Math.Round((boostedScore / maxPossibleScore) * 100.0, 2);
                if (newMatchPercentage > 100.00) newMatchPercentage = 100.00;
                setMatchPct(obj, newMatchPercentage);

                // Reporting logic
                double pointsAdded = newMatchPercentage - originalMatchPct;
                string finalBoostVal = pointsAdded > 0.01
                    ? $"+{Math.Round(pointsAdded, 1)}% Personalized"
                    : "0";

                if (obj is Star_View s) s.BoostDescription = finalBoostVal;
                else if (obj is Planet_View p) p.BoostDescription = finalBoostVal;
                else if (obj is Moon_View m) m.BoostDescription = finalBoostVal;
            }

            return currentLayerObjects
                .Where(obj => getFinalScore(obj) > 0)
                .OrderByDescending(obj => getFinalScore(obj))
                .Take(topN)
                .ToList();
        }

        public Layer3_Boost_Result BoostAll(
            List<Star_View> stars,
            List<Planet_View> planets,
            List<Moon_View> moons,
            Layer3_User_Matrix_Object prefs,
            int topPerType = 5
        )
        {
            if (topPerType <= 0) topPerType = 5;
            var boostedStars = BoostScores(stars, prefs, s => s.Score, s => s.MatchPercentage, s => ComputeStarScore(s, prefs), (s, pct) => s.MatchPercentage = pct, (s, score) => s.Score = score, s => s.Score, topPerType);
            var boostedPlanets = BoostScores(planets, prefs, p => p.Score, p => p.MatchPercentage, p => ComputePlanetScore(p, prefs), (p, pct) => p.MatchPercentage = pct, (p, score) => p.Score = score, p => p.Score, topPerType);
            var boostedMoons = BoostScores(moons, prefs, m => m.Score, m => m.MatchPercentage, m => ComputeMoonScore(m, prefs), (m, pct) => m.MatchPercentage = pct, (m, score) => m.Score = score, m => m.Score, topPerType);

            return new Layer3_Boost_Result { RecommendedStars = boostedStars, RecommendedPlanets = boostedPlanets, RecommendedMoons = boostedMoons };
        }

        internal List<object> GetCombinedTopResults(List<Star_View> stars, List<Planet_View> planets, List<Moon_View> moons, int topN = 15)
        {
            var combined = new List<(object obj, double score)>();
            if (stars != null) combined.AddRange(stars.Select(s => ((object)s, s.Score)));
            if (planets != null) combined.AddRange(planets.Select(p => ((object)p, p.Score)));
            if (moons != null) combined.AddRange(moons.Select(m => ((object)m, m.Score)));
            return combined.OrderByDescending(x => x.score).Take(topN).Select(x => x.obj).ToList();
        }
    }
}