using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Added for async/await calls, though GetAwaiter().GetResult() is used

namespace Poppy_Universe_Engine
{
    // Class responsible for calculating moon visibility and generating a ranked list of moons for the user.
    internal class Layer1_Moon_Engine
    {
        // Dependencies
        private Visibility_Service visibilityService;
        private double minAltitude; // Minimum altitude (in degrees) for an object to be considered visible
        private VisibilityCalculator visibilityCalc;
        private User_Object user;

        /// <summary>
        /// Initializes the Moon Engine with user data and minimum visibility altitude.
        /// </summary>
        /// <param name="user">The observer's location and preferences.</param>
        /// <param name="minAlt">The minimum altitude (degrees) above the horizon for visibility (default 7.5).</param>
        public Layer1_Moon_Engine(User_Object user, double minAlt = 7.5)
        {
            // Initialize services using user's location
            visibilityService = new Visibility_Service(user.Latitude, user.Longitude);
            minAltitude = minAlt;
            visibilityCalc = new VisibilityCalculator(user);
            this.user = user;
        }

        /// <summary>
        /// Compute Altitude, Azimuth, visibility and initialize score for all moons.
        /// </summary>
        /// <param name="moons">List of all known Moon_Objects.</param>
        /// <param name="visiblePlanets">List of planets that are currently visible (already computed).</param>
        /// <param name="utcTime">The current observation time in UTC.</param>
        /// <returns>A list of Moon_View objects containing computed position and visibility data.</returns>
        public List<Moon_View> GetMoonViews(List<Moon_Objects> moons, List<Planet_View> visiblePlanets, DateTime utcTime)
        {
            var moonViews = new List<Moon_View>();

            foreach (var moon in moons)
            {
                // Find the parent planet in the list of currently visible planets (case-insensitive search)
                var parentPlanet = visiblePlanets.FirstOrDefault(p =>
                    string.Equals(p.Planet.Name, moon.Parent, StringComparison.OrdinalIgnoreCase));

                if (parentPlanet == null)
                {
                    // Parent planet is either not visible or not present in the provided list, so skip the moon.
                    continue;
                }

                // Calculate the moon's position (Right Ascension (RA) and Declination (Dec)) 
                // relative to its parent planet and the Earth (geocentric coordinates).
                var (ra, dec) = CalculateMoonPosition(moon, parentPlanet, utcTime);

                // Calculate altitude and azimuth for the moon from the observer's location (using RA/Dec)
                // Note: RA_ICRS is converted from degrees to hours (RA / 15.0) for the service.
                var (alt, az) = visibilityService.CalculateAltAz(
                    new Star_Objects { RA_ICRS = ra / 15.0, DE_ICRS = dec }, utcTime);

                // Moon is visible if its altitude is above the minimum threshold 
                // AND its parent planet has been determined to be visible.
                bool visible = alt >= minAltitude && parentPlanet.IsVisible;

                // Create and populate the Moon_View object with computed data
                moonViews.Add(new Moon_View
                {
                    Moon = moon,
                    Id = moon.Id,
                    Parent = moon.Parent,
                    Altitude = alt,
                    Azimuth = az,
                    IsVisible = visible,
                    // Initialize scoring fields (will be populated later in GetRecommendedMoons)
                    Score = 0,
                    MatchPercentage = 0,
                    VisibilityChance = 0,
                    ChanceReason = ""
                });
            }

            return moonViews;
        }

