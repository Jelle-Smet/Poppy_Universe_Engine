using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Poppy_Universe_Engine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // ═══════════════════════════════════════════════════════════════
            // STEP 1: Catch Data from Node.js (Replaces Hardcoded Data)
            // ═══════════════════════════════════════════════════════════════

            // Read the JSON string
            string jsonInput = Console.In.ReadToEnd();

            // ✨ ADD THIS: Configure the options to allow strings for numbers
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
            };

            options.Converters.Add(new BoolConverter());
            options.Converters.Add(new StringConverter());

            // Use the options in your Deserialize call
            var payload = JsonSerializer.Deserialize<PayloadWrapper>(jsonInput, options);

            // This data feeds into the Layer 1 visibility and scoring engines.
            List<Star_Objects> Stars = payload.Pool.Stars;
            List<Planet_Objects> Planets = payload.Pool.Planets;
            List<Moon_Objects> Moons = payload.Pool.Moons;

            // ═══════════════════════════════════════════════════════════════
            // STEP 2: Configure User Location & Preferences
            // This simulates the observer and their personal data/preferences.
            // ═══════════════════════════════════════════════════════════════

            // Populate the User object from the Node.js payload
            var User = payload.User;

            // Use the coordinates and time sent from Node
            double userLat = User.Latitude;
            double userLon = User.Longitude;
            DateTime utcTime = User.ObservationTime;

            // Display User Configuration (using helper methods defined later in Program.cs)
            PrintSectionHeader("USER INFORMATION");

            Console.WriteLine($"┌─ User: {User.Name}");
            Console.WriteLine($"│  ID: {User.ID}");
            Console.WriteLine($"│  Date Time: {User.ObservationTime}");
            Console.WriteLine($"│  Position: Lat {User.Latitude:F2} | Long {User.Longitude:F2}");
            Console.WriteLine($"│  ┌─ Likes: ");
            Console.WriteLine($"│  │ Stars: [ {String.Join(" - ", User.LikedStars)} ]");
            Console.WriteLine($"│  │ Planets: [ {String.Join(" - ", User.LikedPlanets)} ]");
            Console.WriteLine($"│  │ Moons: [ {String.Join(" - ", User.LikedMoons)} ]");
            Console.WriteLine($"│  └─");
            Console.WriteLine($"└─");

            // ═══════════════════════════════════════════════════════════════
            // LAYER 1: PERSONALIZED RECOMMENDATIONS (Base Layer)
            // Calculates visibility, position (Alt/Az), weather chance, and initial score 
            // based on objective features and explicit user 'Liked' lists.
            // ═══════════════════════════════════════════════════════════════

            PrintSectionHeader("LAYER 1 PERSONALIZED RESULTS");

            // Initialize Layer 1 recommendation engines with the User object
            var starEngine = new Layer1_Star_Engine(User);
            var planetEngine = new Layer1_Planet_Engine(User);
            var moonEngine = new Layer1_Moon_Engine(User);

            // Get personalized recommendations based on user preferences. 
            var L1_recommendedStars = starEngine.GetRecommendedStars(Stars, utcTime, User) ?? new List<Star_View>();
            var L1_recommendedPlanets = planetEngine.GetRecommendedPlanets(Planets, utcTime, User) ?? new List<Planet_View>();
            var L1_recommendedMoons = moonEngine.GetRecommendedMoons(Moons, L1_recommendedPlanets, utcTime) ?? new List<Moon_View>();

            // Display Layer 1 Results (Stars)
            Console.WriteLine("🌟 ═══════════════ RECOMMENDED STARS ═══════════════ 🌟\n");
            foreach (var view in L1_recommendedStars)
            {
                Console.WriteLine($"┌─ Star: {view.Star.Name}");
                Console.WriteLine($"│  ID: {view.Source}");
                Console.WriteLine($"│  Type: {view.SpectralType}");
                // Show Alt/Az position for the current time and location
                Console.WriteLine($"│  Position: Alt {view.Altitude:F2}° | Az {view.Azimuth:F2}°");
                Console.WriteLine($"│  Brightness (Gmag): {view.Star.Gmag}");
                Console.WriteLine($"│  Visible: {view.IsVisible}");
                // Display L1 score and Match Percentage
                Console.WriteLine($"│  Match Score: {view.MatchPercentage:F2}% ({view.Score:F2})");
                Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                Console.WriteLine($"└─ {view.ChanceReason}\n");
            }
            Console.WriteLine($"   ✓ Total visible stars: {L1_recommendedStars.Count(s => s.IsVisible)}\n");

            // Display Layer 1 Results (Planets)
            Console.WriteLine("🪐 ═══════════════ RECOMMENDED PLANETS ═══════════════ 🪐\n");
            foreach (var view in L1_recommendedPlanets)
            {
                Console.WriteLine($"┌─ Planet: {view.Planet.Name}");
                // ... (output data)
                Console.WriteLine($"│  Match Score: {view.MatchPercentage:F2}% ({view.Score:F2})");
                Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                Console.WriteLine($"└─ {view.ChanceReason}\n");
            }
            Console.WriteLine($"   ✓ Total visible planets: {L1_recommendedPlanets.Count(p => p.IsVisible)}\n");

            // Display Layer 1 Results (Moons)
            Console.WriteLine("🌕 ═══════════════ RECOMMENDED MOONS ═══════════════ 🌕\n");
            foreach (var view in L1_recommendedMoons)
            {
                Console.WriteLine($"┌─ Moon: {view.Moon.Name} (orbits {view.Parent})");
                // ... (output data)
                Console.WriteLine($"│  Visible: {view.IsVisible}");
                Console.WriteLine($"│  Match Score: {view.MatchPercentage:F2}% ({view.Score:F2})");
                Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                Console.WriteLine($"└─ {view.ChanceReason}\n");
            }
            Console.WriteLine($"   ✓ Total visible moons: {L1_recommendedMoons.Count(m => m.IsVisible)}\n");

            PrintSectionFooter("END OF LAYER 1");

            // ═══════════════════════════════════════════════════════════════
            // PREPARE FINAL RESULTS (Defaults to Layer 1)
            // ═══════════════════════════════════════════════════════════════
            var finalStars = L1_recommendedStars;
            var finalPlanets = L1_recommendedPlanets;
            var finalMoons = L1_recommendedMoons;


            // ═══════════════════════════════════════════════════════════════
            // LAYER 2: TRENDING & POPULARITY BOOST
            // ═══════════════════════════════════════════════════════════════
            var L2_recommendedStars = new List<Star_View>();
            var L2_recommendedPlanets = new List<Planet_View>();
            var L2_recommendedMoons = new List<Moon_View>();
            // Only run this block if the Config says L2 is active
            if (payload.Config != null && payload.Config.L2 && payload.Layer2Data != null)
            {
                PrintSectionHeader("LAYER 2 BOOSTING RESULTS");

                int Top_Recommendations_Amount = 10;
                var booster = new Layer_2_Poppys_Trend_Booster();

                // Use the data sent from Node instead of the hardcoded list
                var L2_Results = booster.BoostAll(
                    L1_recommendedStars,
                    L1_recommendedPlanets,
                    L1_recommendedMoons,
                    payload.Layer2Data,
                    topPerType: Top_Recommendations_Amount
                );

                L2_recommendedStars = L2_Results.RecommendedStars;
                L2_recommendedPlanets = L2_Results.RecommendedPlanets;
                L2_recommendedMoons = L2_Results.RecommendedMoons;

                // Display Layer 2 Results (Boosted) - Keeping all your prints!
                Console.WriteLine("🌟 ═══════════════ POPULAR STARS (BOOSTED) ═══════════════ 🌟\n");
                foreach (var view in L2_recommendedStars)
                {
                    Console.WriteLine($"┌─ Star: {view.Star.Name}");
                    Console.WriteLine($"│  ID: {view.Source}");
                    Console.WriteLine($"│  Type: {view.SpectralType}");
                    Console.WriteLine($"│  Position: Alt {view.Altitude:F2}° | Az {view.Azimuth:F2}°");
                    Console.WriteLine($"│  Brightness (Gmag): {view.Star.Gmag}");
                    Console.WriteLine($"│  Visible: {view.IsVisible}");
                    Console.WriteLine($"│  **Boosted Score: {view.MatchPercentage:F2}% ({view.Score:F2})**");
                    Console.WriteLine($"│    --> Boost Applied: {view.BoostDescription}%");
                    Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                    Console.WriteLine($"└─ {view.ChanceReason}\n");
                }
                Console.WriteLine($"   ✓ Total visible stars: {finalStars.Count(s => s.IsVisible)}\n");

                Console.WriteLine("🪐 ═══════════════ POPULAR PLANETS (BOOSTED) ═══════════════ 🪐\n");
                foreach (var view in L2_recommendedPlanets)
                {
                    Console.WriteLine($"┌─ Planet: {view.Planet.Name}");
                    Console.WriteLine($"│  **Boosted Score: {view.MatchPercentage:F2}% ({view.Score:F2})**");
                    Console.WriteLine($"│    --> Boost Applied: {view.BoostDescription}%");
                    Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                    Console.WriteLine($"└─ {view.ChanceReason}\n");
                }
                Console.WriteLine($"   ✓ Total visible planets: {finalPlanets.Count(p => p.IsVisible)}\n");

                Console.WriteLine("🌕 ═══════════════ POPULAR MOONS (BOOSTED) ═══════════════ 🌕\n");
                foreach (var view in L2_recommendedMoons)
                {
                    Console.WriteLine($"┌─ Moon: {view.Moon.Name} (orbits {view.Parent})");
                    Console.WriteLine($"│  **Boosted Score: {view.MatchPercentage:F2}% ({view.Score:F2})**");
                    Console.WriteLine($"│    --> Boost Applied: {view.BoostDescription}%");
                    Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                    Console.WriteLine($"└─ {view.ChanceReason}\n");
                }
                Console.WriteLine($"   ✓ Total visible moons: {finalMoons.Count(m => m.IsVisible)}\n");

                PrintSectionFooter("END OF LAYER 2");

                // ═══════════════════════════════════════════════════════════════
                // PREPARE FINAL RESULTS (Defaults to Layer 1)
                // ═══════════════════════════════════════════════════════════════
                finalStars = L2_recommendedStars;
                finalPlanets = L2_recommendedPlanets;
                finalMoons = L2_recommendedMoons;
            }

            // ═══════════════════════════════════════════════════════════════
            // LAYER 3: PERSONALIZATION BOOST (MATRIX FACTORIZATION)
            // Applies a boost based on a calculated user preference matrix (e.g., from collaborative filtering).
            // ═══════════════════════════════════════════════════════════════
            // ✅ DYNAMIC CHECK: Only run if L3 is active in Config and data is provided by Node.js

            var L3_recommendedStars = new List<Star_View>();
            var L3_recommendedPlanets = new List<Planet_View>();
            var L3_recommendedMoons = new List<Moon_View>();

            if (payload.Config != null && payload.Config.L3 && payload.Layer3Data != null)
            {
                PrintSectionHeader("LAYER 2 + 3: PERSONALIZATION RESULTS");

                int L3_Top_Personalized_Amount = 10;
                var L3_personalizer = new Layer_3_Poppys_Matrix_Booster();

                // ✨ CHAINING: We pass the results FROM Layer 2 (finalStars, etc.) into Layer 3
                // and use payload.Layer3Data (the matrix from the DB) instead of hardcoded values.
                var L3_Results = L3_personalizer.BoostAll(
                    finalStars,       // Current stars (already boosted by L2)
                    finalPlanets,     // Current planets (already boosted by L2)
                    finalMoons,       // Current moons (already boosted by L2)
                    payload.Layer3Data,
                    topPerType: L3_Top_Personalized_Amount
                );

                // Assign the new, L3-boosted lists
                L3_recommendedStars = L3_Results.RecommendedStars;
                L3_recommendedPlanets = L3_Results.RecommendedPlanets;
                L3_recommendedMoons = L3_Results.RecommendedMoons;

                // Display Layer 3 Results (Personalized)
                Console.WriteLine("🌟 ═══════════════ PERSONALIZED STARS (LAYER 3) ═══════════════ 🌟\n");
                foreach (var view in L3_recommendedStars)
                {
                    Console.WriteLine($"┌─ Star: {view.Star.Name}");
                    // ... (output data)
                    // Highlight the L3 personalized score
                    Console.WriteLine($"│  **Personalized Score: {view.MatchPercentage:F2}% ({view.Score:F2})**");
                    Console.WriteLine($"│    --> {view.BoostDescription}"); // Shows the L3 boost description
                    Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                    Console.WriteLine($"└─ {view.ChanceReason}\n");
                }
                Console.WriteLine($"   ✓ Total visible stars: {L3_recommendedStars.Count(s => s.IsVisible)}\n");

                // Display Layer 3 Planets
                Console.WriteLine("🪐 ═══════════════ PERSONALIZED PLANETS (LAYER 3) ═══════════════ 🪐\n");
                foreach (var view in L3_recommendedPlanets)
                {
                    // ... (output data)
                    Console.WriteLine($"│  **Personalized Score: {view.MatchPercentage:F2}% ({view.Score:F2})**");
                    Console.WriteLine($"│    --> {view.BoostDescription}");
                    Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                    Console.WriteLine($"└─ {view.ChanceReason}\n");
                }
                Console.WriteLine($"   ✓ Total visible planets: {L3_recommendedPlanets.Count(p => p.IsVisible)}\n");

                // Display Layer 3 Moons
                Console.WriteLine("🌕 ═══════════════ PERSONALIZED MOONS (LAYER 3) ═══════════════ 🌕\n");
                foreach (var view in L3_recommendedMoons)
                {
                    // ... (output data)
                    Console.WriteLine($"│  **Personalized Score: {view.MatchPercentage:F2}% ({view.Score:F2})**");
                    Console.WriteLine($"│    --> {view.BoostDescription}");
                    Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                    Console.WriteLine($"└─ {view.ChanceReason}\n");
                }
                Console.WriteLine($"   ✓ Total visible moons: {L3_recommendedMoons.Count(m => m.IsVisible)}\n");

                PrintSectionFooter("END OF LAYER 3");

                // Overwrite the final lists with the Layer 3 personalized results
                finalStars = L3_Results.RecommendedStars;
                finalPlanets = L3_Results.RecommendedPlanets;
                finalMoons = L3_Results.RecommendedMoons;
            }

            // ═══════════════════════════════════════════════════════════════
            // LAYER 4: PERSONALIZATION BOOST (NEURAL NETWORK)
            // Applies the highest-confidence boost based on learned patterns from an NN model.
            // ═══════════════════════════════════════════════════════════════

            var L4_recommendedStars = new List<Star_View>();
            var L4_recommendedPlanets = new List<Planet_View>();
            var L4_recommendedMoons = new List<Moon_View>();

            if (payload.Config.L4 && payload.Layer4Data != null)
            {
                PrintSectionHeader("LAYER 4 NEURAL NETWORK BOOST");

                var L4_personalizer = new Layer_4_Poppys_NN_Booster();

                // ✨ CHAINING: Use the results from the previous active layer
                // If L3 ran, use L3_recommendedStars. If not, use finalStars.
                var L4_Results = L4_personalizer.BoostAll(
                    finalStars,    // 👈 This variable holds the "latest" results from L2 or L3
                    finalPlanets,
                    finalMoons,
                    payload.Layer4Data,
                    topPerType: 10
                );

                // Assign the new, L3-boosted lists
                L4_recommendedStars = L4_Results.RecommendedStars;
                L4_recommendedPlanets = L4_Results.RecommendedPlanets;
                L4_recommendedMoons = L4_Results.RecommendedMoons;

                // Display Layer 4 Results (Personalized)
                Console.WriteLine("🌟 ═══════════════ PERSONALIZED STARS (LAYER 4) ═══════════════ 🌟\n");
                foreach (var view in L4_recommendedStars)
                {
                    Console.WriteLine($"┌─ Star: {view.Star.Name}");
                    // ... (output data)
                    // Highlight the final L4 score
                    Console.WriteLine($"│  **Personalized Score: {view.MatchPercentage:F2}% ({view.Score:F2})**");
                    Console.WriteLine($"│    --> {view.BoostDescription}"); // Shows the L4 NN boost description
                    Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                    Console.WriteLine($"└─ {view.ChanceReason}\n");
                }
                Console.WriteLine($"   ✓ Total visible stars: {L4_recommendedStars.Count(s => s.IsVisible)}\n"); // Fixed variable name here

                Console.WriteLine("🪐 ═══════════════ PERSONALIZED PLANETS (LAYER 4) ═══════════════ 🪐\n");
                foreach (var view in L4_recommendedPlanets)
                {
                    // ... (output data)
                    Console.WriteLine($"│  **Personalized Score: {view.MatchPercentage:F2}% ({view.Score:F2})**");
                    Console.WriteLine($"│    --> {view.BoostDescription}");
                    Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                    Console.WriteLine($"└─ {view.ChanceReason}\n");
                }
                Console.WriteLine($"   ✓ Total visible planets: {L4_recommendedPlanets.Count(p => p.IsVisible)}\n"); // Fixed variable name here

                Console.WriteLine("🌕 ═══════════════ PERSONALIZED MOONS (LAYER 4) ═══════════════ 🌕\n");
                foreach (var view in L4_recommendedMoons)
                {
                    // ... (output data)
                    Console.WriteLine($"│  **Personalized Score: {view.MatchPercentage:F2}% ({view.Score:F2})**");
                    Console.WriteLine($"│    --> {view.BoostDescription}");
                    Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                    Console.WriteLine($"└─ {view.ChanceReason}\n");
                }
                Console.WriteLine($"   ✓ Total visible moons: {L4_recommendedMoons.Count(m => m.IsVisible)}\n"); // Fixed variable name here

                PrintSectionFooter("END OF LAYER 4");

                // Update final lists for JSON return
                finalStars = L4_Results.RecommendedStars;
                finalPlanets = L4_Results.RecommendedPlanets;
                finalMoons = L4_Results.RecommendedMoons;
            }


            // ═══════════════════════════════════════════════════════════════
            // LAYER 5: RANK FUSION (GENETIC ALGORITHM)
            // The final stage that takes the ranked lists from L1, L2, L3, and L4, 
            // finds the optimal weight combination (W1-W4) to create a consensus, and outputs the final rankings.
            // ═══════════════════════════════════════════════════════════════
            var L5_recommendedStars = new List<Layer5_Poppys_GA_Object>();
            var L5_recommendedPlanets = new List<Layer5_Poppys_GA_Object>();
            var L5_recommendedMoons = new List<Layer5_Poppys_GA_Object>();
            if (payload.Config.L5)
            {
                PrintSectionHeader("LAYER 5 GA RANK FUSION");
                var layer5Handler = new Layer5_Poppys_GA_Handler(seed: 42);

                var L5_Results = layer5Handler.RunOptimization(
                    user: payload.User, // From payload
                    L1_Stars: L1_recommendedStars,
                    L1_Planets: L1_recommendedPlanets,
                    L1_Moons: L1_recommendedMoons,
                    L2_Stars: L2_recommendedStars,
                    L2_Planets: L2_recommendedPlanets,
                    L2_Moons: L2_recommendedMoons,
                    L3_Stars: L3_recommendedStars,
                    L3_Planets: L3_recommendedPlanets,
                    L3_Moons: L3_recommendedMoons,
                    L4_Stars: L4_recommendedStars,
                    L4_Planets: L4_recommendedPlanets,
                    L4_Moons: L4_recommendedMoons
                );

                L5_recommendedStars = L5_Results.Stars;
                L5_recommendedPlanets = L5_Results.Planets;
                L5_recommendedMoons = L5_Results.Moons;


                // ═══════════════════════════════════════════════════════════════
                // Display Layer 5 Results (FINAL RECOMMENDATIONS)
                // ═══════════════════════════════════════════════════════════════

                Console.WriteLine("\n🌟 ═══════════════ FINAL RANKED STARS (LAYER 5 GA) ═══════════════ 🌟\n");
                foreach (var obj in L5_recommendedStars)
                {
                    Console.WriteLine($"┌─ Star: {obj.Object_Name}");
                    Console.WriteLine($"│  ID: {obj.Object_ID}");
                    Console.WriteLine($"│  Type: {obj.SpectralType}");
                    Console.WriteLine($"│  Position: Alt {obj.Altitude:F2}° | Az {obj.Azimuth:F2}°");
                    Console.WriteLine($"│  Brightness (Gmag): {obj.Gmag}");
                    Console.WriteLine($"│  Visible: {obj.IsVisible}");
                    // Highlight the final output of the entire system: L5 Rank and Score
                    Console.WriteLine($"│  **Final GA Rank: #{obj.Layer5_FinalRank + 1} (Score: {obj.Layer5_FinalScore:F4})**");
                    // Show the input ranks that determined the final rank
                    Console.WriteLine($"│  Layer Ranks: Layer 1 = {obj.Layer1_Rank + 1} | Layer 2 = {obj.Layer2_Rank + 1} | Layer 3 = {obj.Layer3_Rank + 1} | Layer 4 = {obj.Layer4_Rank + 1}");
                    Console.WriteLine($"│  Match Score: {obj.MatchPercentage:F2}%");
                    Console.WriteLine($"│  Weather Visibility: {obj.VisibilityChance}%");
                    Console.WriteLine($"└─ {obj.ChanceReason}\n");
                }
                Console.WriteLine($"   ✓ Total visible stars: {L5_recommendedStars.Count(s => s.IsVisible)}\n");

                Console.WriteLine("🪐 ═══════════════ FINAL RANKED PLANETS (LAYER 5 GA) ═══════════════ 🪐\n");
                foreach (var obj in L5_recommendedPlanets)
                {
                    Console.WriteLine($"┌─ Planet: {obj.Object_Name}");
                    // ... (output data)
                    Console.WriteLine($"│  **Final GA Rank: #{obj.Layer5_FinalRank + 1} (Score: {obj.Layer5_FinalScore:F4})**");
                    Console.WriteLine($"│  Layer Ranks: Layer 1 = {obj.Layer1_Rank + 1} | Layer 2 = {obj.Layer2_Rank + 1} | Layer 3 = {obj.Layer3_Rank + 1} | Layer 4 = {obj.Layer4_Rank + 1}");
                    Console.WriteLine($"│  Match Score: {obj.MatchPercentage:F2}%");
                    Console.WriteLine($"│  Weather Visibility: {obj.VisibilityChance}%");
                    Console.WriteLine($"└─ {obj.ChanceReason}\n");
                }
                Console.WriteLine($"   ✓ Total visible planets: {L5_recommendedPlanets.Count(p => p.IsVisible)}\n");

                Console.WriteLine("🌕 ═══════════════ FINAL RANKED MOONS (LAYER 5 GA) ═══════════════ 🌕\n");
                foreach (var obj in L5_recommendedMoons)
                {
                    Console.WriteLine($"┌─ Moon: {obj.Object_Name} (orbits {obj.Parent})");
                    // ... (output data)
                    Console.WriteLine($"│  **Final GA Rank: #{obj.Layer5_FinalRank + 1} (Score: {obj.Layer5_FinalScore:F4})**");
                    Console.WriteLine($"│  Layer Ranks: Layer 1 = {obj.Layer1_Rank + 1} | Layer 2 = {obj.Layer2_Rank + 1} | Layer 3 = {obj.Layer3_Rank + 1} | Layer 4 ={obj.Layer4_Rank + 1}");
                    Console.WriteLine($"│  Match Score: {obj.MatchPercentage:F2}%");
                    Console.WriteLine($"│  Weather Visibility: {obj.VisibilityChance}%");
                    Console.WriteLine($"└─ {obj.ChanceReason}\n");
                }
                Console.WriteLine($"   ✓ Total visible moons: {L5_recommendedMoons.Count(m => m.IsVisible)}\n");

                PrintSectionFooter("END OF LAYER 5 - FINAL RECOMMENDATIONS");
            }

            // ═══════════════════════════════════════════════════════════════
            // FINAL STEP: RETURN DATA TO NODE
            // ═══════════════════════════════════════════════════════════════
            Console.WriteLine("---JSON_START---");

            // ✨ THE FIX: Explicitly cast to 'object' to satisfy the compiler
            object finalOutputStars;
            object finalOutputPlanets;
            object finalOutputMoons;

            if (payload.Config != null && payload.Config.L5)
            {
                finalOutputStars = L5_recommendedStars;
                finalOutputPlanets = L5_recommendedPlanets;
                finalOutputMoons = L5_recommendedMoons;
            }
            else
            {
                finalOutputStars = finalStars;
                finalOutputPlanets = finalPlanets;
                finalOutputMoons = finalMoons;
            }

            Console.WriteLine(JsonSerializer.Serialize(new
            {
                Stars = finalOutputStars,
                Planets = finalOutputPlanets,
                Moons = finalOutputMoons
            }));
        }

        // ═══════════════════════════════════════════════════════════════
        // Helper Methods for Pretty Console Output
        // These methods are necessary to execute the PrintSectionHeader/Footer calls above.
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Prints a nicely formatted section header
        /// </summary>
        private static void PrintSectionHeader(string title)
        {
            string border = new string('═', 70);
            string spacedTitle = $"█  {title}  █";
            Console.WriteLine("\n" + border);
            Console.WriteLine(spacedTitle.PadLeft((70 + spacedTitle.Length) / 2).PadRight(70));
            Console.WriteLine(border + "\n");
        }

        /// <summary>
        /// Prints a nicely formatted section footer
        /// </summary>
        private static void PrintSectionFooter(string title)
        {
            string border = new string('═', 70);
            string spacedTitle = $">>> {title} <<<";
            Console.WriteLine("\n" + border);
            Console.WriteLine(spacedTitle.PadLeft((70 + spacedTitle.Length) / 2).PadRight(70));
            Console.WriteLine(border + "\n");
        }
    }

    public class EngineConfig
    {
        public bool L2 { get; set; }
        public bool L3 { get; set; }
        public bool L4 { get; set; }
        public bool L5 { get; set; }
    }

    public class PayloadWrapper
    {
        public User_Object User { get; set; }
        public PoolWrapper Pool { get; set; }
        public EngineConfig Config { get; set; } // The Toggles
        public List<Layer2_Interaction_Object> Layer2Data { get; set; } // Using existing class!
        public Layer3_User_Matrix_Object Layer3Data { get; set; } // Using existing class!

        public Layer4_User_NN_Object Layer4Data { get; set; } // Using existing class!
    }

    public class PoolWrapper
    {
        public List<Star_Objects> Stars { get; set; }
        public List<Planet_Objects> Planets { get; set; }
        public List<Moon_Objects> Moons { get; set; }
    }

    public class BoolConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32() == 1;
            }
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString() == "1" || reader.GetString()?.ToLower() == "true";
            }
            return reader.GetBoolean();
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }

    public class StringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.TryGetInt64(out long l) ? l.ToString() : reader.GetDouble().ToString();
            }
            return reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}