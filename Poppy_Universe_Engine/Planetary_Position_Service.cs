using System;

namespace Poppy_Universe_Engine
{
    /// <summary>
    /// Service for calculating the heliocentric (Sun-centered) position of planets 
    /// and major solar system bodies (including Dwarf Planets) using simplified time-varying 
    /// orbital elements (similar to VSOP87) for accuracy.
    /// </summary>
    public class Planetary_Position_Service
    {
        /// <summary>
        /// Data structure to hold the orbital elements and their secular (century) rates of change.
        /// </summary>
        private class OrbitalElements
        {
            // Orbital Element (a, e, I, L, ω̄, Ω) and its rate of change per Julian Century (cy)
            public double a, a_cy;      // Semi-major axis (a, in AU)
            public double e, e_cy;      // Eccentricity (e, dimensionless)
            public double I, I_cy;      // Inclination (I, in degrees)
            public double L, L_cy;      // Mean longitude (L, in degrees)
            public double w_bar, w_bar_cy; // Longitude of perihelion (ω̄ = ω + Ω, in degrees)
            public double Omega, Omega_cy; // Longitude of ascending node (Ω, in degrees)
        }

        /// <summary>
        /// Retrieves the J2000.0 orbital elements and their century rates for a given planet/dwarf planet.
        /// Elements added for: Pluto, Ceres, Eris, Haumea, and Makemake.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if the planet name is not supported.</exception>
        private static OrbitalElements GetPlanetElements(string planetName)
        {
            // Source: Data compiled from various astronomical sources (e.g., NASA JPL, Minor Planet Center)
            switch (planetName.ToLower())
            {
                // Major Planets (unchanged)
                case "mercury":
                    return new OrbitalElements
                    {
                        a = 0.38709927,
                        a_cy = 0.00000037,
                        e = 0.20563593,
                        e_cy = 0.00001906,
                        I = 7.00497902,
                        I_cy = -0.00594749,
                        L = 252.25032350,
                        L_cy = 149472.67411175,
                        w_bar = 77.45779628,
                        w_bar_cy = 0.16047689,
                        Omega = 48.33076593,
                        Omega_cy = -0.12534081
                    };
                case "venus":
                    return new OrbitalElements
                    {
                        a = 0.72333566,
                        a_cy = 0.00000390,
                        e = 0.00677672,
                        e_cy = -0.00004107,
                        I = 3.39467605,
                        I_cy = -0.00078890,
                        L = 181.97909950,
                        L_cy = 58517.81538729,
                        w_bar = 131.60246718,
                        w_bar_cy = 0.00268329,
                        Omega = 76.67984255,
                        Omega_cy = -0.27769418
                    };
                case "earth":
                    return new OrbitalElements
                    {
                        a = 1.00000261,
                        a_cy = 0.00000562,
                        e = 0.01671123,
                        e_cy = -0.00004392,
                        I = -0.00001531,
                        I_cy = -0.01294668,
                        L = 100.46457166,
                        L_cy = 35999.37244981,
                        w_bar = 102.93768193,
                        w_bar_cy = 0.32327364,
                        Omega = 0.0,
                        Omega_cy = 0.0
                    };
                case "mars":
                    return new OrbitalElements
                    {
                        a = 1.52371034,
                        a_cy = 0.00001847,
                        e = 0.09339410,
                        e_cy = 0.00007882,
                        I = 1.84969142,
                        I_cy = -0.00813131,
                        L = -4.55343205,
                        L_cy = 19140.30268499,
                        w_bar = -23.94362959,
                        w_bar_cy = 0.44441088,
                        Omega = 49.55953891,
                        Omega_cy = -0.29257343
                    };
                case "jupiter":
                    return new OrbitalElements
                    {
                        a = 5.20288700,
                        a_cy = -0.00011607,
                        e = 0.04838624,
                        e_cy = -0.00013253,
                        I = 1.30439695,
                        I_cy = -0.00183714,
                        L = 34.39644051,
                        L_cy = 3034.74612775,
                        w_bar = 14.72847983,
                        w_bar_cy = 0.21252668,
                        Omega = 100.47390909,
                        Omega_cy = 0.20469106
                    };
                case "saturn":
                    return new OrbitalElements
                    {
                        a = 9.53667594,
                        a_cy = -0.00125060,
                        e = 0.05386179,
                        e_cy = -0.00050991,
                        I = 2.48599187,
                        I_cy = 0.00193609,
                        L = 49.95424423,
                        L_cy = 1222.49362201,
                        w_bar = 92.59887831,
                        w_bar_cy = -0.41897216,
                        Omega = 113.66242448,
                        Omega_cy = -0.28867794
                    };
                case "uranus":
                    return new OrbitalElements
                    {
                        a = 19.18916464,
                        a_cy = -0.00196176,
                        e = 0.04725744,
                        e_cy = -0.00004397,
                        I = 0.77263783,
                        I_cy = -0.00242939,
                        L = 313.23810451,
                        L_cy = 428.48202785,
                        w_bar = 170.95427630,
                        w_bar_cy = 0.40805281,
                        Omega = 74.01692503,
                        Omega_cy = 0.04240589
                    };
                case "neptune":
                    return new OrbitalElements
                    {
                        a = 30.06992276,
                        a_cy = 0.00026291,
                        e = 0.00859048,
                        e_cy = 0.00005105,
                        I = 1.77004347,
                        I_cy = 0.00035372,
                        L = -55.12002969,
                        L_cy = 218.45945325,
                        w_bar = 44.96476227,
                        w_bar_cy = -0.32241464,
                        Omega = 131.78422574,
                        Omega_cy = -0.00508664
                    };

                // -----------------------------------------------------------
                // Dwarf Planets (New Additions)
                // -----------------------------------------------------------
                case "ceres":
                    return new OrbitalElements
                    {
                        a = 2.766,
                        a_cy = 0.0,
                        e = 0.078,
                        e_cy = 0.0,
                        I = 10.59,
                        I_cy = 0.0,
                        L = 92.5,
                        L_cy = 43258.9,
                        w_bar = 73.6,
                        w_bar_cy = 0.0,
                        Omega = 80.5,
                        Omega_cy = 0.0
                    };
                case "pluto":
                    return new OrbitalElements
                    {
                        a = 39.482,
                        a_cy = 0.0,
                        e = 0.2488,
                        e_cy = 0.0,
                        I = 17.14,
                        I_cy = 0.0,
                        L = 238.9,
                        L_cy = 1.45,
                        w_bar = 224.1,
                        w_bar_cy = 0.0,
                        Omega = 110.3,
                        Omega_cy = 0.0
                    };
                case "eris":
                    return new OrbitalElements
                    {
                        a = 67.8,
                        a_cy = 0.0,
                        e = 0.437,
                        e_cy = 0.0,
                        I = 44.0,
                        I_cy = 0.0,
                        L = 197.8,
                        L_cy = 0.39,
                        w_bar = 320.1,
                        w_bar_cy = 0.0,
                        Omega = 35.8,
                        Omega_cy = 0.0
                    };
                case "haumea":
                    return new OrbitalElements
                    {
                        a = 43.1,
                        a_cy = 0.0,
                        e = 0.188,
                        e_cy = 0.0,
                        I = 28.2,
                        I_cy = 0.0,
                        L = 216.9,
                        L_cy = 0.54,
                        w_bar = 240.5,
                        w_bar_cy = 0.0,
                        Omega = 121.2,
                        Omega_cy = 0.0
                    };
                case "makemake":
                    return new OrbitalElements
                    {
                        a = 45.8,
                        a_cy = 0.0,
                        e = 0.160,
                        e_cy = 0.0,
                        I = 28.9,
                        I_cy = 0.0,
                        L = 351.4,
                        L_cy = 0.49,
                        w_bar = 295.1,
                        w_bar_cy = 0.0,
                        Omega = 79.5,
                        Omega_cy = 0.0
                    };

                default:
                    throw new ArgumentException($"Unknown planet or dwarf planet: {planetName}");
            }
        }