        /// <summary>
        /// Calculates the geocentric Right Ascension (RA) and Declination (Dec) of a moon.
        /// This involves using the moon's orbital elements relative to its parent planet
        /// and combining the moon's position vector with the parent planet's geocentric position.
        /// </summary>
        /// <returns>A tuple containing (Right Ascension in degrees, Declination in degrees).</returns>
        private (double RA, double Dec) CalculateMoonPosition(Moon_Objects moon, Planet_View parentPlanet, DateTime utcTime)
        {
            // --- Step 1: Time Calculation ---
            double jd = ToJulianDate(utcTime);
            double daysSinceEpoch = jd - 2451545.0; // J2000.0 epoch

            // --- Step 2: Convert Orbital Elements to Radians/AU ---
            double a = moon.SemiMajorAxisKm / 149597870.7; // Convert Semi-Major Axis from km to AU
            double e = moon.Eccentricity;
            double i = moon.Inclination * Math.PI / 180.0; // Inclination
            double omega = moon.ArgumentOfPeriapsis * Math.PI / 180.0; // Argument of Periapsis
            double Omega = moon.LongitudeOfAscendingNode * Math.PI / 180.0; // Longitude of Ascending Node
            double M0 = moon.MeanAnomalyAtEpoch * Math.PI / 180.0; // Mean Anomaly at Epoch

            // --- Step 3: Calculate Mean Motion (n) ---
            double n;
            if (moon.MeanMotion > 0)
            {
                // Use provided Mean Motion
                n = moon.MeanMotion * Math.PI / 180.0;
            }
            else if (moon.OrbitalPeriod > 0)
            {
                // Calculate n from Orbital Period (n = 2π / P)
                n = (2 * Math.PI) / moon.OrbitalPeriod;
            }
            else
            {
                // Fallback: Estimate n using a rough approximation based on Kepler's third law 
                // and a ratio against the Earth-Moon system.
                double periodDays = Math.Sqrt(Math.Pow(moon.SemiMajorAxisKm / 384400.0, 3)) * 27.3;
                n = (2 * Math.PI) / periodDays;
            }

            // --- Step 4: Calculate Current Mean Anomaly (M) ---
            double M = M0 + n * daysSinceEpoch;
            M = M % (2 * Math.PI); // Normalize to 0-2π range
            if (M < 0) M += 2 * Math.PI;

            // --- Step 5: Solve Kepler's Equation to find Eccentric Anomaly (E) ---
            double E = SolveKeplerEquation(M, e);

            // --- Step 6: Calculate True Anomaly (nu) and Distance (r) ---
            double nu = 2 * Math.Atan2(
                Math.Sqrt(1 + e) * Math.Sin(E / 2),
                Math.Sqrt(1 - e) * Math.Cos(E / 2)
            );
            double r = a * (1 - e * Math.Cos(E)); // Distance from parent planet (in AU)

            // --- Step 7: Position in Orbital Plane ---
            double xOrbit = r * Math.Cos(nu);
            double yOrbit = r * Math.Sin(nu);

            // --- Step 8: Transform Moon Position to Reference Frame (e.g., Ecliptic/Equatorial relative to planet) ---

            // Precompute trig values for rotation
            double cosOmega = Math.Cos(omega);
            double sinOmega = Math.Sin(omega);
            double cosI = Math.Cos(i);
            double sinI = Math.Sin(i);
            double cosOmegaCap = Math.Cos(Omega);
            double sinOmegaCap = Math.Sin(Omega);

            // Apply 3D rotation based on orbital elements (Omega, i, omega) to get planet-relative (x, y, z)
            double xRel = (cosOmegaCap * cosOmega - sinOmegaCap * sinOmega * cosI) * xOrbit +
                          (-cosOmegaCap * sinOmega - sinOmegaCap * cosOmega * cosI) * yOrbit;

            double yRel = (sinOmegaCap * cosOmega + cosOmegaCap * sinOmega * cosI) * xOrbit +
                          (-sinOmegaCap * sinOmega + cosOmegaCap * cosOmega * cosI) * yOrbit;

            double zRel = (sinOmega * sinI) * xOrbit + (cosOmega * sinI) * yOrbit;

            // --- Step 9: Shift to Geocentric Position ---
            // Add moon's planet-relative position to parent planet's known geocentric position (all in AU)
            double moonGeoX = parentPlanet.GeocentricX + xRel;
            double moonGeoY = parentPlanet.GeocentricY + yRel;
            double moonGeoZ = parentPlanet.GeocentricZ + zRel;

            // --- Step 10: Convert Geocentric Ecliptic to Equatorial Coordinates (ICRS/J2000.0) ---
            double obliquity = 23.43928 * Math.PI / 180.0; // Obliquity of the Ecliptic (J2000.0)

            double xEquat = moonGeoX;
            // The following rotates from Ecliptic plane to Equatorial plane
            double yEquat = moonGeoY * Math.Cos(obliquity) - moonGeoZ * Math.Sin(obliquity);
            double zEquat = moonGeoY * Math.Sin(obliquity) + moonGeoZ * Math.Cos(obliquity);

            // --- Step 11: Calculate Final RA and Dec ---
            double ra = Math.Atan2(yEquat, xEquat) * 180.0 / Math.PI; // Right Ascension (in degrees)
            if (ra < 0) ra += 360.0; // Normalize RA to 0-360 degrees

            double distance = Math.Sqrt(xEquat * xEquat + yEquat * yEquat + zEquat * zEquat);
            double dec = Math.Asin(zEquat / distance) * 180.0 / Math.PI; // Declination (in degrees)

            return (ra, dec);
        }

