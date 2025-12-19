using System;

namespace Poppy_Universe_Engine
{
    /// <summary>
    /// Service to calculate the local horizontal coordinates (Altitude and Azimuth) 
    /// of a celestial object for an observer at a specific geographical location and time.
    /// </summary>
    public class Visibility_Service
    {
        private double latitude;   // Observer latitude in degrees
        private double longitude;  // Observer longitude in degrees

        /// <summary>
        /// Initializes the service with the observer's location.
        /// </summary>
        public Visibility_Service(double lat, double lon)
        {
            latitude = lat;
            longitude = lon;
        }

        /// <summary>
        /// Calculate Altitude (Alt) and Azimuth (Az) for an object at a specific UTC time.
        /// This involves converting equatorial coordinates (RA/Dec) to local horizontal coordinates (Alt/Az).
        /// Returns values in degrees.
        /// </summary>
        /// <param name="star">The object whose position is to be calculated (contains RA/Dec).</param>
        /// <param name="utcTime">The current observation time in UTC.</param>
        /// <returns>A tuple containing (Altitude in degrees, Azimuth in degrees).</returns>
        public (double Altitude, double Azimuth) CalculateAltAz(Star_Objects star, DateTime utcTime)
        {
            // --- 1. Preparation ---

            // Use ?? 0 to provide a fallback if the database value is null
            double raRad = DegreesToRadians((star.RA_ICRS ?? 0) * 15);

            // Declination (Dec) in radians
            double decRad = DegreesToRadians(star.DE_ICRS ?? 0);

            // Observer latitude in radians
            double latRad = DegreesToRadians(latitude);

            // Calculate Local Sidereal Time (LST) at the observer's longitude (required for time-dependent conversion)
            double lstRad = CalculateLocalSiderealTime(utcTime, longitude);

            // Hour Angle (HA) in radians: HA = LST - RA
            // HA is the angular distance along the celestial equator from the observer's meridian to the object.
            double ha = lstRad - raRad;

            // --- 2. Altitude calculation ---
            // Formula: sin(Alt) = sin(Dec) * sin(Lat) + cos(Dec) * cos(Lat) * cos(HA)
            double sinAlt = Math.Sin(decRad) * Math.Sin(latRad) +
                            Math.Cos(decRad) * Math.Cos(latRad) * Math.Cos(ha);
            double altRad = Math.Asin(sinAlt); // Altitude in radians

            // --- 3. Azimuth calculation ---
            // Formula for cos(Az): cos(Az) = (sin(Dec) - sin(Alt) * sin(Lat)) / (cos(Alt) * cos(Lat))
            double cosAz = (Math.Sin(decRad) - Math.Sin(altRad) * Math.Sin(latRad)) /
                           (Math.Cos(altRad) * Math.Cos(latRad));

            // Clamp cosAz to the [-1, 1] range to avoid Math.Acos domain error due to floating point precision
            if (cosAz > 1.0) cosAz = 1.0;
            if (cosAz < -1.0) cosAz = -1.0;

            double azRad = Math.Acos(cosAz); // initial Azimuth (0 to π)

            // Azimuth correction for hemisphere (0 to 2π)
            // If the object is rising (HA > 0 in the range 0 to π, or sin(HA) > 0), Azimuth needs inversion.
            if (Math.Sin(ha) > 0)
                azRad = 2 * Math.PI - azRad;

            // 

            // --- 4. Final Conversion ---
            return (RadiansToDegrees(altRad), RadiansToDegrees(azRad));
        }

        /// <summary>
        /// Simple check to determine if an object is above the specified minimum altitude.
        /// </summary>
        public bool IsVisible(Star_Objects star, DateTime utcTime, double minAltitude = 0)
        {
            var (alt, _) = CalculateAltAz(star, utcTime);
            return alt >= minAltitude;
        }

        // -------------------------------
        // Helper functions
        // -------------------------------

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        private double DegreesToRadians(double deg) => deg * Math.PI / 180;

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        private double RadiansToDegrees(double rad) => rad * 180 / Math.PI;

        /// <summary>
        /// Calculate Local Sidereal Time (LST) in radians.
        /// LST is the Right Ascension currently on the local meridian.
        /// </summary>
        /// <param name="utcTime">The current time in UTC.</param>
        /// <param name="longitude">The observer's longitude in degrees.</param>
        /// <returns>LST in radians.</returns>
        private double CalculateLocalSiderealTime(DateTime utcTime, double longitude)
        {
            double JD = GetJulianDay(utcTime);  // Julian Day
            double D = JD - 2451545.0;          // Days since J2000.0 epoch

            // GMST (Greenwich Mean Sidereal Time) formula (in degrees)
            // 280.46... is GMST at J2000.0 (0h UT)
            // 360.985... is the rate of change (slightly faster than 360 degrees per solar day)
            double GMST = 280.46061837 + 360.98564736629 * D;

            // Local Sidereal Time (LST) in degrees: LST = GMST + Longitude
            double LST = GMST + longitude;

            // Normalize LST to the 0-360 degree range
            LST = LST % 360;
            if (LST < 0) LST += 360;

            return DegreesToRadians(LST);
        }

        /// <summary>
        /// Converts DateTime to Julian Day (JD) for astronomical calculations.
        /// </summary>
        private double GetJulianDay(DateTime utcTime)
        {
            int Y = utcTime.Year;
            int M = utcTime.Month;
            // Fractional part of the day
            double D = utcTime.Day + utcTime.Hour / 24.0 + utcTime.Minute / 1440.0 + utcTime.Second / 86400.0;

            // Adjust months for astronomical calculation starting point
            if (M <= 2)
            {
                Y -= 1;
                M += 12;
            }

            int A = Y / 100;
            int B = 2 - A + (A / 4);

            // Standard Julian Day formula (accurate for Gregorian calendar)
            double JD = Math.Floor(365.25 * (Y + 4716)) +
                        Math.Floor(30.6001 * (M + 1)) +
                        D + B - 1524.5;

            return JD;
        }
    }
}