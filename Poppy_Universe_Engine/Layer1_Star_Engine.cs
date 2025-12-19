using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Added for async/await calls, though GetAwaiter().GetResult() is used

namespace Poppy_Universe_Engine
{
    /// <summary>
    /// Engine responsible for calculating the visibility of stars and generating 
    /// a ranked list of recommendations for the observer.
    /// </summary>
    internal class Layer1_Star_Engine
    {
        // Dependencies
        private Visibility_Service visibilityService;
        private double minAltitude; // Minimum altitude (in degrees) for a star to be considered visible
        private VisibilityCalculator visibilityCalc;
        private User_Object user;

        /// <summary>
        /// Initializes the Star Engine with user data and minimum visibility altitude.
        /// </summary>
        /// <param name="user">The observer's location and preferences.</param>
        /// <param name="minAlt">The minimum altitude (degrees) above the horizon for visibility (default 7.5).</param>
        public Layer1_Star_Engine(User_Object user, double minAlt = 7.5)
        {
            // Initialize services using user's location
            visibilityService = new Visibility_Service(user.Latitude, user.Longitude);
            minAltitude = minAlt;
            visibilityCalc = new VisibilityCalculator(user);
            this.user = user;
        }

        /// <summary>
        /// Computes the Altitude and Azimuth (Alt/Az), and basic visibility for all stars
        /// at a given UTC time.
        /// </summary>
        /// <param name="stars">List of all known Star_Objects (containing RA/Dec coordinates).</param>
        /// <param name="utcTime">The current observation time in UTC.</param>
        /// <returns>A list of Star_View objects containing computed position and visibility data.</returns>
        public List<Star_View> GetStarViews(List<Star_Objects> stars, DateTime utcTime)
        {
            var starViews = new List<Star_View>();

            // Stars are "fixed" in Equatorial coordinates (RA/Dec), so we iterate and calculate 
            // the local horizon coordinates (Alt/Az) for each.
            foreach (var star in stars)
            {
                // Calculate Altitude and Azimuth (Alt/Az) for the observer's location
                var (alt, az) = visibilityService.CalculateAltAz(star, utcTime);
                // Determine basic visibility based on minimum altitude threshold
                bool visible = alt >= minAltitude;

                // Create and populate the Star_View object
                starViews.Add(new Star_View
                {
                    Star = star,
                    Id = star.Id,
                    Source = star.Source,
                    Altitude = alt,
                    Azimuth = az,
                    IsVisible = visible,
                    Score = 0, // Initialized for later scoring
                    MatchPercentage = 0, // Initialized for later scoring
                    VisibilityChance = 0,
                    SpectralType = star.SpectralType,
                    ChanceReason = ""
                });
            }

            return starViews;
        }

        /// <summary>
        /// Filters for visible stars, applies a preference-based scoring system,
        /// incorporates weather data, and returns a ranked list of recommended stars.
        /// </summary>
        /// <param name="stars">List of all known Star_Objects.</param>
        /// <param name="utcTime">The current observation time in UTC.</param>
        /// <param name="user">The observer's user preferences.</param>
        /// <returns>A ranked list of visible Star_View objects.</returns>
        public List<Star_View> GetRecommendedStars(List<Star_Objects> stars, DateTime utcTime, User_Object user)
        {
            // 1. Calculate positions and filter for visibility
            var starViews = GetStarViews(stars, utcTime);
            var visibleStars = starViews.Where(s => s.IsVisible).ToList();

            // 2. Fetch and calculate weather visibility chance (applies globally)
            // Note: This uses GetAwaiter().GetResult() to synchronously call an async method (blocking call).
            var weatherData = visibilityCalc.FetchWeatherAsync().GetAwaiter().GetResult();
            var (weatherChance, weatherReason) = visibilityCalc.ComputeWeatherChanceWithReason(weatherData);

            Random random = new Random();

            // 4. Calculate individual scores for each visible star
            foreach (var s in visibleStars)
            {
                double score = 0;

                // 1️⃣ Liked stars
                double likedScore = user.LikedStars.Contains(s.Star.Name) ? 5 : 3;

                // 2️⃣ Brightness (main driver)
                double brightnessScore = Math.Pow(
                    Math.Max(0, 5 - (s.Star.Gmag ?? 6.0)),
                    1.5
                );

                // 3️⃣ Color appeal (BP-RP)
                double colorScore = 0;
                if (s.Star.ColorIndexBP_RP.HasValue)
                {
                    double color = s.Star.ColorIndexBP_RP.Value;
                    colorScore = Math.Max(0, 2.0 - Math.Abs(color - 1.2));
                }

                // 4️⃣ Distance clarity
                double distanceScore = 0;
                if (s.Star.DistancePc.HasValue)
                {
                    distanceScore = Math.Max(0, 3.0 - Math.Log10(s.Star.DistancePc.Value + 1));
                }

                // 5️⃣ Temperature preference
                double tempScore = 0;
                if (s.Star.Teff.HasValue)
                {
                    double teff = s.Star.Teff.Value;
                    tempScore = Math.Max(0, 2.5 - Math.Abs(teff - 5500) / 3000);
                }

                // 6️⃣ Physical presence
                double physicalScore = 0;
                if (s.Star.Luminosity.HasValue)
                    physicalScore += Math.Log10(s.Star.Luminosity.Value + 1);

                if (s.Star.Mass.HasValue)
                    physicalScore += Math.Log10(s.Star.Mass.Value + 1);

                // 7️⃣ Tiny chaos
                double randomNudge = random.NextDouble() * 0.5;

                // 🔥 Weighted final score (slightly boosted for better separation)
                score =
                    0.4 * likedScore +
                    0.9 * brightnessScore +   // bigger weight
                    0.4 * colorScore +        // slightly bigger
                    0.45 * distanceScore +    // slightly bigger
                    0.2 * tempScore +
                    0.25 * physicalScore +    // slightly bigger
                    randomNudge;

                // Optional: stretch scores non-linearly for more separation
                score = Math.Pow(score, 1.1);

                s.Score = Math.Round(score, 2);
            }

            // 🔥 Normalize with headroom (prevents top star hitting 100% too early)
            double bestScore = visibleStars.Max(s => s.Score);
            double headroomFactor = 1.1; // 10% headroom above the current top star

            foreach (var s in visibleStars)
            {
                // Scale relative to best star + headroom
                double normalized = (s.Score / (bestScore * headroomFactor)) * 100;

                // Clamp 0–100%
                s.MatchPercentage = Math.Round(Math.Min(100, normalized), 2);

                // Apply weather info
                s.VisibilityChance = Math.Round(weatherChance, 2);
                s.ChanceReason = weatherReason;
            }

            // 5. Return the final list, sorted by the calculated score (highest score first)
            return visibleStars.OrderByDescending(s => s.Score).ToList();
        }
    }
}