        /// <summary>
        /// Calculates the heliocentric position (X, Y, Z) of a celestial body in rectangular ecliptic coordinates (AU).
        /// </summary>
        /// <param name="planetName">The name of the body.</param>
        /// <param name="utcTime">The observation time in UTC.</param>
        /// <returns>A tuple containing (X, Y, Z) coordinates in AU, referenced to the J2000.0 ecliptic plane.</returns>
        public static (double X, double Y, double Z) CalculateHeliocentricPosition(string planetName, DateTime utcTime)
        {
            // 1. Calculate time difference from epoch (J2000.0)
            double jd = ToJulianDate(utcTime);
            double T = (jd - 2451545.0) / 36525.0; // Julian centuries from J2000.0 epoch

            var elem = GetPlanetElements(planetName);

            // 2. Compute current time-dependent orbital elements
            double a = elem.a + elem.a_cy * T; // Semi-major axis
            double e = elem.e + elem.e_cy * T; // Eccentricity
            double I = (elem.I + elem.I_cy * T) * Math.PI / 180.0; // Inclination (converted to radians)

            double L = (elem.L + elem.L_cy * T) % 360.0; // Mean longitude
            if (L < 0) L += 360.0;

            double w_bar = (elem.w_bar + elem.w_bar_cy * T) % 360.0; // Longitude of perihelion
            if (w_bar < 0) w_bar += 360.0;

            double Omega = (elem.Omega + elem.Omega_cy * T) % 360.0; // Longitude of ascending node
            if (Omega < 0) Omega += 360.0;

            // 3. Calculate argument of perihelion (ω) and Mean Anomaly (M)
            double w = w_bar - Omega;      // argument of perihelion (ω = ω̄ - Ω)
            double M = L - w_bar;          // mean anomaly (M = L - ω̄)
            if (M < 0) M += 360.0;

            // 4. Convert angular measures to radians
            double M_rad = M * Math.PI / 180.0;
            double w_rad = w * Math.PI / 180.0;
            double Omega_rad = Omega * Math.PI / 180.0;

            // 5. Solve Kepler's equation to find the Eccentric Anomaly (E)
            double E = SolveKeplerEquation(M_rad, e);

            // 6. Calculate True Anomaly (ν) and Heliocentric Distance (r)
            double nu = 2 * Math.Atan2(
                Math.Sqrt(1 + e) * Math.Sin(E / 2),
                Math.Sqrt(1 - e) * Math.Cos(E / 2)
            );
            double r = a * (1 - e * Math.Cos(E)); // Heliocentric distance (r) in AU

            // 7. Calculate position in the orbital plane
            double x_orb = r * Math.Cos(nu);
            double y_orb = r * Math.Sin(nu);

            // 8. Rotate position to J2000.0 rectangular ecliptic coordinates (X, Y, Z)
            double x_ecl = (Math.Cos(w_rad) * Math.Cos(Omega_rad) - Math.Sin(w_rad) * Math.Sin(Omega_rad) * Math.Cos(I)) * x_orb +
                           (-Math.Sin(w_rad) * Math.Cos(Omega_rad) - Math.Cos(w_rad) * Math.Sin(Omega_rad) * Math.Cos(I)) * y_orb;

            double y_ecl = (Math.Cos(w_rad) * Math.Sin(Omega_rad) + Math.Sin(w_rad) * Math.Cos(Omega_rad) * Math.Cos(I)) * x_orb +
                           (-Math.Sin(w_rad) * Math.Sin(Omega_rad) + Math.Cos(w_rad) * Math.Cos(Omega_rad) * Math.Cos(I)) * y_orb;

            double z_ecl = Math.Sin(w_rad) * Math.Sin(I) * x_orb + Math.Cos(w_rad) * Math.Sin(I) * y_orb;

            return (x_ecl, y_ecl, z_ecl);
        }

