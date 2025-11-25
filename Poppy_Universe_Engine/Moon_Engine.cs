using System;
using System.Collections.Generic;
using System.Linq;

namespace Poppy_Universe_Engine
{
    internal class Moon_Engine
    {
        private Visibility_Service visibilityService;
        private double minAltitude;
        private VisibilityCalculator visibilityCalc;
        private Fake_User user;

        public Moon_Engine(Fake_User user,double minAlt = 7.5)
        {
            visibilityService = new Visibility_Service(user.Latitude, user.Longitude);
            minAltitude = minAlt;
            visibilityCalc = new VisibilityCalculator(user);
            this.user = user;
        }

        /// <summary>
        /// Compute Altitude, Azimuth, visibility and initialize score for all moons
        /// </summary>
        public List<Moon_View> GetMoonViews(List<Moon_Objects> moons, List<Planet_View> visiblePlanets, DateTime utcTime)
        {
            var moonViews = new List<Moon_View>();

            foreach (var moon in moons)
            {
                // Find parent planet (case-insensitive)
                var parentPlanet = visiblePlanets.FirstOrDefault(p =>
                    string.Equals(p.Planet.Name, moon.Parent, StringComparison.OrdinalIgnoreCase));

                if (parentPlanet == null)
                {
                    // Parent planet not in view list, skip this moon
                    continue;
                }

                // Calculate moon's position relative to its parent planet
                var (ra, dec) = CalculateMoonPosition(moon, parentPlanet, utcTime);

                // Calculate altitude and azimuth
                var (alt, az) = visibilityService.CalculateAltAz(
                    new Star_Objects { RA_ICRS = ra / 15.0, DE_ICRS = dec }, utcTime);

                // Moon is visible if it's above horizon AND parent planet is visible
                bool visible = alt >= minAltitude && parentPlanet.IsVisible;

                moonViews.Add(new Moon_View
                {
                    Moon = moon,
                    Id = moon.Id,
                    Altitude = alt,
                    Azimuth = az,
                    IsVisible = visible,
                    Score = 0,
                    MatchPercentage = 0,
                    VisibilityChance = 0,
                    ChanceReason = ""
                });
            }

            return moonViews;
        }

        private (double RA, double Dec) CalculateMoonPosition(Moon_Objects moon, Planet_View parentPlanet, DateTime utcTime)
        {
            // Get Julian Date
            double jd = ToJulianDate(utcTime);
            double daysSinceEpoch = jd - 2451545.0;

            // Convert orbital elements
            double a = moon.SemiMajorAxisKm / 149597870.7; // Convert km to AU
            double e = moon.Eccentricity;
            double i = moon.Inclination * Math.PI / 180.0;
            double omega = moon.ArgumentOfPeriapsis * Math.PI / 180.0;
            double Omega = moon.LongitudeOfAscendingNode * Math.PI / 180.0;
            double M0 = moon.MeanAnomalyAtEpoch * Math.PI / 180.0;

            // Calculate mean motion (radians per day)
            double n;
            if (moon.MeanMotion > 0)
            {
                n = moon.MeanMotion * Math.PI / 180.0;
            }
            else if (moon.OrbitalPeriod > 0)
            {
                n = (2 * Math.PI) / moon.OrbitalPeriod;
            }
            else
            {
                // Fallback: estimate using Kepler's third law (rough approximation)
                // For planet-moon system, using Earth-Moon as reference
                double periodDays = Math.Sqrt(Math.Pow(moon.SemiMajorAxisKm / 384400.0, 3)) * 27.3;
                n = (2 * Math.PI) / periodDays;
            }

            // Calculate current mean anomaly
            double M = M0 + n * daysSinceEpoch;

            // Normalize to 0-2π range
            M = M % (2 * Math.PI);
            if (M < 0) M += 2 * Math.PI;

            // Solve Kepler's equation
            double E = SolveKeplerEquation(M, e);

            // Calculate true anomaly
            double nu = 2 * Math.Atan2(
                Math.Sqrt(1 + e) * Math.Sin(E / 2),
                Math.Sqrt(1 - e) * Math.Cos(E / 2)
            );

            // Distance from planet (in AU)
            double r = a * (1 - e * Math.Cos(E));

            // Position in orbital plane (relative to parent planet)
            double xOrbit = r * Math.Cos(nu);
            double yOrbit = r * Math.Sin(nu);

            // Precompute trig values
            double cosOmega = Math.Cos(omega);
            double sinOmega = Math.Sin(omega);
            double cosI = Math.Cos(i);
            double sinI = Math.Sin(i);
            double cosOmegaCap = Math.Cos(Omega);
            double sinOmegaCap = Math.Sin(Omega);

            // Transform from orbital plane to reference frame
            // Using standard 3D rotation matrices: Rz(Omega) * Rx(i) * Rz(omega)
            double xRel = (cosOmegaCap * cosOmega - sinOmegaCap * sinOmega * cosI) * xOrbit +
                         (-cosOmegaCap * sinOmega - sinOmegaCap * cosOmega * cosI) * yOrbit;

            double yRel = (sinOmegaCap * cosOmega + cosOmegaCap * sinOmega * cosI) * xOrbit +
                         (-sinOmegaCap * sinOmega + cosOmegaCap * cosOmega * cosI) * yOrbit;

            double zRel = (sinOmega * sinI) * xOrbit + (cosOmega * sinI) * yOrbit;

            // Add moon's position to parent planet's geocentric position (all in AU)
            double moonGeoX = parentPlanet.GeocentricX + xRel;
            double moonGeoY = parentPlanet.GeocentricY + yRel;
            double moonGeoZ = parentPlanet.GeocentricZ + zRel;

            // Convert ecliptic geocentric coordinates to equatorial coordinates
            double obliquity = 23.43928 * Math.PI / 180.0; // J2000.0 obliquity

            double xEquat = moonGeoX;
            double yEquat = moonGeoY * Math.Cos(obliquity) - moonGeoZ * Math.Sin(obliquity);
            double zEquat = moonGeoY * Math.Sin(obliquity) + moonGeoZ * Math.Cos(obliquity);

            // Calculate RA and Dec
            double ra = Math.Atan2(yEquat, xEquat) * 180.0 / Math.PI;
            if (ra < 0) ra += 360.0;

            double distance = Math.Sqrt(xEquat * xEquat + yEquat * yEquat + zEquat * zEquat);
            double dec = Math.Asin(zEquat / distance) * 180.0 / Math.PI;

            return (ra, dec);
        }

