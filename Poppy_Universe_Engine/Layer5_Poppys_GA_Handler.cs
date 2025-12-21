// Layer5_Poppys_GA_Handler.cs
// Handler that converts data and runs GA optimization
// Program.cs handles the display logic (consistent with other layers)

using System;
using System.Collections.Generic;
using System.Linq;

namespace Poppy_Universe_Engine
{
    /// <summary>
    /// Handler class for Layer 5 Genetic Algorithm rank fusion
    /// Handles data conversion and GA execution, leaving display to Program.cs
    /// </summary>
    public class Layer5_Poppys_GA_Handler
    {
        private readonly Layer5_Poppys_GA_Engine gaEngine;

        public Layer5_Poppys_GA_Handler(int? seed = null)
        {
            gaEngine = new Layer5_Poppys_GA_Engine(seed);
        }

        /// <summary>
        /// Main method: Converts Layer 1-4 results, runs GA, and returns final rankings
        /// </summary>
        public Layer5_GA_Result RunOptimization(
            User_Object user,
            List<Star_View> L1_Stars,
            List<Planet_View> L1_Planets,
            List<Moon_View> L1_Moons,
            List<Star_View> L2_Stars,
            List<Planet_View> L2_Planets,
            List<Moon_View> L2_Moons,
            List<Star_View> L3_Stars,
            List<Planet_View> L3_Planets,
            List<Moon_View> L3_Moons,
            List<Star_View> L4_Stars,
            List<Planet_View> L4_Planets,
            List<Moon_View> L4_Moons)
        {
            Console.WriteLine("🧬 Preparing data for Genetic Algorithm optimization...\n");

            // Convert all layer data to GA input format
            List<Layer5_Poppys_GA_Object> inputObjects = ConvertToGAInput(
                user,
                L1_Stars, L1_Planets, L1_Moons,
                L2_Stars, L2_Planets, L2_Moons,
                L3_Stars, L3_Planets, L3_Moons,
                L4_Stars, L4_Planets, L4_Moons
            );

            Console.WriteLine($"✓ Prepared {inputObjects.Count} objects for GA optimization");
            Console.WriteLine($"  Stars: {inputObjects.Count(o => o.Object_Type == "Star")}");
            Console.WriteLine($"  Planets: {inputObjects.Count(o => o.Object_Type == "Planet")}");
            Console.WriteLine($"  Moons: {inputObjects.Count(o => o.Object_Type == "Moon")}");
            Console.WriteLine("\n🧬 Running Genetic Algorithm to find optimal rank fusion weights...\n");

            // Run GA optimization
            Layer5_GA_Result results = gaEngine.OptimizeRankFusion(inputObjects);

            Console.WriteLine("\n✓ GA Optimization Complete!");
            Console.WriteLine($"  Final Stars: {results.Stars?.Count ?? 0}");
            Console.WriteLine($"  Final Planets: {results.Planets?.Count ?? 0}");
            Console.WriteLine($"  Final Moons: {results.Moons?.Count ?? 0}");

            // Display optimal weights
            if (results.BestStarWeights != null)
                Console.WriteLine($"\n📊 Optimal Star Weights: {results.BestStarWeights}");
            if (results.BestPlanetWeights != null)
                Console.WriteLine($"📊 Optimal Planet Weights: {results.BestPlanetWeights}");
            if (results.BestMoonWeights != null)
                Console.WriteLine($"📊 Optimal Moon Weights: {results.BestMoonWeights}");

            return results;
        }

        #region Private Helper Methods

        /// <summary>
        /// Converts all layer view objects to Layer5_Poppys_GA_Object format
        /// </summary>
        private List<Layer5_Poppys_GA_Object> ConvertToGAInput(
            User_Object user,
            List<Star_View> L1_Stars, List<Planet_View> L1_Planets, List<Moon_View> L1_Moons,
            List<Star_View> L2_Stars, List<Planet_View> L2_Planets, List<Moon_View> L2_Moons,
            List<Star_View> L3_Stars, List<Planet_View> L3_Planets, List<Moon_View> L3_Moons,
            List<Star_View> L4_Stars, List<Planet_View> L4_Planets, List<Moon_View> L4_Moons)
        {
            var inputObjects = new List<Layer5_Poppys_GA_Object>();

            // Convert Stars
            inputObjects.AddRange(ConvertStars(user.ID, L1_Stars, L2_Stars, L3_Stars, L4_Stars));

            // Convert Planets
            inputObjects.AddRange(ConvertPlanets(user.ID, L1_Planets, L2_Planets, L3_Planets, L4_Planets));

            // Convert Moons
            inputObjects.AddRange(ConvertMoons(user.ID, L1_Moons, L2_Moons, L3_Moons, L4_Moons));

            return inputObjects;
        }

