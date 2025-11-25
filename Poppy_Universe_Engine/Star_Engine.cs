using System;
using System.Collections.Generic;
using System.Linq;

namespace Poppy_Universe_Engine
{
    internal class Star_Engine
    {
        private Visibility_Service visibilityService;
        private double minAltitude;
        private VisibilityCalculator visibilityCalc;
        private Fake_User user;

        public Star_Engine(Fake_User user, double minAlt = 7.5)
        {
            visibilityService = new Visibility_Service(user.Latitude, user.Longitude);
            minAltitude = minAlt;
            visibilityCalc = new VisibilityCalculator(user);
            this.user = user;
        }

        /// <summary>
        /// Compute Altitude, Azimuth, visibility and initialize score for all stars
        /// </summary>
        public List<Star_View> GetStarViews(List<Star_Objects> stars, DateTime utcTime)
        {
            var starViews = new List<Star_View>();

            foreach (var star in stars)
            {
                var (alt, az) = visibilityService.CalculateAltAz(star, utcTime);
                bool visible = alt >= minAltitude; // apply horizon buffer

                starViews.Add(new Star_View
                {
                    Star = star,
                    Id = star.Id,
                    Source = star.Source,
                    Altitude = alt,
                    Azimuth = az,
                    IsVisible = visible,
                    Score = 0,
                    MatchPercentage = 0,
                    VisibilityChance = 0,
                    ChanceReason = ""
                });
            }

            return starViews;
        }

        /// <summary>
        /// Get recommended stars (filtered + scored + ranked)
        /// </summary>
        public List<Star_View> GetRecommendedStars(List<Star_Objects> stars, DateTime utcTime, Fake_User user)
        {
            var starViews = GetStarViews(stars, utcTime);
            var visibleStars = starViews.Where(s => s.IsVisible).ToList();

            // Fetch weather once for all moons
            var weatherData = visibilityCalc.FetchWeatherAsync().GetAwaiter().GetResult();
            var (weatherChance, weatherReason) = visibilityCalc.ComputeWeatherChanceWithReason(weatherData);

            double maxScore =
                              2.5   // liked stars: 0.5 * 5
                            + 2.0   // spectral preference: 0.4 * 5
                            + 3.75  // brightness (weighted 0.75 * max 5)
                            + 2.0;  // synergy bonus: liked star AND preferred spectral type
                                    // Total ≈ 10.25



            Random random = new Random();

            foreach (var s in visibleStars)
            {
                double score = 0;

                // Liked stars
                double likedScore = user.LikedStars.Contains(s.Star.Name) ? 5 : 0;

                // Spectral preference
                double spectralScore = (user.FavoriteSpectralTypes != null && user.FavoriteSpectralTypes.Contains(s.Star.SpectralType)) ? 5 : 0;

                // Brightness (non-linear, brighter = more appealing)
                double brightnessScore = Math.Pow(Math.Max(0, 5 - s.Star.Gmag), 1.5);

                // Synergy bonus: liked star AND preferred spectral type
                double synergyBonus = (likedScore > 0 && spectralScore > 0) ? 2 : 0;

                // Tiny random nudge
                double randomNudge = random.NextDouble() * 0.5;

                // Weighted sum (you can tweak weights)
                score = 0.5 * likedScore + 0.4 * spectralScore + 0.75 * brightnessScore + synergyBonus + randomNudge;

                s.Score = Math.Round(score, 2);
                s.MatchPercentage = Math.Round(Math.Min(100, (score / maxScore) * 100), 2);

                // Weather info
                s.VisibilityChance = Math.Round(weatherChance, 2);
                s.ChanceReason = weatherReason;
            }

            return visibleStars.OrderByDescending(s => s.Score).ToList();
        }
    }
}
