using System;

namespace Poppy_Universe_Engine
{
    /// <summary>
    /// Data model representing a single moon, containing both static physical characteristics 
    /// and dynamic orbital elements required for calculating its position and using in recommendations.
    /// </summary>
    public class Moon_Objects
    {
        // ═══════════════════════════════════════════════════════════════
        // BASIC PHYSICAL PROPERTIES (Used for scoring and identification)
        // ═══════════════════════════════════════════════════════════════
        public int Id { get; set; }                          // Unique identifier
        public string Name { get; set; }                     // Moon's common name
        public string Parent { get; set; }                   // The planet or body the moon orbits
        public string Color { get; set; }                    // Moon's observable color/albedo description
        public double Diameter { get; set; }                 // Diameter in kilometers (km)
        public double Mass { get; set; }                     // Mass in 10^24 kilograms
        public double SurfaceTemperature { get; set; }       // Average surface temperature in degrees Celsius (C)
        public string Composition { get; set; }              // Primary material composition
        public string SurfaceFeatures { get; set; }          // Description of major surface features (e.g., "Volcanoes", "Craters")

        /// <summary>
        /// Apparent magnitude / Reflectivity factor. Lower values indicate a brighter, more visible object.
        /// </summary>
        public double Magnitude { get; set; }

        /// <summary>
        /// Current distance from Earth in Astronomical Units (AU) or a similar unit.
        /// This value is likely calculated dynamically, but stored here for context.
        /// </summary>
        public double DistanceFromEarth { get; set; }


        // ═══════════════════════════════════════════════════════════════
        // EXTRA PHYSICAL PROPERTIES (Optional inputs for detailed analysis/scoring)
        // ═══════════════════════════════════════════════════════════════
        public double? Density { get; set; }            // Density in kg/m^3
        public double? SurfaceGravity { get; set; }     // Surface gravity in m/s^2
        public double? EscapeVelocity { get; set; }     // Escape velocity in km/s
        public double? RotationPeriod { get; set; }     // Rotation period in hours
        public int? NumberOfRings { get; set; }         // Number of rings (if applicable)

        // ═══════════════════════════════════════════════════════════════
        // ORBITAL ELEMENTS (Required for Ephemeris Calculations)
        // 
        // ═══════════════════════════════════════════════════════════════
        public double SemiMajorAxisKm { get; set; }            // 'a' - Half the longest diameter of the orbit in km
        public double Eccentricity { get; set; }               // 'e' - Shape of the orbit (0=circle, <1=ellipse)
        public double Inclination { get; set; }                // 'i' - Tilt of the orbit plane relative to a reference plane (degrees)
        public double LongitudeOfAscendingNode { get; set; }   // 'Ω' - Position where the orbit crosses the reference plane (degrees)
        public double ArgumentOfPeriapsis { get; set; }        // 'ω' - Angle from ascending node to point of closest approach (degrees)
        public double MeanAnomalyAtEpoch { get; set; }         // 'M₀' - Position in orbit at a reference time (epoch) (degrees)
        public double MeanMotion { get; set; }                 // 'n' - Average angular speed (degrees per day)
        public double OrbitalPeriod { get; set; }              // 'T' or 'P' - Time to complete one orbit (days)

        // ═══════════════════════════════════════════════════════════════
        // CONSTRUCTOR AND HELPERS
        // ═══════════════════════════════════════════════════════════════
        public Moon_Objects()
        {
            // Initialize string properties to non-null defaults
            Name = string.Empty;
            Parent = string.Empty;
            Color = string.Empty;
            Composition = string.Empty;
            SurfaceFeatures = string.Empty;
        }

        /// <summary>
        /// Calculates the Mean Motion (average angular speed) in degrees per day 
        /// from the Orbital Period.
        /// </summary>
        public void CalculateMeanMotion()
        {
            if (OrbitalPeriod > 0)
            {
                MeanMotion = 360.0 / OrbitalPeriod; // Degrees per day (n = 360 / T)
            }
        }

        /// <summary>
        /// Calculates the Orbital Period using Kepler's Third Law, 
        /// given the moon's semi-major axis and the parent body's mass.
        /// Formula: $T^2 = \frac{4\pi^2}{G(M_{parent} + M_{moon})} \times a^3$ 
        /// (Simplified to ignore moon mass).
        /// </summary>
        /// <param name="parentMassKg">The mass of the parent planet in kilograms.</param>
        public void CalculateOrbitalPeriod(double parentMassKg)
        {
            if (SemiMajorAxisKm > 0 && parentMassKg > 0)
            {
                const double G = 6.67430e-11; // Gravitational constant $m^3/(kg\cdot s^2)$
                double a_meters = SemiMajorAxisKm * 1000.0; // Convert semi-major axis from km to meters

                // Calculate period T in seconds using T² = (4π²/GM) × a³
                double periodSeconds = 2 * Math.PI * Math.Sqrt(Math.Pow(a_meters, 3) / (G * parentMassKg));

                // Convert to days
                OrbitalPeriod = periodSeconds / 86400.0;
            }
        }
    }
}