using System;

namespace Poppy_Universe_Engine
{
    /// <summary>
    /// More accurate planetary position calculations using VSOP87-style formulas
    /// These use time-varying orbital elements for better accuracy
    /// </summary>
    public class Planetary_Position_Service
    {
        // Orbital elements at J2000.0 epoch with century rates
        private class OrbitalElements
        {
            public double a, a_cy;      // Semi-major axis (AU) and century rate
            public double e, e_cy;      // Eccentricity and century rate
            public double I, I_cy;      // Inclination (deg) and century rate
            public double L, L_cy;      // Mean longitude (deg) and century rate
            public double w_bar, w_bar_cy; // Longitude of perihelion (deg) and century rate
            public double Omega, Omega_cy; // Longitude of ascending node (deg) and century rate
        }

        private static OrbitalElements GetPlanetElements(string planetName)
        {
            // Source: NASA JPL Planetary Ephemerides
            switch (planetName.ToLower())
            {
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
                default:
                    throw new ArgumentException($"Unknown planet: {planetName}");
            }
        }

        public static (double X, double Y, double Z) CalculateHeliocentricPosition(string planetName, DateTime utcTime)
        {
            double jd = ToJulianDate(utcTime);
            double T = (jd - 2451545.0) / 36525.0; // Julian centuries from J2000.0

            var elem = GetPlanetElements(planetName);

            // Compute current orbital elements
            double a = elem.a + elem.a_cy * T;
            double e = elem.e + elem.e_cy * T;
            double I = (elem.I + elem.I_cy * T) * Math.PI / 180.0;
            double L = (elem.L + elem.L_cy * T) % 360.0;
            if (L < 0) L += 360.0;
            double w_bar = (elem.w_bar + elem.w_bar_cy * T) % 360.0;
            if (w_bar < 0) w_bar += 360.0;
            double Omega = (elem.Omega + elem.Omega_cy * T) % 360.0;
            if (Omega < 0) Omega += 360.0;

            // Calculate argument of perihelion and mean anomaly
            double w = w_bar - Omega; // argument of perihelion
            double M = L - w_bar;     // mean anomaly
            if (M < 0) M += 360.0;

            // Convert to radians
            double M_rad = M * Math.PI / 180.0;
            double w_rad = w * Math.PI / 180.0;
            double Omega_rad = Omega * Math.PI / 180.0;

            // Solve Kepler's equation
            double E = SolveKeplerEquation(M_rad, e);

            // True anomaly
            double nu = 2 * Math.Atan2(
                Math.Sqrt(1 + e) * Math.Sin(E / 2),
                Math.Sqrt(1 - e) * Math.Cos(E / 2)
            );

            // Heliocentric distance
            double r = a * (1 - e * Math.Cos(E));

            // Position in orbital plane
            double x_orb = r * Math.Cos(nu);
            double y_orb = r * Math.Sin(nu);

            // Rotate to ecliptic coordinates
            double x_ecl = (Math.Cos(w_rad) * Math.Cos(Omega_rad) - Math.Sin(w_rad) * Math.Sin(Omega_rad) * Math.Cos(I)) * x_orb +
                          (-Math.Sin(w_rad) * Math.Cos(Omega_rad) - Math.Cos(w_rad) * Math.Sin(Omega_rad) * Math.Cos(I)) * y_orb;

            double y_ecl = (Math.Cos(w_rad) * Math.Sin(Omega_rad) + Math.Sin(w_rad) * Math.Cos(Omega_rad) * Math.Cos(I)) * x_orb +
                          (-Math.Sin(w_rad) * Math.Sin(Omega_rad) + Math.Cos(w_rad) * Math.Cos(Omega_rad) * Math.Cos(I)) * y_orb;

            double z_ecl = Math.Sin(w_rad) * Math.Sin(I) * x_orb + Math.Cos(w_rad) * Math.Sin(I) * y_orb;

            return (x_ecl, y_ecl, z_ecl);
        }

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

        public static (double RA, double Dec) ConvertToEquatorial(double x, double y, double z)
        {
            double obliquity = 23.43928 * Math.PI / 180.0;

            double x_eq = x;
            double y_eq = y * Math.Cos(obliquity) - z * Math.Sin(obliquity);
            double z_eq = y * Math.Sin(obliquity) + z * Math.Cos(obliquity);

            double ra = Math.Atan2(y_eq, x_eq) * 180.0 / Math.PI;
            if (ra < 0) ra += 360.0;

            double distance = Math.Sqrt(x_eq * x_eq + y_eq * y_eq + z_eq * z_eq);
            double dec = Math.Asin(z_eq / distance) * 180.0 / Math.PI;

            return (ra, dec);
        }

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