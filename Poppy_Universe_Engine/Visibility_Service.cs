using System;

namespace Poppy_Universe_Engine
{
    // Service to calculate visibility from a given location
    public class Visibility_Service
    {
        private double latitude;   // Observer latitude in degrees
        private double longitude;  // Observer longitude in degrees

        public Visibility_Service(double lat, double lon)
        {
            latitude = lat;
            longitude = lon;
        }

        /// <summary>
        /// Calculate Altitude and Azimuth for an object at a specific UTC time
        /// Returns values in degrees
        /// </summary>
        public (double Altitude, double Azimuth) CalculateAltAz(Star_Objects star, DateTime utcTime)
        {
            // Convert RA from hours to degrees, then to radians
            double raRad = DegreesToRadians(star.RA_ICRS * 15);

            // Declination in radians
            double decRad = DegreesToRadians(star.DE_ICRS);

            // Observer latitude in radians
            double latRad = DegreesToRadians(latitude);

            // Calculate Local Sidereal Time in radians
            double lstRad = CalculateLocalSiderealTime(utcTime, longitude);

            // Hour Angle in radians: HA = LST - RA
            double ha = lstRad - raRad;

            // -------------------------------
            // Altitude calculation (angle above horizon)
            // sin(Alt) = sin(Dec) * sin(Lat) + cos(Dec) * cos(Lat) * cos(HA)
            // -------------------------------
            double sinAlt = Math.Sin(decRad) * Math.Sin(latRad) + Math.Cos(decRad) * Math.Cos(latRad) * Math.Cos(ha);
            double altRad = Math.Asin(sinAlt); // Altitude in radians

            // -------------------------------
            // Azimuth calculation (direction along horizon)
            // cos(Az) = (sin(Dec) - sin(Alt) * sin(Lat)) / (cos(Alt) * cos(Lat))
            // Correct for hemisphere depending on HA
            // -------------------------------
            double cosAz = (Math.Sin(decRad) - Math.Sin(altRad) * Math.Sin(latRad)) / (Math.Cos(altRad) * Math.Cos(latRad));
            double azRad = Math.Acos(cosAz); // initial Azimuth

            // If HA > 0, adjust Azimuth to full 0-360 range
            if (Math.Sin(ha) > 0)
                azRad = 2 * Math.PI - azRad;

            // Convert Alt/Az to degrees and return
            return (RadiansToDegrees(altRad), RadiansToDegrees(azRad));
        }

        /// <summary>
        /// Returns true if the star is above the horizon (Altitude >= minAltitude)
        /// </summary>
        public bool IsVisible(Star_Objects star, DateTime utcTime, double minAltitude = 0)
        {
            var (alt, _) = CalculateAltAz(star, utcTime);
            return alt >= minAltitude;
        }

        // -------------------------------
        // Helper functions
        // -------------------------------

        private double DegreesToRadians(double deg) => deg * Math.PI / 180;
        private double RadiansToDegrees(double rad) => rad * 180 / Math.PI;

        /// <summary>
        /// Calculate Local Sidereal Time (LST) in radians
        /// LST is used to convert RA/Dec to Alt/Az for observer's longitude
        /// </summary>
        private double CalculateLocalSiderealTime(DateTime utcTime, double longitude)
        {
            double JD = GetJulianDay(utcTime);  // Julian Day
            double D = JD - 2451545.0;          // Days since J2000.0

            // GMST formula (in degrees)
            double GMST = 280.46061837 + 360.98564736629 * D;

            // Local Sidereal Time (LST) in degrees
            double LST = GMST + longitude;
            LST = LST % 360;
            if (LST < 0) LST += 360;

            return DegreesToRadians(LST);
        }

        /// <summary>
        /// Convert DateTime to Julian Day
        /// </summary>
        private double GetJulianDay(DateTime utcTime)
        {
            int Y = utcTime.Year;
            int M = utcTime.Month;
            double D = utcTime.Day + utcTime.Hour / 24.0 + utcTime.Minute / 1440.0 + utcTime.Second / 86400.0;

            // Adjust months for January and February
            if (M <= 2)
            {
                Y -= 1;
                M += 12;
            }

            int A = Y / 100;
            int B = 2 - A + (A / 4);

            // Standard Julian Day formula
            double JD = Math.Floor(365.25 * (Y + 4716)) +
                        Math.Floor(30.6001 * (M + 1)) +
                        D + B - 1524.5;

            return JD;
        }
    }
}
