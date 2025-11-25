namespace Poppy_Universe_Engine
{
    internal class Planet_View
    {
        public Planet_Objects Planet { get; set; }
        public int Id { get; set; }
        public double Altitude { get; set; }       // Angle above horizon
        public double Azimuth { get; set; }        // Direction along horizon
        public bool IsVisible { get; set; }
        public double Score { get; set; }
        public double MatchPercentage { get; set; }
        public double Magnitude { get; set; }                 // Reflectivity (0–1), Lower = brighter
        public double DistanceFromSun { get; set; } // 10^6 km (average)
        public double DistanceFromEarth { get; set; } // 10^6 km (average)

        // -------------------------------
        // 3D Geocentric coordinates (in AU)
        // -------------------------------
        public double GeocentricX { get; set; }
        public double GeocentricY { get; set; }
        public double GeocentricZ { get; set; }

        // -------------------------------
        // Sky coordinates for reference
        // -------------------------------
        public double RightAscension { get; set; } // in hours
        public double Declination { get; set; }    // in degrees

        public double VisibilityChance { get; set; } // The chance of visibility taking weather conditions into account.
        public string ChanceReason { get; set; }    // Explanation of the visibility chance
        public string BoostDescription { get; set; }

    }
}
