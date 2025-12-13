using System;
using System.Collections.Generic;
using System.Linq;

namespace Poppy_Universe_Engine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // ═══════════════════════════════════════════════════════════════
            // STEP 1: Initialize Celestial Objects Data (Simulated Database)
            // This data feeds into the Layer 1 visibility and scoring engines.
            // ═══════════════════════════════════════════════════════════════

            List<Star_Objects> Stars = new List<Star_Objects>
            {
                // Star_Objects: RA_ICRS and DE_ICRS define the fixed position in the sky.
                // Gmag (magnitude) and SpectralType are key inputs for Layer 1 scoring.
                new Star_Objects { Id = 1, Name = "Sirius", Source = 1, RA_ICRS = 6.752, DE_ICRS = -16.716, Gmag = -1.46, SpectralType = "A" },
                new Star_Objects { Id = 2, Name = "Betelgeuse", Source = 2, RA_ICRS = 5.9195, DE_ICRS = 7.4071, Gmag = 0.42, SpectralType = "M" },
                new Star_Objects { Id = 3, Name = "Rigel", Source = 3, RA_ICRS = 5.242, DE_ICRS = -8.201, Gmag = 0.18, SpectralType = "B" },
                new Star_Objects { Id = 4, Name = "Procyon", Source = 4, RA_ICRS = 7.655, DE_ICRS = 5.225, Gmag = 0.38, SpectralType = "F" },
                new Star_Objects { Id = 5, Name = "Achernar", Source = 5, RA_ICRS = 1.6286, DE_ICRS = -57.2368, Gmag = 0.46, SpectralType = "B" },
                new Star_Objects { Id = 6, Name = "Altair", Source = 6, RA_ICRS = 19.8464, DE_ICRS = 8.8683, Gmag = 0.77, SpectralType = "A" },
                new Star_Objects { Id = 7, Name = "Aldebaran", Source = 7, RA_ICRS = 4.5987, DE_ICRS = 16.5093, Gmag = 0.87, SpectralType = "K" },
                new Star_Objects { Id = 8, Name = "Spica", Source = 8, RA_ICRS = 13.4199, DE_ICRS = -11.1614, Gmag = 0.97, SpectralType = "B" },
                new Star_Objects { Id = 9, Name = "Antares", Source = 9, RA_ICRS = 16.4901, DE_ICRS = -26.4319, Gmag = 1.06, SpectralType = "M" },
                new Star_Objects { Id = 10, Name = "Pollux", Source = 10, RA_ICRS = 7.7553, DE_ICRS = 28.0262, Gmag = 1.14, SpectralType = "K" },
                new Star_Objects { Id = 11, Name = "Fomalhaut", Source = 11, RA_ICRS = 22.9608, DE_ICRS = -29.6222, Gmag = 1.16, SpectralType = "A" },
                new Star_Objects { Id = 12, Name = "Deneb", Source = 12, RA_ICRS = 20.6905, DE_ICRS = 45.2803, Gmag = 1.25, SpectralType = "A" },
                new Star_Objects { Id = 13, Name = "Regulus", Source = 13, RA_ICRS = 10.1395, DE_ICRS = 11.9672, Gmag = 1.35, SpectralType = "B" },
                new Star_Objects { Id = 14, Name = "Castor", Source = 14, RA_ICRS = 7.5767, DE_ICRS = 31.8883, Gmag = 1.58, SpectralType = "A" },
                new Star_Objects { Id = 15, Name = "Adhara", Source = 15, RA_ICRS = 6.9771, DE_ICRS = -28.9721, Gmag = 1.50, SpectralType = "B" },
                new Star_Objects { Id = 16, Name = "Shaula", Source = 16, RA_ICRS = 17.5601, DE_ICRS = -37.1038, Gmag = 1.62, SpectralType = "B" },
                new Star_Objects { Id = 17, Name = "Bellatrix", Source = 17, RA_ICRS = 5.4189, DE_ICRS = 6.3497, Gmag = 1.64, SpectralType = "B" },
                new Star_Objects { Id = 18, Name = "Elnath", Source = 18, RA_ICRS = 5.4382, DE_ICRS = 28.6075, Gmag = 1.65, SpectralType = "B" },
                new Star_Objects { Id = 19, Name = "Miaplacidus", Source = 19, RA_ICRS = 9.2190, DE_ICRS = -69.7172, Gmag = 1.67, SpectralType = "A" },
                new Star_Objects { Id = 20, Name = "Alnilam", Source = 20, RA_ICRS = 5.6036, DE_ICRS = -1.2019, Gmag = 1.69, SpectralType = "B" },

            };

            List<Planet_Objects> Planets = new List<Planet_Objects>
            {
                // Planet_Objects: Contains physical properties and orbital elements (like SemiMajorAxisAU)
                // The "Type" field is important for Layer 3/4 boosting.
                new Planet_Objects { Id = 1, Name = "Sun", Magnitude = 4.83, Color="Yellow-White", DistanceFromSun=0, DistanceFromEarth=149.6, Diameter=1391016, Mass=1989000, OrbitalPeriod=0, OrbitalInclination=0, SemiMajorAxisAU=0, LongitudeAscendingNode=0, ArgumentPeriapsis=0, MeanAnomaly=0, MeanTemperature=5505, NumberOfMoons=0, HasRings=false, HasMagneticField=true, Type="Star" },
                new Planet_Objects { Id = 2, Name = "Mercury", Magnitude = 5.73, Color="Gray", DistanceFromSun=57.9, DistanceFromEarth=91.7, Diameter=4879, Mass=0.330, OrbitalPeriod=88, OrbitalInclination=7.0, SemiMajorAxisAU=0.387, LongitudeAscendingNode=48.331, ArgumentPeriapsis=29.124, MeanAnomaly=174.796, MeanTemperature=167, NumberOfMoons=0, HasRings=false, HasMagneticField=true, Type="Terrestrial" },
                new Planet_Objects { Id = 3, Name = "Venus", Magnitude = 4.38, Color="Yellow", DistanceFromSun=108.2, DistanceFromEarth=41.4, Diameter=12104, Mass=4.87, OrbitalPeriod=225, OrbitalInclination=3.39, SemiMajorAxisAU=0.723, LongitudeAscendingNode=76.680, ArgumentPeriapsis=54.884, MeanAnomaly=50.115, MeanTemperature=464, NumberOfMoons=0, HasRings=false, HasMagneticField=false, Type="Terrestrial" },
                new Planet_Objects { Id = 4, Name = "Earth", Magnitude = 4.83, Color="Blue", DistanceFromSun=149.6, DistanceFromEarth=83.0, Diameter=12742, Mass=5.97, OrbitalPeriod=365, OrbitalInclination=0.0, SemiMajorAxisAU=1.0, LongitudeAscendingNode=0, ArgumentPeriapsis=102.947, MeanAnomaly=100.464, MeanTemperature=15, NumberOfMoons=1, HasRings=false, HasMagneticField=true, Type="Terrestrial" },
                new Planet_Objects { Id = 5, Name = "Mars", Magnitude = 6.40, Color="Red", DistanceFromSun=227.9, DistanceFromEarth=78.3, Diameter=6792, Mass=0.642, OrbitalPeriod=687, OrbitalInclination=1.85, SemiMajorAxisAU=1.524, LongitudeAscendingNode=49.558, ArgumentPeriapsis=286.502, MeanAnomaly=19.41, MeanTemperature=-60, NumberOfMoons=2, HasRings=false, HasMagneticField=false, Type="Terrestrial" },
                new Planet_Objects { Id = 6, Name = "Jupiter", Magnitude = 2.70, Color="Orange", DistanceFromSun=778.5, DistanceFromEarth=628.7, Diameter=139820, Mass=1898, OrbitalPeriod=4333, OrbitalInclination=1.31, SemiMajorAxisAU=5.203, LongitudeAscendingNode=100.464, ArgumentPeriapsis=273.867, MeanAnomaly=20.020, MeanTemperature=-110, NumberOfMoons=79, HasRings=true, HasMagneticField=true, Type="Gas Giant" },
                new Planet_Objects { Id = 7, Name = "Saturn", Magnitude = 1.47, Color="Yellow", DistanceFromSun=1427, DistanceFromEarth=1275, Diameter=120536, Mass=568, OrbitalPeriod=10759, OrbitalInclination=2.49, SemiMajorAxisAU=9.537, LongitudeAscendingNode=113.665, ArgumentPeriapsis=339.392, MeanAnomaly=317.020, MeanTemperature=-140, NumberOfMoons=82, HasRings=true, HasMagneticField=true, Type="Gas Giant" },
                new Planet_Objects { Id = 8, Name = "Uranus", Magnitude = 5.52, Color="LightBlue", DistanceFromSun=2871, DistanceFromEarth=2721, Diameter=51118, Mass=86.8, OrbitalPeriod=30687, OrbitalInclination=0.77, SemiMajorAxisAU=19.191, LongitudeAscendingNode=74.006, ArgumentPeriapsis=96.998, MeanAnomaly=142.238, MeanTemperature=-195, NumberOfMoons=27, HasRings=true, HasMagneticField=true, Type="Ice Giant" },
                new Planet_Objects { Id = 9, Name = "Neptune", Magnitude = 7.05, Color="Blue", DistanceFromSun=4495, DistanceFromEarth=4351, Diameter=49528, Mass=102, OrbitalPeriod=60190, OrbitalInclination=1.77, SemiMajorAxisAU=30.07, LongitudeAscendingNode=131.784, ArgumentPeriapsis=272.846, MeanAnomaly=256.228, MeanTemperature=-200, NumberOfMoons=14, HasRings=true, HasMagneticField=true, Type="Ice Giant" },

            };

            List<Moon_Objects> Moons = new List<Moon_Objects>
            {
                // Moon_Objects: Contains orbital elements relative to the parent planet (SemiMajorAxisKm, Inclination).
                // Features and Composition are used in Layer 1 scoring.
                // Earth's Moon
                new Moon_Objects { Id = 1, Name="Moon", Parent="Earth", Color="Gray", Diameter=3475, Mass=0.073, OrbitalPeriod=27.3, SemiMajorAxisKm=384400, Inclination=5.145, SurfaceTemperature=-53, Composition="Rock/Ice", SurfaceFeatures="Craters", DistanceFromEarth=0 },

                // Mars Moons
                new Moon_Objects { Id = 2, Name="Phobos", Parent="Mars", Color="Gray", Diameter=22.4, Mass=1.065e-8, OrbitalPeriod=0.319, SemiMajorAxisKm=9376, Inclination=1.093, SurfaceTemperature=-40, Composition="Rock", SurfaceFeatures="Craters", DistanceFromEarth=78.4 },
                new Moon_Objects { Id = 3, Name="Deimos", Parent="Mars", Color="Gray", Diameter=12.4, Mass=1.476e-9, OrbitalPeriod=1.263, SemiMajorAxisKm=23460, Inclination=0.93, SurfaceTemperature=-40, Composition="Rock", SurfaceFeatures="Craters", DistanceFromEarth=78.4 },

                // Jupiter Moons
                new Moon_Objects { Id = 4, Name="Io", Parent="Jupiter", Color="Yellow", Diameter=3643, Mass=0.089, OrbitalPeriod=1.769, SemiMajorAxisKm=421700, Inclination=0.04, SurfaceTemperature=-143, Composition="Rock/Ice", SurfaceFeatures="Volcanoes", DistanceFromEarth=628.5 },
                new Moon_Objects { Id = 5, Name="Europa", Parent="Jupiter", Color="Gray", Diameter=3122, Mass=0.008, OrbitalPeriod=3.551, SemiMajorAxisKm=671000, Inclination=0.47, SurfaceTemperature=-160, Composition="Ice/Rock", SurfaceFeatures="Cracks", DistanceFromEarth=628.5 },
                new Moon_Objects { Id = 6, Name="Ganymede", Parent="Jupiter", Color="Gray", Diameter=5268, Mass=0.148, OrbitalPeriod=7.154, SemiMajorAxisKm=1070400, Inclination=0.20, SurfaceTemperature=-160, Composition="Ice/Rock", SurfaceFeatures="Cratered terrain", DistanceFromEarth=628.5 },
                new Moon_Objects { Id = 7, Name="Callisto", Parent="Jupiter", Color="Gray", Diameter=4821, Mass=0.107, OrbitalPeriod=16.689, SemiMajorAxisKm=1882700, Inclination=0.19, SurfaceTemperature=-139, Composition="Ice/Rock", SurfaceFeatures="Craters", DistanceFromEarth=628.5 },

                // Saturn Moons
                new Moon_Objects { Id = 8, Name="Titan", Parent="Saturn", Color="Orange", Diameter=5150, Mass=0.1345, OrbitalPeriod=15.945, SemiMajorAxisKm=1221870, Inclination=0.33, SurfaceTemperature=-179, Composition="Nitrogen/Methane", SurfaceFeatures="Lakes, dunes", DistanceFromEarth=1272 },
                new Moon_Objects { Id = 9, Name="Enceladus", Parent="Saturn", Color="White", Diameter=504, Mass=1.08e-4, OrbitalPeriod=1.370, SemiMajorAxisKm=238000, Inclination=0.0, SurfaceTemperature=-198, Composition="Ice/Rock", SurfaceFeatures="Geysers", DistanceFromEarth=1272 },

                // Uranus Moons
                new Moon_Objects { Id = 10, Name="Miranda", Parent="Uranus", Color="Gray", Diameter=471, Mass=6.59e-5, OrbitalPeriod=1.413, SemiMajorAxisKm=129900, Inclination=4.338, SurfaceTemperature=-201, Composition="Ice/Rock", SurfaceFeatures="Cliffs, craters", DistanceFromEarth=2718 },
                new Moon_Objects { Id = 11, Name="Titania", Parent="Uranus", Color="Gray", Diameter=1580, Mass=0.035, OrbitalPeriod=8.706, SemiMajorAxisKm=436300, Inclination=0.079, SurfaceTemperature=-202, Composition="Ice/Rock", SurfaceFeatures="Craters", DistanceFromEarth=2718 },

                // Neptune Moons
                new Moon_Objects { Id = 12, Name="Triton", Parent="Neptune", Color="LightBlue", Diameter=2706, Mass=0.022, OrbitalPeriod=-5.877, SemiMajorAxisKm=354800, Inclination=156.865, SurfaceTemperature=-235, Composition="Ice/Rock", SurfaceFeatures="Craters, geysers", DistanceFromEarth=4300 }
            };

            // ═══════════════════════════════════════════════════════════════
            // STEP 2: Configure User Location & Preferences
            // This simulates the observer and their personal data/preferences.
            // ═══════════════════════════════════════════════════════════════

            double userLat = 51.016; // Latitude for the observer (Londerzeel, Belgium)
            double userLon = 4.24222; // Longitude for the observer
            DateTime utcTime = DateTime.UtcNow; // Current time in UTC (used for ephemeris calculation)

            // Simulate user preferences for personalized recommendations
            var User = new User_Object
            {
                ID = 1,
                Name = "Raven",
                Latitude = userLat,
                Longitude = userLon,
                ObservationTime = utcTime,
                // These preferences directly influence Layer 1 scoring weights
                LikedStars = new List<string> { "Sirius", "Procyon", "Deneb" },
                LikedPlanets = new List<string> { "Sun", "Mars", "Venus", "Mercury" },
                LikedMoons = new List<string> { "Europa", "Moon" }
            };

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
            // These lists are sorted by the Layer 1 score (L1_recommendedStars[0] has the best L1 score).
            var L1_recommendedStars = starEngine.GetRecommendedStars(Stars, utcTime, User) ?? new List<Star_View>();
            var L1_recommendedPlanets = planetEngine.GetRecommendedPlanets(Planets, utcTime, User) ?? new List<Planet_View>();
            // Moon positions depend on the parent planet's calculated position, hence the dependency on L1_recommendedPlanets.
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

            PrintSectionFooter("END OF LAYER 5 - FINAL RECOMMENDATIONS");
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
}