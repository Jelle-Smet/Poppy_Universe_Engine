using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks; // Added for async/await calls, though GetAwaiter().GetResult() is used

namespace Poppy_Universe_Engine
{
    /// <summary>
    /// Engine responsible for calculating the visibility of planets and generating 
    /// a ranked list of recommendations for the observer.
    /// </summary>
    internal class Layer1_Planet_Engine
    {
        // Dependencies
        private Visibility_Service visibilityService;
        private double minAltitude; // Minimum altitude (in degrees) for a planet to be considered visible
        private VisibilityCalculator visibilityCalc;
        private User_Object user;

        /// <summary>
        /// Initializes the Planet Engine with user data and minimum visibility altitude.
        /// </summary>
        /// <param name="user">The observer's location and preferences.</param>
        /// <param name="minAlt">The minimum altitude (degrees) above the horizon for visibility (default 7.5).</param>
        public Layer1_Planet_Engine(User_Object user, double minAlt = 7.5)
        {
            // Initialize services using user's location
            visibilityService = new Visibility_Service(user.Latitude, user.Longitude);
            minAltitude = minAlt;
            visibilityCalc = new VisibilityCalculator(user);
            this.user = user;
        }

        /// <summary>
        /// Calculates the geocentric position (RA/Dec, Alt/Az) and basic visibility for all planets
        /// at a given time.
        /// </summary>
        /// <param name="planets">List of all known Planet_Objects.</param>
        /// <param name="utcTime">The current observation time in UTC.</param>
        /// <returns>A list of Planet_View objects containing computed position and visibility data.</returns>
        public List<Planet_View> GetPlanetViews(List<Planet_Objects> planets, DateTime utcTime)
        {
            var planetViews = new List<Planet_View>();

            // Get Earth's position first (Heliocentric: relative to the Sun) 
            // This is required to calculate geocentric positions later.
            var earthPos = Planetary_Position_Service.CalculateHeliocentricPosition("earth", utcTime);

            foreach (var planet in planets)
            {
                try
                {
                    // 1. Calculate the planet's Heliocentric position (Sun-centered)
                    var planetPos = Planetary_Position_Service.CalculateHeliocentricPosition(planet.Name, utcTime);

                    // 2. Convert to Geocentric (Earth-centered) by subtracting Earth's position.
                    // The coordinates (X, Y, Z) are typically in Astronomical Units (AU).
                    double geocentricX = planetPos.X - earthPos.X;
                    double geocentricY = planetPos.Y - earthPos.Y;
                    double geocentricZ = planetPos.Z - earthPos.Z;

                    // 3. Convert Geocentric coordinates to Equatorial coordinates (RA/Dec).
                    // This is the common method to determine direction in the sky.
                    var (ra, dec) = Planetary_Position_Service.ConvertToEquatorial(geocentricX, geocentricY, geocentricZ);
                    // 

                    // 4. Calculate Altitude and Azimuth (Alt/Az) from RA/Dec for the observer's location.
                    // Note: RA_ICRS is converted from degrees to hours (RA / 15.0) for the service.
                    var (alt, az) = visibilityService.CalculateAltAz(
                        new Star_Objects { RA_ICRS = ra / 15.0, DE_ICRS = dec }, utcTime);

                    // 5. Determine basic visibility based on minimum altitude.
                    bool visible = alt >= minAltitude;

                    // 6. Create and populate the Planet_View object
                    planetViews.Add(new Planet_View
                    {
                        Planet = planet,
                        Id = planet.Id,
                        Type = planet.Type,
                        Altitude = alt,
                        Azimuth = az,
                        IsVisible = visible,
                        Score = 0, // Initialized for later scoring
                        MatchPercentage = 0, // Initialized for later scoring
                        GeocentricX = geocentricX,
                        GeocentricY = geocentricY,
                        GeocentricZ = geocentricZ,
                        RightAscension = ra,
                        Declination = dec,
                        VisibilityChance = 0,
                        ChanceReason = ""
                    });
                }
                catch (ArgumentException)
                {
                    // Catch exception if the planet's name is not supported by the position service (e.g., Pluto).
                    continue; // Skip the unsupported planet
                }
            }

            return planetViews;
        }