        private double SolveKeplerEquation(double M, double e, double tolerance = 1e-8)
        {
            // Newton-Raphson method to solve M = E - e*sin(E)
            double E = M; // Initial guess

            for (int i = 0; i < 30; i++)
            {
                double sinE = Math.Sin(E);
                double cosE = Math.Cos(E);
                double dE = (E - e * sinE - M) / (1 - e * cosE);
                E -= dE;

                if (Math.Abs(dE) < tolerance)
                    break;
            }

            return E;
        }

        private double ToJulianDate(DateTime utcTime)
        {
            int year = utcTime.Year;
            int month = utcTime.Month;
            double day = utcTime.Day + utcTime.Hour / 24.0 +
                        utcTime.Minute / 1440.0 + utcTime.Second / 86400.0;

            if (month <= 2)
            {
                year--;
                month += 12;
            }

            int A = year / 100;
            int B = 2 - A + (A / 4);

            double jd = Math.Floor(365.25 * (year + 4716)) +
                       Math.Floor(30.6001 * (month + 1)) +
                       day + B - 1524.5;

            return jd;
        }

        /// <summary>
        /// Get recommended moons (filtered + scored + ranked), including weather info
        /// </summary>
        public List<Moon_View> GetRecommendedMoons(List<Moon_Objects> moons, List<Planet_View> visiblePlanets, DateTime utcTime)
        {
            var moonViews = GetMoonViews(moons, visiblePlanets, utcTime);
            var visibleMoons = moonViews.Where(m => m.IsVisible).ToList();


            // Fetch weather once for all moons
            var weatherData = visibilityCalc.FetchWeatherAsync().GetAwaiter().GetResult();
            var (weatherChance, weatherReason) = visibilityCalc.ComputeWeatherChanceWithReason(weatherData);

            double maxScore =
                              0.3   // liked parent planet: 0.3 * 1
                            + 1.0   // liked moon itself: 0.5 * 2
                            + 0.4   // composition: 0.2 * 2
                            + 0.4   // surface features: 0.2 * 2
                            + 0.9   // distance from Earth: 0.3 * 3
                            + 1.5   // magnitude: 0.75 * 2
                            + 1.0;  // synergy bonus: liked planet AND liked moon
                                    // Total ≈ 5.5


            Random random = new Random();

            foreach (var m in visibleMoons)
            {
                double score = 0;

                // 1) Liked parent planet
                double likedPlanetScore = (user.LikedPlanets != null && user.LikedPlanets.Contains(m.Moon.Parent)) ? 1 : 0;

                // 2) Liked moon itself
                double likedMoonScore = (user.LikedMoons != null && user.LikedMoons.Contains(m.Moon.Name)) ? 2 : 0;

                // 3) Composition & surface features
                double compositionScore = !string.IsNullOrEmpty(m.Moon.Composition) ? 2 : 0;
                double surfaceScore = !string.IsNullOrEmpty(m.Moon.SurfaceFeatures) ? 2 : 0;

                // 4) Distance from Earth (non-linear)
                double distanceScore = 0;
                if (m.Moon.DistanceFromEarth > 0)
                {
                    distanceScore = Math.Min(3, Math.Pow(1_000.0 / (m.Moon.DistanceFromEarth + 1.0), 1.2));
                }

                // 5) Magnitude (non-linear)
                double magScore = 0;
                if (m.Moon.Magnitude > 0)
                {
                    magScore = Math.Max(0, Math.Min(2, Math.Pow(2 - (m.Moon.Magnitude / 10.0), 1.2)));
                }

                // 6) Synergy bonus: liked planet AND liked moon
                double synergyBonus = (likedPlanetScore > 0 && likedMoonScore > 0) ? 1 : 0;

                // 7) Tiny random nudge
                double randomNudge = random.NextDouble() * 0.3;

                // Weighted sum (adjust weights to taste)
                score = 0.3 * likedPlanetScore + 0.5 * likedMoonScore + 0.2 * compositionScore + 0.2 * surfaceScore + 0.3 * distanceScore + 0.75 * magScore + synergyBonus + randomNudge;

                m.Score = Math.Round(score, 2);
                m.MatchPercentage = Math.Round(Math.Min(100, (score / maxScore) * 100), 2);

                // Weather info
                m.VisibilityChance = Math.Round(weatherChance, 2);
                m.ChanceReason = weatherReason;
            }

            return visibleMoons.OrderByDescending(m => m.Score).ToList();
        }
    }
}