        /// <summary>
        /// Solves Kepler's equation (M = E - e*sin(E)) for the Eccentric Anomaly (E)
        /// using the Newton-Raphson iterative method.
        /// </summary>
        /// <param name="M">Mean Anomaly (in radians).</param>
        /// <param name="e">Eccentricity.</param>
        /// <param name="tolerance">Convergence tolerance.</param>
        /// <returns>Eccentric Anomaly (E) in radians.</returns>
        private double SolveKeplerEquation(double M, double e, double tolerance = 1e-8)
        {
            double E = M; // Initial guess for E is M (Mean Anomaly)

            for (int i = 0; i < 30; i++) // Max 30 iterations to ensure termination
            {
                double sinE = Math.Sin(E);
                double cosE = Math.Cos(E);
                // The correction factor (dE) is f(E) / f'(E) from Newton-Raphson
                double dE = (E - e * sinE - M) / (1 - e * cosE);
                E -= dE;

                if (Math.Abs(dE) < tolerance)
                    break; // Convergence achieved
            }

            return E;
        }

        /// <summary>
        /// Converts a UTC DateTime object to a Julian Date (JD).
        /// </summary>
        private double ToJulianDate(DateTime utcTime)
        {
            int year = utcTime.Year;
            int month = utcTime.Month;
            // Calculate fractional day
            double day = utcTime.Day + utcTime.Hour / 24.0 +
                         utcTime.Minute / 1440.0 + utcTime.Second / 86400.0;

            // Handle January/February for JD calculation
            if (month <= 2)
            {
                year--;
                month += 12;
            }

            // A and B are terms for the Gregorian calendar correction
            int A = year / 100;
            int B = 2 - A + (A / 4);

            // Full Julian Date calculation formula
            double jd = Math.Floor(365.25 * (year + 4716)) +
                        Math.Floor(30.6001 * (month + 1)) +
                        day + B - 1524.5;

            return jd;
        }