        /// <summary>
        /// Filters for visible planets, applies a preference-based scoring system,
        /// incorporates weather data, and returns a ranked list of recommended planets.
        /// </summary>
        /// <param name="planets">List of all known Planet_Objects.</param>
        /// <param name="utcTime">The current observation time in UTC.</param>
        /// <param name="user">The observer's user preferences.</param>
        /// <returns>A ranked list of visible Planet_View objects.</returns>
        public List<Planet_View> GetRecommendedPlanets(List<Planet_Objects> planets, DateTime utcTime, User_Object user)
        {
            // 1. Calculate positions and filter for visibility
            var planetViews = GetPlanetViews(planets, utcTime);
            var visiblePlanets = planetViews.Where(p => p.IsVisible).ToList();

            // 2. Fetch and calculate weather visibility chance (applies globally to all objects)
            // Note: This uses GetAwaiter().GetResult() to synchronously call an async method.
            var weatherData = visibilityCalc.FetchWeatherAsync().GetAwaiter().GetResult();
            var (weatherChance, weatherReason) = visibilityCalc.ComputeWeatherChanceWithReason(weatherData);

            // 3. Define the theoretical maximum score for normalization (based on weights below)
            double maxScore =
                                 1.5 // liked planets: 0.5 * Max 3
                               + 0.4 // rings: 0.2 * Max 2
                               + 0.9 // comfortable temperature: 0.3 * Max 3
                               + 0.4 // distance contribution: 0.2 * Max 2
                               + 5.0 // magnitude + distance: 1 * Max 5 (capped)
                               + 2.0; // synergy bonus: Max 2
                                      // Total theoretical maximum score ≈ 10.2


            Random random = new Random();

            // 4. Calculate individual scores for each visible planet
            foreach (var p in visiblePlanets)
            {
                double score = 0;

                // --- Scoring Components (Raw Points) ---

                // Liked planets: 3 points if the planet is in the user's liked list, 0 otherwise.
                double likedScore = user.LikedPlanets.Contains(p.Planet.Name) ? 3 : 0;

                // Rings: 2 points if the planet has rings.
                double ringsScore = p.Planet.HasRings ? 2 : 0;

                // Comfortable temperature: 3 points if mean temperature is between -50°C and 60°C.
                double tempScore = (p.Planet.MeanTemperature > -50 && p.Planet.MeanTemperature < 60) ? 3 : 0;

                // Distance from Sun contribution (Closer to 0 means closer to Sun)
                // This formula favors planets closer to the sun, but is likely based on internal logic.
                double distanceScore = (1 / (p.Planet.DistanceFromSun + 1)) * 2;

                // Magnitude + distance, scaled 0-5, non-linear
                // The raw calculation involves Absolute Magnitude (H) estimation (5 - M + 5 * log10(r * delta))
                // where M is the apparent magnitude, r is distance to Sun, and delta is distance to Earth.
                double sunAU = Math.Max(p.DistanceFromSun / 149.6, 1e-6); // Distance from Sun in AU (149.6M km/AU)
                double earthAU = Math.Max(p.DistanceFromEarth / 149.6, 1e-6); // Distance from Earth in AU

                // magScoreRaw approximates a measure of "potential brightness/easiness to see"
                double magScoreRaw = 5 - (p.Magnitude + 5 * Math.Log10(sunAU * earthAU));

                // Final magnitude score: non-linear scaling of the raw score, capped at 5.
                double magScore = Math.Pow(Math.Max(0, Math.Min(magScoreRaw, 5)), 1.2);

                // Synergy bonus: 2 extra points if the user likes the planet AND it has a comfortable temperature.
                double synergyBonus = (likedScore > 0 && tempScore > 0) ? 2 : 0;

                // Tiny random nudge: A small random value (0-0.5) to break ties.
                double randomNudge = random.NextDouble() * 0.5;

                // --- Final Weighted Sum ---
                // Weights prioritize magnitude/visibility (1.0), then user preferences (0.5), 
                // then objective features (0.2, 0.3) and synergy.
                score = 0.5 * likedScore +
                        0.2 * ringsScore +
                        0.3 * tempScore +
                        0.2 * distanceScore +
                        1 * magScore +
                        synergyBonus +
                        randomNudge;

                // Set final scores and match percentage (normalized against the max score)
                p.Score = Math.Round(score, 2);
                p.MatchPercentage = Math.Round(Math.Min(100, (score / maxScore) * 100), 2);

                // Apply the weather info
                p.VisibilityChance = Math.Round(weatherChance, 2);
                p.ChanceReason = weatherReason;
            }

            // 5. Return the final list, sorted by the calculated score (highest score first)
            return visiblePlanets.OrderByDescending(p => p.Score).ToList();
        }

    }
}