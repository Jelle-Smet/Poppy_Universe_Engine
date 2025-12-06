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
            // STEP 1: Initialize Celestial Objects Data
            // ═══════════════════════════════════════════════════════════════

            List<Star_Objects> Stars = new List<Star_Objects>
            {
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
            // ═══════════════════════════════════════════════════════════════

            double userLat = 51.016;
            double userLon = 4.24222;
            DateTime utcTime = DateTime.UtcNow;

            // Simulate user preferences for personalized recommendations
            var User = new User_Object
            {
                ID = 1,
                Name = "Raven",
                Latitude = userLat,
                Longitude = userLon,
                ObservationTime = utcTime,
                LikedStars = new List<string> { "Sirius", "Procyon", "Deneb" },
                LikedPlanets = new List<string> { "Sun", "Mars", "Venus", "Mercury" },
                LikedMoons = new List<string> { "Europa", "Moon" }
            };

            PrintSectionHeader("USER INFORMATION");

            Console.WriteLine($"┌─ User: {User.Name}");
            Console.WriteLine($"│  ID: {User.ID}");
            Console.WriteLine($"│  Date Time: {User.ObservationTime}");
            Console.WriteLine($"│  Position: Lat {User.Latitude:F2} | Long {User.Longitude:F2}");
            Console.WriteLine($"│  ┌─ Likes: ");
            Console.WriteLine($"│  │ Stars: [ {String.Join(" - ", User.LikedStars)} ]");
            Console.WriteLine($"│  │ Planets: [ {String.Join(" - ", User.LikedPlanets)} ]");
            Console.WriteLine($"│  │ Moons: [ {String.Join(" - ", User.LikedMoons)} ]");
            Console.WriteLine($"│  └─");
            Console.WriteLine($"└─");

            // ═══════════════════════════════════════════════════════════════
            // LAYER 1: PERSONALIZED RECOMMENDATIONS
            // ═══════════════════════════════════════════════════════════════

            PrintSectionHeader("LAYER 1 PERSONALIZED RESULTS");

            // Initialize recommendation engines
            var starEngine = new Layer1_Star_Engine(User);
            var planetEngine = new Layer1_Planet_Engine(User);
            var moonEngine = new Layer1_Moon_Engine(User);

            // Get personalized recommendations based on user preferences
            var L1_recommendedStars= starEngine.GetRecommendedStars(Stars, utcTime, User) ?? new List<Star_View>();
            var L1_recommendedPlanets = planetEngine.GetRecommendedPlanets(Planets, utcTime, User) ?? new List<Planet_View>();
            var L1_recommendedMoons = moonEngine.GetRecommendedMoons(Moons, L1_recommendedPlanets, utcTime) ?? new List<Moon_View>();

            // Display Layer 1 Results
            Console.WriteLine("🌟 ═══════════════ RECOMMENDED STARS ═══════════════ 🌟\n");
            foreach (var view in L1_recommendedStars)
            {
                Console.WriteLine($"┌─ Star: {view.Star.Name}");
                Console.WriteLine($"│  ID: {view.Source}");
                Console.WriteLine($"│  Type: {view.SpectralType}");
                Console.WriteLine($"│  Position: Alt {view.Altitude:F2}° | Az {view.Azimuth:F2}°");
                Console.WriteLine($"│  Brightness (Gmag): {view.Star.Gmag}");
                Console.WriteLine($"│  Visible: {view.IsVisible}");
                Console.WriteLine($"│  Match Score: {view.MatchPercentage:F2}% ({view.Score:F2})");
                Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                Console.WriteLine($"└─ {view.ChanceReason}\n");
            }
            Console.WriteLine($"   ✓ Total visible stars: {L1_recommendedStars.Count(s => s.IsVisible)}\n");

            Console.WriteLine("🪐 ═══════════════ RECOMMENDED PLANETS ═══════════════ 🪐\n");
            foreach (var view in L1_recommendedPlanets)
            {
                Console.WriteLine($"┌─ Planet: {view.Planet.Name}");
                Console.WriteLine($"│  ID: {view.Id}");
                Console.WriteLine($"│  Type: {view.Type}");
                Console.WriteLine($"│  Position: Alt {view.Altitude:F2}° | Az {view.Azimuth:F2}°");
                Console.WriteLine($"│  Properties: {view.Planet.Color} | Ø {view.Planet.Diameter:N0} km | {view.Planet.Mass} × 10²⁴kg");
                Console.WriteLine($"│  Visible: {view.IsVisible}");
                Console.WriteLine($"│  Match Score: {view.MatchPercentage:F2}% ({view.Score:F2})");
                Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                Console.WriteLine($"└─ {view.ChanceReason}\n");
            }
            Console.WriteLine($"   ✓ Total visible planets: {L1_recommendedPlanets.Count(p => p.IsVisible)}\n");

            Console.WriteLine("🌕 ═══════════════ RECOMMENDED MOONS ═══════════════ 🌕\n");
            foreach (var view in L1_recommendedMoons)
            {
                Console.WriteLine($"┌─ Moon: {view.Moon.Name} (orbits {view.Parent})");
                Console.WriteLine($"│  ID: {view.Id}");
                Console.WriteLine($"│  Position: Alt {view.Altitude:F2}° | Az {view.Azimuth:F2}°");
                Console.WriteLine($"│  Properties: Ø {view.Moon.Diameter:N0} km | {view.Moon.Mass} × 10²⁴kg");
                Console.WriteLine($"│  Composition: {view.Moon.Composition}");
                Console.WriteLine($"│  Features: {view.Moon.SurfaceFeatures}");
                Console.WriteLine($"│  Visible: {view.IsVisible}");
                Console.WriteLine($"│  Match Score: {view.MatchPercentage:F2}% ({view.Score:F2})");
                Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                Console.WriteLine($"└─ {view.ChanceReason}\n");
            }
            Console.WriteLine($"   ✓ Total visible moons: {L1_recommendedMoons.Count(m => m.IsVisible)}\n");

            PrintSectionFooter("END OF LAYER 1");

            // ═══════════════════════════════════════════════════════════════
            // LAYER 2: TRENDING & POPULARITY BOOST
            // ═══════════════════════════════════════════════════════════════

            PrintSectionHeader("LAYER 2 BOOSTING RESULTS");

            // Define interaction data (from analytics/backend)
            // This represents user engagement: views, clicks, favorites, trending scores
            List<Layer2_Interaction_Object> interactions = new List<Layer2_Interaction_Object>
            {
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

            // Apply popularity boost (max 25% of theoretical max score)
            int Top_Recommendations_Amount = 10; // Change this to control how many per type, or set to 0 for default 5
            var booster = new Layer_2_Poppys_Trend_Booster();
            var L2_Results = booster.BoostAll(
                L1_recommendedStars,
                L1_recommendedPlanets,
                L1_recommendedMoons,
                interactions,
                topPerType: Top_Recommendations_Amount
            );

            // Now you can access the new lists from L2_Results
            var L2_recommendedStars = L2_Results.RecommendedStars;
            var L2_recommendedPlanets = L2_Results.RecommendedPlanets;
            var L2_recommendedMoons = L2_Results.RecommendedMoons;


            // Display Layer 2 Results (Boosted)
            Console.WriteLine("🌟 ═══════════════ POPULAR STARS (BOOSTED) ═══════════════ 🌟\n");
            foreach (var view in L2_recommendedStars)
            {
                Console.WriteLine($"┌─ Star: {view.Star.Name}");
                Console.WriteLine($"│  ID: {view.Source}");
                Console.WriteLine($"│  Type: {view.SpectralType}");
                Console.WriteLine($"│  Position: Alt {view.Altitude:F2}° | Az {view.Azimuth:F2}°");
                Console.WriteLine($"│  Brightness (Gmag): {view.Star.Gmag}");
                Console.WriteLine($"│  Visible: {view.IsVisible}");
                Console.WriteLine($"│  Boosted Score: {view.MatchPercentage:F2}% ({view.Score:F2})");
                Console.WriteLine($"│    --> Boost Applied: {view.BoostDescription}%");
                Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                Console.WriteLine($"└─ {view.ChanceReason}\n");
            }
            Console.WriteLine($"   ✓ Total visible stars: {L2_recommendedStars.Count(s => s.IsVisible)}\n");

            Console.WriteLine("🪐 ═══════════════ POPULAR PLANETS (BOOSTED) ═══════════════ 🪐\n");
            foreach (var view in L2_recommendedPlanets)
            {
                Console.WriteLine($"┌─ Planet: {view.Planet.Name}");
                Console.WriteLine($"│  ID: {view.Id}");
                Console.WriteLine($"│  Type: {view.Type}");
                Console.WriteLine($"│  Position: Alt {view.Altitude:F2}° | Az {view.Azimuth:F2}°");
                Console.WriteLine($"│  Properties: {view.Planet.Color} | Ø {view.Planet.Diameter:N0} km | {view.Planet.Mass} × 10²⁴kg");
                Console.WriteLine($"│  Visible: {view.IsVisible}");
                Console.WriteLine($"│  Boosted Score: {view.MatchPercentage:F2}% ({view.Score:F2})");
                Console.WriteLine($"│    --> Boost Applied: {view.BoostDescription}%");
                Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                Console.WriteLine($"└─ {view.ChanceReason}\n");
            }
            Console.WriteLine($"   ✓ Total visible planets: {L2_recommendedPlanets.Count(p => p.IsVisible)}\n");

            Console.WriteLine("🌕 ═══════════════ POPULAR MOONS (BOOSTED) ═══════════════ 🌕\n");
            foreach (var view in L2_recommendedMoons)
            {
                Console.WriteLine($"┌─ Moon: {view.Moon.Name} (orbits {view.Parent})");
                Console.WriteLine($"│  ID: {view.Id}");
                Console.WriteLine($"│  Position: Alt {view.Altitude:F2}° | Az {view.Azimuth:F2}°");
                Console.WriteLine($"│  Properties: Ø {view.Moon.Diameter:N0} km | Mass {view.Moon.Mass} × 10²⁴kg");
                Console.WriteLine($"│  Composition: {view.Moon.Composition}");
                Console.WriteLine($"│  Features: {view.Moon.SurfaceFeatures}");
                Console.WriteLine($"│  Visible: {view.IsVisible}");
                Console.WriteLine($"│  Boosted Score: {view.MatchPercentage:F2}% ({view.Score:F2})");
                Console.WriteLine($"│    --> Boost Applied: {view.BoostDescription}%");
                Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                Console.WriteLine($"└─ {view.ChanceReason}\n");
            }
            Console.WriteLine($"   ✓ Total visible moons: {L2_recommendedMoons.Count(m => m.IsVisible)}\n");

            PrintSectionFooter("END OF LAYER 2");

            // ═══════════════════════════════════════════════════════════════
            // LAYER 3: PERSONALIZATION BOOST (MATRIX FACTORIZATION)
            // ═══════════════════════════════════════════════════════════════

            PrintSectionHeader("LAYER 3 BOOSTING RESULTS (PERSONALIZATION)");

            // Define user preference matrix (from matrix factorization)
            // Values should be between 0-10, representing learned preferences
            Layer3_User_Matrix_Object userPreferences = new Layer3_User_Matrix_Object
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

            // Apply personalization boost (max 25% of theoretical max score)
            int Top_Personalized_Amount = 10; // Change this to control how many per type
            var personalizer = new Layer_3_Poppys_Matrix_Booster();
            // Capture the results in a new Layer3_Boost_Result object
            var L3_Results = personalizer.BoostAll(
                L1_recommendedStars, // Note: If L2 was run, this should probably be L2_recommendedStars
                L1_recommendedPlanets, // If using L2, use L2_recommendedPlanets here
                L1_recommendedMoons, // If using L2, use L2_recommendedMoons here
                userPreferences,
                topPerType: Top_Personalized_Amount
            );

            // Now you can assign the new lists from L3_Results
            var L3_recommendedStars = L3_Results.RecommendedStars;
            var L3_recommendedPlanets = L3_Results.RecommendedPlanets;
            var L3_recommendedMoons = L3_Results.RecommendedMoons;


            // Display Layer 3 Results (Personalized)
            Console.WriteLine("🌟 ═══════════════ PERSONALIZED STARS (LAYER 3) ═══════════════ 🌟\n");
            foreach (var view in L3_recommendedStars)
            {
                Console.WriteLine($"┌─ Star: {view.Star.Name}");
                Console.WriteLine($"│  ID: {view.Source}");
                Console.WriteLine($"│  Type: {view.SpectralType}");
                Console.WriteLine($"│  Position: Alt {view.Altitude:F2}° | Az {view.Azimuth:F2}°");
                Console.WriteLine($"│  Brightness (Gmag): {view.Star.Gmag}");
                Console.WriteLine($"│  Visible: {view.IsVisible}");
                Console.WriteLine($"│  Personalized Score: {view.MatchPercentage:F2}% ({view.Score:F2})");
                Console.WriteLine($"│    --> {view.BoostDescription}");
                Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                Console.WriteLine($"└─ {view.ChanceReason}\n");
            }
            Console.WriteLine($"   ✓ Total visible stars: {L3_recommendedStars.Count(s => s.IsVisible)}\n");

            Console.WriteLine("🪐 ═══════════════ PERSONALIZED PLANETS (LAYER 3) ═══════════════ 🪐\n");
            foreach (var view in L3_recommendedPlanets)
            {
                Console.WriteLine($"┌─ Planet: {view.Planet.Name}");
                Console.WriteLine($"│  ID: {view.Id}");
                Console.WriteLine($"│  Type: {view.Type}");
                Console.WriteLine($"│  Position: Alt {view.Altitude:F2}° | Az {view.Azimuth:F2}°");
                Console.WriteLine($"│  Properties: {view.Planet.Color} | Ø {view.Planet.Diameter:N0} km | {view.Planet.Mass} × 10²⁴kg");
                Console.WriteLine($"│  Visible: {view.IsVisible}");
                Console.WriteLine($"│  Personalized Score: {view.MatchPercentage:F2}% ({view.Score:F2})");
                Console.WriteLine($"│    --> {view.BoostDescription}");
                Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                Console.WriteLine($"└─ {view.ChanceReason}\n");
            }
            Console.WriteLine($"   ✓ Total visible planets: {L3_recommendedPlanets.Count(p => p.IsVisible)}\n");

            Console.WriteLine("🌕 ═══════════════ PERSONALIZED MOONS (LAYER 3) ═══════════════ 🌕\n");
            foreach (var view in L3_recommendedMoons)
            {
                Console.WriteLine($"┌─ Moon: {view.Moon.Name} (orbits {view.Parent})");
                Console.WriteLine($"│  ID: {view.Id}");
                Console.WriteLine($"│  Position: Alt {view.Altitude:F2}° | Az {view.Azimuth:F2}°");
                Console.WriteLine($"│  Properties: Ø {view.Moon.Diameter:N0} km | Mass {view.Moon.Mass} × 10²⁴kg");
                Console.WriteLine($"│  Composition: {view.Moon.Composition}");
                Console.WriteLine($"│  Features: {view.Moon.SurfaceFeatures}");
                Console.WriteLine($"│  Visible: {view.IsVisible}");
                Console.WriteLine($"│  Personalized Score: {view.MatchPercentage:F2}% ({view.Score:F2})");
                Console.WriteLine($"│    --> {view.BoostDescription}");
                Console.WriteLine($"│  Weather Visibility: {view.VisibilityChance}%");
                Console.WriteLine($"└─ {view.ChanceReason}\n");
            }
            Console.WriteLine($"   ✓ Total visible moons: {L3_recommendedMoons.Count(m => m.IsVisible)}\n");

            PrintSectionFooter("END OF LAYER 3");
        }

        // ═══════════════════════════════════════════════════════════════
        // Helper Methods for Pretty Console Output
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Prints a nicely formatted section header
        /// </summary>
        private static void PrintSectionHeader(string title)
        {
            string border = new string('═', 70);
            string spacedTitle = $"█  {title}  █";
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