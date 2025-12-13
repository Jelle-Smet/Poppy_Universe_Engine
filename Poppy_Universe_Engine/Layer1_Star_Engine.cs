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

            // 3. Define the theoretical maximum score for normalization
            double maxScore =
                                 2.5    // Liked stars: 0.5 * Max 5
                               + 2.0    // Spectral preference: 0.4 * Max 5
                               + 3.75   // Brightness (weighted 0.75 * Max 5)
                               + 2.0;   // Synergy bonus: Max 2
                                        // Total theoretical maximum score ≈ 10.25

            Random random = new Random();

            // 4. Calculate individual scores for each visible star
            foreach (var s in visibleStars)
            {
                double score = 0;

                // --- Scoring Components (Raw Points) ---

                // Liked stars: 5 points if the star is in the user's liked list.
                double likedScore = user.LikedStars.Contains(s.Star.Name) ? 5 : 0;

                // Spectral preference: 5 points if the star's spectral type matches a user favorite.
                double spectralScore = (user.FavoriteSpectralTypes != null && user.FavoriteSpectralTypes.Contains(s.Star.SpectralType)) ? 5 : 0;
                // 

                // Brightness (non-linear, brighter = more appealing)
                // Score is based on the G-band magnitude (Gmag). Lower magnitude = brighter star = higher score.
                // The formula favors brighter stars heavily: Math.Pow(Max(0, 5 - Gmag), 1.5)
                double brightnessScore = Math.Pow(Math.Max(0, 5 - s.Star.Gmag), 1.5);

                // Synergy bonus: 2 extra points if the user likes the star AND it matches a preferred spectral type.
                double synergyBonus = (likedScore > 0 && spectralScore > 0) ? 2 : 0;

                // Tiny random nudge: A small random value (0-0.5) to help break ties.
                double randomNudge = random.NextDouble() * 0.5;

                // --- Final Weighted Sum ---
                // Weights prioritize brightness (0.75) and synergy (1.0), then user preferences (0.5, 0.4).
                score = 0.5 * likedScore +
                        0.4 * spectralScore +
                        0.75 * brightnessScore +
                        synergyBonus +
                        randomNudge;

                // Set final scores and match percentage (normalized against the max score)
                s.Score = Math.Round(score, 2);
                s.MatchPercentage = Math.Round(Math.Min(100, (score / maxScore) * 100), 2);

                // Apply the shared weather info
                s.VisibilityChance = Math.Round(weatherChance, 2);
                s.ChanceReason = weatherReason;
            }

            // 5. Return the final list, sorted by the calculated score (highest score first)
            return visibleStars.OrderByDescending(s => s.Score).ToList();
        }
    }
}