        private List<Layer5_Poppys_GA_Object> ConvertStars(
            int userId,
            List<Star_View> L1, List<Star_View> L2, List<Star_View> L3, List<Star_View> L4)
        {
            var result = new List<Layer5_Poppys_GA_Object>();

            for (int i = 0; i < L1.Count; i++)
            {
                var starView = L1[i];
                var gaObject = new Layer5_Poppys_GA_Object
                {
                    User_ID = userId,
                    Object_ID = starView.Id,
                    Object_Type = "Star",
                    Object_Name = starView.Star.Name,

                    Layer1_Rank = i,
                    Layer2_Rank = GetRank(L2, s => s.Id == starView.Id),
                    Layer3_Rank = GetRank(L3, s => s.Id == starView.Id),
                    Layer4_Rank = GetRank(L4, s => s.Id == starView.Id),

                    MatchPercentage = starView.MatchPercentage,
                    Score = starView.Score,
                    IsVisible = starView.IsVisible,
                    Altitude = starView.Altitude,
                    Azimuth = starView.Azimuth,
                    VisibilityChance = (int)starView.VisibilityChance,
                    ChanceReason = starView.ChanceReason,
                    BoostDescription = starView.BoostDescription,
                    SpectralType = starView.SpectralType,
                    Gmag = starView.Star.Gmag
                };

                result.Add(gaObject);
            }

            return result;
        }

        private List<Layer5_Poppys_GA_Object> ConvertPlanets(
            int userId,
            List<Planet_View> L1, List<Planet_View> L2, List<Planet_View> L3, List<Planet_View> L4)
        {
            var result = new List<Layer5_Poppys_GA_Object>();

            for (int i = 0; i < L1.Count; i++)
            {
                var planetView = L1[i];
                var gaObject = new Layer5_Poppys_GA_Object
                {
                    User_ID = userId,
                    Object_ID = planetView.Id,
                    Object_Type = "Planet",
                    Object_Name = planetView.Planet.Name,

                    Layer1_Rank = i,
                    Layer2_Rank = GetRank(L2, p => p.Id == planetView.Id),
                    Layer3_Rank = GetRank(L3, p => p.Id == planetView.Id),
                    Layer4_Rank = GetRank(L4, p => p.Id == planetView.Id),

                    MatchPercentage = planetView.MatchPercentage,
                    Score = planetView.Score,
                    IsVisible = planetView.IsVisible,
                    Altitude = planetView.Altitude,
                    Azimuth = planetView.Azimuth,
                    VisibilityChance = (int)planetView.VisibilityChance,
                    ChanceReason = planetView.ChanceReason,
                    BoostDescription = planetView.BoostDescription,
                    Type = planetView.Type,
                    Color = planetView.Planet.Color,
                    Diameter = planetView.Planet.Diameter ?? 0,
                    Mass = planetView.Planet.Mass ?? 0
                };

                result.Add(gaObject);
            }

            return result;
        }

        private List<Layer5_Poppys_GA_Object> ConvertMoons(
            int userId,
            List<Moon_View> L1, List<Moon_View> L2, List<Moon_View> L3, List<Moon_View> L4)
        {
            var result = new List<Layer5_Poppys_GA_Object>();

            for (int i = 0; i < L1.Count; i++)
            {
                var moonView = L1[i];
                var gaObject = new Layer5_Poppys_GA_Object
                {
                    User_ID = userId,
                    Object_ID = moonView.Id,
                    Object_Type = "Moon",
                    Object_Name = moonView.Moon.Name,

                    Layer1_Rank = i,
                    Layer2_Rank = GetRank(L2, m => m.Id == moonView.Id),
                    Layer3_Rank = GetRank(L3, m => m.Id == moonView.Id),
                    Layer4_Rank = GetRank(L4, m => m.Id == moonView.Id),

                    MatchPercentage = moonView.MatchPercentage,
                    Score = moonView.Score,
                    IsVisible = moonView.IsVisible,
                    Altitude = moonView.Altitude,
                    Azimuth = moonView.Azimuth,
                    VisibilityChance = (int)moonView.VisibilityChance,
                    ChanceReason = moonView.ChanceReason,
                    BoostDescription = moonView.BoostDescription,
                    Parent = moonView.Parent,
                    Color = moonView.Moon.Color,
                    Diameter = (moonView.Moon.Diameter ?? 0.0),
                    Mass = (moonView.Moon.Mass ?? 0.0),
                    Composition = moonView.Moon.Composition,
                    SurfaceFeatures = moonView.Moon.SurfaceFeatures
                };

                result.Add(gaObject);
            }

            return result;
        }

        /// <summary>
        /// Helper to get rank position or 999 if not found
        /// </summary>
        private int GetRank<T>(List<T> list, Func<T, bool> predicate)
        {
            int index = list.FindIndex(item => predicate(item));
            return index == -1 ? 998 : index;
        }

        #endregion
    }
}