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
            // FINAL STEP: RETURN DATA TO NODE
            // ═══════════════════════════════════════════════════════════════
            // We print a marker so Node knows where the raw data is
            Console.WriteLine("---JSON_START---");
            Console.WriteLine(JsonSerializer.Serialize(new
            {
                Stars = L1_recommendedStars,
                Planets = L1_recommendedPlanets,
                Moons = L1_recommendedMoons
            }));

            /*// ═══════════════════════════════════════════════════════════════
            // LAYER 2: TRENDING & POPULARITY BOOST
            // Modifies L1 scores by applying a weighting based on global interaction data.
            // ═══════════════════════════════════════════════════════════════

            PrintSectionHeader("LAYER 2 BOOSTING RESULTS");

            // Define simulated interaction data (Layer 2 Input)
            List<Layer2_Interaction_Object> interactions = new List<Layer2_Interaction_Object>
            {
                    // Interactions include raw counts (num_views, num_clicks, num_favorites) and 
                    // a computed 'trending_score' which Layer_2_Poppys_Trend_Booster uses to calculate the boost.
    
                    // Popular Stars
                    new Layer2_Interaction_Object { Object_Type = "Star", Object_ID = 12, total_interactions = 50, num_views = 12.5, num_clicks = 10.3, num_favorites = 8.2, trending_score = 45.1 },
                    new Layer2_Interaction_Object { Object_Type = "Star", Object_ID = 2, total_interactions = 45, num_views = 11.0, num_clicks = 9.1, num_favorites = 7.8, trending_score = 42.5 },
                    new Layer2_Interaction_Object { Object_Type = "Star", Object_ID = 3, total_interactions = 40, num_views = 10.0, num_clicks = 8.0, num_favorites = 6.5, trending_score = 39.8 },
                    new Layer2_Interaction_Object { Object_Type = "Star", Object_ID = 4, total_interactions = 35, num_views = 9.5, num_clicks = 7.5, num_favorites = 6.0, trending_score = 37.2 },

                    // Popular Planets
                    new Layer2_Interaction_Object { Object_Type = "Planet", Object_ID = 1, total_interactions = 100, num_views = 25.0, num_clicks = 20.0, num_favorites = 15.0, trending_score = 80.0 }, // Sun
                    new Layer2_Interaction_Object { Object_Type = "Planet", Object_ID = 2, total_interactions = 75, num_views = 18.0, num_clicks = 14.0, num_favorites = 10.0, trending_score = 65.0 }, // Mercury
                    new Layer2_Interaction_Object { Object_Type = "Planet", Object_ID = 3, total_interactions = 80, num_views = 20.0, num_clicks = 16.0, num_favorites = 12.0, trending_score = 70.0 }, // Venus
                    new Layer2_Interaction_Object { Object_Type = "Planet", Object_ID = 4, total_interactions = 90, num_views = 22.0, num_clicks = 18.0, num_favorites = 14.0, trending_score = 75.0 }, // Earth
                    new Layer2_Interaction_Object { Object_Type = "Planet", Object_ID = 5, total_interactions = 70, num_views = 17.0, num_clicks = 13.0, num_favorites = 9.0, trending_score = 60.0 }, // Mars

                    // Popular Moons
                    new Layer2_Interaction_Object { Object_Type = "Moon", Object_ID = 1, total_interactions = 30, num_views = 8.0, num_clicks = 6.0, num_favorites = 5.0, trending_score = 28.0 }, // Earth's Moon
                    new Layer2_Interaction_Object { Object_Type = "Moon", Object_ID = 2, total_interactions = 15, num_views = 4.0, num_clicks = 3.0, num_favorites = 2.5, trending_score = 14.0 }, // Phobos
                    new Layer2_Interaction_Object { Object_Type = "Moon", Object_ID = 3, total_interactions = 12, num_views = 3.0, num_clicks = 2.5, num_favorites = 2.0, trending_score = 11.0 }, // Deimos
                    new Layer2_Interaction_Object { Object_Type = "Moon", Object_ID = 4, total_interactions = 25, num_views = 7.0, num_clicks = 5.5, num_favorites = 4.5, trending_score = 22.0 }, // Io
                    new Layer2_Interaction_Object { Object_Type = "Moon", Object_ID = 9, total_interactions = 20, num_views = 6.0, num_clicks = 4.5, num_favorites = 3.5, trending_score = 19.0 } // Enceladus
                };

            // Apply the popularity boost, using L1 results as the base.
            int Top_Recommendations_Amount = 10;
            var booster = new Layer_2_Poppys_Trend_Booster();
            var L2_Results = booster.BoostAll(
                L1_recommendedStars,
                L1_recommendedPlanets,
                L1_recommendedMoons,
                interactions,
                topPerType: Top_Recommendations_Amount // Filters results to top N after boosting
            );

            // Get the boosted and filtered lists
            var L2_recommendedStars = L2_Results.RecommendedStars;
            var L2_recommendedPlanets = L2_Results.RecommendedPlanets;
            var L2_recommendedMoons = L2_Results.RecommendedMoons;


            // Display Layer 2 Results (Boosted)
            Console.WriteLine("🌟 ═══════════════ POPULAR STARS (BOOSTED) ═══════════════ 🌟\n");
            foreach (var view in L2_recommendedStars)
            {
                Console.WriteLine($"┌─ Star: {view.Star.Name}");
                Console.WriteLine($"│  ID: {view.Source}");
                Console.WriteLine($"│  Type: {view.SpectralType}");
                Console.WriteLine($"│  Position: Alt {view.Altitude:F2}° | Az {view.Azimuth:F2}°");
                Console.WriteLine($"│  Brightness (Gmag): {view.Star.Gmag}");
                Console.WriteLine($"│  Visible: {view.IsVisible}");
                // Highlight the boosted score and the boost description
                Console.WriteLine($"│  **Boosted Score: {view.MatchPercentage:F2}% ({view.Score:F2})**");
                Console.WriteLine($"│    --> Boost Applied: {view.BoostDescription}%");
                Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                Console.WriteLine($"└─ {view.ChanceReason}\n");
            }
            Console.WriteLine($"   ✓ Total visible stars: {L2_recommendedStars.Count(s => s.IsVisible)}\n");

            Console.WriteLine("🪐 ═══════════════ POPULAR PLANETS (BOOSTED) ═══════════════ 🪐\n");
            foreach (var view in L2_recommendedPlanets)
            {
                Console.WriteLine($"┌─ Planet: {view.Planet.Name}");
                // ... (output data)
                Console.WriteLine($"│  **Boosted Score: {view.MatchPercentage:F2}% ({view.Score:F2})**");
                Console.WriteLine($"│    --> Boost Applied: {view.BoostDescription}%");
                Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                Console.WriteLine($"└─ {view.ChanceReason}\n");
            }
            Console.WriteLine($"   ✓ Total visible planets: {L2_recommendedPlanets.Count(p => p.IsVisible)}\n");

            Console.WriteLine("🌕 ═══════════════ POPULAR MOONS (BOOSTED) ═══════════════ 🌕\n");
            foreach (var view in L2_recommendedMoons)
            {
                Console.WriteLine($"┌─ Moon: {view.Moon.Name} (orbits {view.Parent})");
                // ... (output data)
                Console.WriteLine($"│  **Boosted Score: {view.MatchPercentage:F2}% ({view.Score:F2})**");
                Console.WriteLine($"│    --> Boost Applied: {view.BoostDescription}%");
                Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                Console.WriteLine($"└─ {view.ChanceReason}\n");
            }
            Console.WriteLine($"   ✓ Total visible moons: {L2_recommendedMoons.Count(m => m.IsVisible)}\n");

            PrintSectionFooter("END OF LAYER 2");

            // ═══════════════════════════════════════════════════════════════
            // LAYER 3: PERSONALIZATION BOOST (MATRIX FACTORIZATION)
            // Applies a boost based on a calculated user preference matrix (e.g., from collaborative filtering).
            // ═══════════════════════════════════════════════════════════════

            PrintSectionHeader("LAYER 3 BOOSTING RESULTS (PERSONALIZATION)");

            // Define user preference matrix (L3 Input)
            // This matrix contains pre-calculated scores (0-10) reflecting the user's affinity 
            // for specific categories (e.g., G-type stars, Terrestrial planets).
            Layer3_User_Matrix_Object L3_userPreferences = new Layer3_User_Matrix_Object
            {
                User_ID = 1,

                // Star preferences (spectral types)
                A = 7.5,
                B = 3.2,
                F = 6.8,
                G = 9.1,
                K = 5.4,
                M = 8.0,
                O = 2.1,

                // Planet preferences
                DwarfPlanet = 4.5,
                GasGiant = 8.5,
                IceGiant = 7.2,
                Terrestrial = 9.0,

                // Moon preferences (by parent body)
                Earth = 8.8,
                Eris = 3.0,
                Haumea = 2.5,
                Jupiter = 9.5,
                Makemake = 2.0,
                Mars = 7.0,
                Neptune = 6.5,
                Pluto = 5.5,
                Saturn = 8.0,
                Uranus = 6.0
            };

            // Apply personalization boost (max boost ratio of 50% defined in Layer_3_Poppys_Matrix_Booster)
            int L3_Top_Personalized_Amount = 10;
            var L3_personalizer = new Layer_3_Poppys_Matrix_Booster();

            // The input here should ideally be the L2 results (L2_recommended*), as L3 builds upon L2.
            var L3_Results = L3_personalizer.BoostAll(
                // Assuming L2_recommended* variables contain the results from the previous block
                L2_recommendedStars,
                L2_recommendedPlanets,
                L2_recommendedMoons,
                L3_userPreferences,
                topPerType: L3_Top_Personalized_Amount
            );

            // Assign the new, L3-boosted lists
            var L3_recommendedStars = L3_Results.RecommendedStars;
            var L3_recommendedPlanets = L3_Results.RecommendedPlanets;
            var L3_recommendedMoons = L3_Results.RecommendedMoons;


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

            // ═══════════════════════════════════════════════════════════════
            // LAYER 4: PERSONALIZATION BOOST (NEURAL NETWORK)
            // Applies the highest-confidence boost based on learned patterns from an NN model.
            // ═══════════════════════════════════════════════════════════════

            PrintSectionHeader("LAYER 4 BOOSTING RESULTS (PERSONALIZATION)");

            // Define user preference matrix (L4 Input - NN Output)
            // Values are NN-predicted scores (0-10) for maximum predicted user engagement.
            Layer4_User_NN_Object L4_userPreferences = new Layer4_User_NN_Object
            {
                User_ID = 1,

                // Star preferences (spectral types)
                A = 6.3,
                B = 4.7,
                F = 7.5,
                G = 8.2,
                K = 5.9,
                M = 7.8,
                O = 3.0,

                // Planet preferences
                DwarfPlanet = 5.2,
                GasGiant = 7.9,
                IceGiant = 6.8,
                Terrestrial = 8.7,

                // Moon preferences (by parent body)
                Earth = 9.0,
                Eris = 2.8,
                Haumea = 3.1,
                Jupiter = 9.2,
                Makemake = 1.9,
                Mars = 6.5,
                Neptune = 7.1,
                Pluto = 5.8,
                Saturn = 8.3,
                Uranus = 6.7
            };

            // Apply NN personalization boost (max boost ratio of 75% defined in Layer_4_Poppys_NN_Booster)
            int L4_Top_Personalized_Amount = 10;
            var L4_personalizer = new Layer_4_Poppys_NN_Booster();

            // The input here should be the L3 results (L3_recommended*)
            var L4_Results = L4_personalizer.BoostAll(
                L3_recommendedStars,
                L3_recommendedPlanets,
                L3_recommendedMoons,
                L4_userPreferences,
                topPerType: L4_Top_Personalized_Amount
            );

            // Assign the final score lists before rank fusion
            var L4_recommendedStars = L4_Results.RecommendedStars;
            var L4_recommendedPlanets = L4_Results.RecommendedPlanets;
            var L4_recommendedMoons = L4_Results.RecommendedMoons;


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

            // ═══════════════════════════════════════════════════════════════
            // LAYER 5: RANK FUSION (GENETIC ALGORITHM)
            // The final stage that takes the ranked lists from L1, L2, L3, and L4, 
            // finds the optimal weight combination (W1-W4) to create a consensus, and outputs the final rankings.
            // ═══════════════════════════════════════════════════════════════

            PrintSectionHeader("LAYER 5 GA RANK FUSION");

            // Initialize handler and run GA optimization
            var layer5Handler = new Layer5_Poppys_GA_Handler(seed: 42); // Seed ensures reproducible GA results

            // Run GA: The GA needs the ranks from ALL four previous layers to calculate the optimal fusion weights.
            var L5_Results = layer5Handler.RunOptimization(
                user: User,
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

            // ═══════════════════════════════════════════════════════════════
            // Extract optimized results
            // The results are now of type Layer5_Poppys_GA_Object, containing all previous ranks and the final L5 score/rank.
            // ═══════════════════════════════════════════════════════════════

            var L5_recommendedStars = L5_Results.Stars ?? new List<Layer5_Poppys_GA_Object>();
            var L5_recommendedPlanets = L5_Results.Planets ?? new List<Layer5_Poppys_GA_Object>();
            var L5_recommendedMoons = L5_Results.Moons ?? new List<Layer5_Poppys_GA_Object>();

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

            PrintSectionFooter("END OF LAYER 5 - FINAL RECOMMENDATIONS");*/
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
    public class PayloadWrapper
    {
        public User_Object User { get; set; }
        public PoolWrapper Pool { get; set; }
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