        /// <summary>
        /// Solves Kepler's equation ($M = E - e \cdot \sin(E)$) for the Eccentric Anomaly (E)
        /// using the iterative Newton-Raphson method.
        /// </summary>
        private static double SolveKeplerEquation(double M, double e, double tolerance = 1e-8)
        {
            double E = M;
            for (int i = 0; i < 30; i++)
            {
                double dE = (E - e * Math.Sin(E) - M) / (1 - e * Math.Cos(E));
                E -= dE;
                if (Math.Abs(dE) < tolerance)
                    break;
            }
            return E;
        }

        /// <summary>
        /// Converts rectangular ecliptic coordinates (X, Y, Z) to spherical equatorial coordinates 
        /// (Right Ascension (RA) and Declination (Dec)).
        /// </summary>
        /// <returns>A tuple containing (RA in degrees, Dec in degrees).</returns>
        public static (double RA, double Dec) ConvertToEquatorial(double x, double y, double z)
        {
            // J2000.0 obliquity of the ecliptic
            double obliquity = 23.43928 * Math.PI / 180.0;

            // Rotation of coordinates from Ecliptic plane to Equatorial plane
            double x_eq = x;
            double y_eq = y * Math.Cos(obliquity) - z * Math.Sin(obliquity);
            double z_eq = y * Math.Sin(obliquity) + z * Math.Cos(obliquity);

            // Calculate Right Ascension (RA)
            double ra = Math.Atan2(y_eq, x_eq) * 180.0 / Math.PI;
            if (ra < 0) ra += 360.0;

            // Calculate Declination (Dec)
            double distance = Math.Sqrt(x_eq * x_eq + y_eq * y_eq + z_eq * z_eq);
            double dec = Math.Asin(z_eq / distance) * 180.0 / Math.PI;

            return (ra, dec);
        }

        /// <summary>
        /// Converts a UTC DateTime object to a Julian Date (JD).
        /// </summary>
        private static double ToJulianDate(DateTime utcTime)
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
    }
}