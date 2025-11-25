using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Poppy_Universe_Engine
{
    internal class Planet_Engine
    {
        private Visibility_Service visibilityService;
        private double minAltitude;
        private VisibilityCalculator visibilityCalc;
        private Fake_User user;

        public Planet_Engine(Fake_User user, double minAlt = 7.5)
        {
            visibilityService = new Visibility_Service(user.Latitude, user.Longitude);
            minAltitude = minAlt;
            visibilityCalc = new VisibilityCalculator(user);
            this.user = user;
        }

        public List<Planet_View> GetPlanetViews(List<Planet_Objects> planets, DateTime utcTime)
        {
            var planetViews = new List<Planet_View>();

            // Get Earth's position for this time using accurate calculations
            var earthPos = Planetary_Position_Service.CalculateHeliocentricPosition("earth", utcTime);

            foreach (var planet in planets)
            {
                try
                {
                    // Use the accurate planetary position service
                    var planetPos = Planetary_Position_Service.CalculateHeliocentricPosition(planet.Name, utcTime);

                    // Convert to geocentric (relative to Earth)
                    double geocentricX = planetPos.X - earthPos.X;
                    double geocentricY = planetPos.Y - earthPos.Y;
                    double geocentricZ = planetPos.Z - earthPos.Z;

                    // Convert to RA/Dec
                    var (ra, dec) = Planetary_Position_Service.ConvertToEquatorial(geocentricX, geocentricY, geocentricZ);

                    // Calculate altitude and azimuth
                    var (alt, az) = visibilityService.CalculateAltAz(
                        new Star_Objects { RA_ICRS = ra / 15.0, DE_ICRS = dec }, utcTime);

                    bool visible = alt >= minAltitude;

                    planetViews.Add(new Planet_View
                    {
                        Planet = planet,
                        Id = planet.Id,
                        Altitude = alt,
                        Azimuth = az,
                        IsVisible = visible,
                        Score = 0,
                        MatchPercentage = 0,
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
                    // Planet not supported by service, skip it
                    continue;
                }
            }

            return planetViews;
        }

        public List<Planet_View> GetRecommendedPlanets(List<Planet_Objects> planets, DateTime utcTime, Fake_User user)
        {
            var planetViews = GetPlanetViews(planets, utcTime);
            var visiblePlanets = planetViews.Where(p => p.IsVisible).ToList();

            // Fetch weather once for all moons
            var weatherData = visibilityCalc.FetchWeatherAsync().GetAwaiter().GetResult();
            var (weatherChance, weatherReason) = visibilityCalc.ComputeWeatherChanceWithReason(weatherData);

            double maxScore =
                              1.5  // liked planets: 0.5 * 3
                            + 0.4  // rings: 0.2 * 2
                            + 0.9  // comfortable temperature: 0.3 * 3
                            + 0.4  // distance contribution: 0.2 * 2
                            + 5.0  // magnitude + distance: 1 * 5
                            + 2.0; // synergy bonus: liked planet AND comfortable temperature
                                   // Total ≈ 10.2


            Random random = new Random();

            foreach (var p in visiblePlanets)
            {
                double score = 0;

                // Liked planets: 0-3
                double likedScore = user.LikedPlanets.Contains(p.Planet.Name) ? 3 : 0;

                // Rings: +2
                double ringsScore = p.Planet.HasRings ? 2 : 0;

                // Comfortable temperature: 0-3
                double tempScore = (p.Planet.MeanTemperature > -50 && p.Planet.MeanTemperature < 60) ? 3 : 0;

                // Distance from Sun contribution (avoid division by zero)
                double distanceScore = (1 / (p.Planet.DistanceFromSun + 1)) * 2;

                // Magnitude + distance, scaled 0-5, non-linear
                double sunAU = Math.Max(p.DistanceFromSun / 149.6, 1e-6);
                double earthAU = Math.Max(p.DistanceFromEarth / 149.6, 1e-6);
                double magScoreRaw = 5 - (p.Magnitude + 5 * Math.Log10(sunAU * earthAU));
                double magScore = Math.Pow(Math.Max(0, Math.Min(magScoreRaw, 5)), 1.2); // slightly non-linear

                // Synergy bonus: liked AND comfortable temperature
                double synergyBonus = (likedScore > 0 && tempScore > 0) ? 2 : 0;

                // Tiny random nudge
                double randomNudge = random.NextDouble() * 0.5;

                // Weighted sum (tweak weights if you like)
                score = 0.5 * likedScore + 0.2 * ringsScore + 0.3 * tempScore + 0.2 * distanceScore + 1 * magScore + synergyBonus + randomNudge;

                p.Score = Math.Round(score, 2);
                p.MatchPercentage = Math.Round(Math.Min(100, (score / maxScore) * 100), 2);

                // Weather info
                p.VisibilityChance = Math.Round(weatherChance, 2);
                p.ChanceReason = weatherReason;
            }

            return visiblePlanets.OrderByDescending(p => p.Score).ToList();

        }

    }
}