        /// <summary>
        /// Get recommended moons (filtered + scored + ranked), including weather info.
        /// </summary>
        /// <param name="moons">List of all known Moon_Objects.</param>
        /// <param name="visiblePlanets">List of planets that are currently visible.</param>
        /// <param name="utcTime">The current observation time in UTC.</param>
        /// <returns>A ranked list of visible Moon_View objects.</returns>
        public List<Moon_View> GetRecommendedMoons(List<Moon_Objects> moons, List<Planet_View> visiblePlanets, DateTime utcTime)
        {
            // 1. Calculate positions and visibility for all moons
            var moonViews = GetMoonViews(moons, visiblePlanets, utcTime);
            // Filter down to only those that are currently visible
            var visibleMoons = moonViews.Where(m => m.IsVisible).ToList();

            // 2. Fetch weather once for all moons
            // Note: This uses GetAwaiter().GetResult() to synchronously call an async method (blocking call).
            var weatherData = visibilityCalc.FetchWeatherAsync().GetAwaiter().GetResult();
            // Compute a single visibility chance score based on local weather conditions
            var (weatherChance, weatherReason) = visibilityCalc.ComputeWeatherChanceWithReason(weatherData);

            // 3. Define the theoretical maximum score for normalization
            double maxScore =
                                 0.3    // Liked parent planet (0.3 * 1)
                               + 1.0    // Liked moon itself (0.5 * 2)
                               + 0.4    // Composition score (0.2 * 2)
                               + 0.4    // Surface features score (0.2 * 2)
                               + 0.9    // Distance from Earth (0.3 * Max 3)
                               + 1.5    // Magnitude (0.75 * Max 2)
                               + 1.0;   // Synergy bonus (Max 1)
                                        // Total theoretical maximum score

            Random random = new Random();

            // 4. Calculate individual scores for each visible moon
            foreach (var m in visibleMoons)
            {
                double score = 0;

                // --- Scoring Components ---

                // 1) Liked parent planet: 1 point if the parent is in the user's liked list, 0 otherwise.
                double likedPlanetScore = (user.LikedPlanets != null && user.LikedPlanets.Contains(m.Moon.Parent)) ? 1 : 0;

                // 2) Liked moon itself: 2 points if the moon is in the user's liked list.
                double likedMoonScore = (user.LikedMoons != null && user.LikedMoons.Contains(m.Moon.Name)) ? 2 : 0;

                // 3) Composition & surface features: 2 points each if the data is present (non-empty string).
                double compositionScore = !string.IsNullOrEmpty(m.Moon.Composition) ? 2 : 0;
                double surfaceScore = !string.IsNullOrEmpty(m.Moon.SurfaceFeatures) ? 2 : 0;

                // 4) Distance from Earth (non-linear): Scores higher for closer moons (max 3 points).
                double distanceScore = 0;
                if (m.Moon.DistanceFromEarth > 0)
                {
                    // Inverse power relationship, capped at 3
                    distanceScore = Math.Min(3, Math.Pow(1_000.0 / (m.Moon.DistanceFromEarth + 1.0), 1.2));
                }

                // 5) Magnitude (non-linear): Scores higher for brighter moons (lower magnitude value, max 2 points).
                double magScore = 0;
                if (m.Moon.Magnitude > 0)
                {
                    // Inverse relationship (smaller magnitude = brighter = higher score), capped at 2
                    magScore = Math.Max(0, Math.Min(2, Math.Pow(2 - (m.Moon.Magnitude / 10.0), 1.2)));
                }

                // 6) Synergy bonus: 1 extra point if the user likes both the moon AND its parent planet.
                double synergyBonus = (likedPlanetScore > 0 && likedMoonScore > 0) ? 1 : 0;

                // 7) Tiny random nudge: A small random value (0-0.3) to break ties and add variation.
                double randomNudge = random.NextDouble() * 0.3;

                // --- Final Weighted Sum ---
                score = 0.3 * likedPlanetScore +
                        0.5 * likedMoonScore +
                        0.2 * compositionScore +
                        0.2 * surfaceScore +
                        0.3 * distanceScore +
                        0.75 * magScore +
                        synergyBonus +
                        randomNudge;

                // Set final scores and match percentage (normalized against the max score)
                m.Score = Math.Round(score, 2);
                m.MatchPercentage = Math.Round(Math.Min(100, (score / maxScore) * 100), 2);

                // Apply the single, shared weather info to all visible moons
                m.VisibilityChance = Math.Round(weatherChance, 2);
                m.ChanceReason = weatherReason;
            }

            // 5. Return the final list, sorted by the calculated score (highest score first)
            return visibleMoons.OrderByDescending(m => m.Score).ToList();
        }
    }
}