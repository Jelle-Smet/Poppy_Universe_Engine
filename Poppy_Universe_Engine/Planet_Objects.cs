using System;

namespace Poppy_Universe_Engine
{
    /// <summary>
    /// Data model representing a single planet or major solar system body. 
    /// Contains both static physical characteristics and Keplerian orbital elements.
    /// </summary>
    public class Planet_Objects
    {
        // ═══════════════════════════════════════════════════════════════
        // BASIC PHYSICAL PROPERTIES (Used for scoring and identification)
        // ═══════════════════════════════════════════════════════════════
        public int Id { get; set; }                          // Unique identifier
        public string Name { get; set; }                     // Planet's common name
        public string Type { get; set; }                     // Classification (e.g., "Terrestrial," "Gas Giant")
        public string Color { get; set; }                    // General observable color

        /// <summary>
        /// Average distance from the Sun in millions of kilometers ($10^6$ km).
        /// </summary>
        public double DistanceFromSun { get; set; }

        /// <summary>
        /// Average distance from Earth in millions of kilometers ($10^6$ km).
        /// </summary>
        public double DistanceFromEarth { get; set; }

        public double Diameter { get; set; }                 // Diameter in kilometers (km)
        public double Mass { get; set; }                     // Mass in $10^{24}$ kilograms
        public double MeanTemperature { get; set; }          // Average surface temperature in degrees Celsius (°C)
        public int NumberOfMoons { get; set; }               // Current count of known moons
        public bool HasRings { get; set; }                   // Flag indicating presence of a ring system
        public bool HasMagneticField { get; set; }           // Flag indicating a global magnetic field

        /// <summary>
        /// Apparent magnitude / Reflectivity factor. Lower values indicate a brighter, more visible object.
        /// </summary>
        public double Magnitude { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // ORBITAL ELEMENTS (Required for Ephemeris Calculations)
        // 
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// 'a' - Semi-major axis of the orbit in Astronomical Units (AU).
        /// </summary>
        public double SemiMajorAxis { get; set; }

        /// <summary>
        /// 'e' - Orbital eccentricity (dimensionless, 0=circle).
        /// </summary>
        public double Eccentricity { get; set; }

        /// <summary>
        /// 'i' - Inclination of the orbit plane relative to the ecliptic (degrees).
        /// </summary>
        public double OrbitalInclination { get; set; }

        /// <summary>
        /// 'Ω' - Longitude of the ascending node (degrees).
        /// </summary>
        public double LongitudeOfAscendingNode { get; set; }

        /// <summary>
        /// 'ω' - Argument of periapsis (degrees).
        /// </summary>
        public double ArgumentOfPeriapsis { get; set; }

        /// <summary>
        /// 'M₀' - Mean Anomaly at the J2000.0 epoch (degrees).
        /// </summary>
        public double MeanAnomalyAtEpoch { get; set; }

        /// <summary>
        /// 'n' - Mean Motion (average angular speed in degrees per day).
        /// </summary>
        public double MeanMotion { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // DERIVED/LEGACY PROPERTIES
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// 'T' or 'P' - Time to complete one orbit around the Sun (days).
        /// </summary>
        public double OrbitalPeriod { get; set; }

        // --- Legacy Property Aliases for backward compatibility ---
        public double SemiMajorAxisAU
        {
            get => SemiMajorAxis;
            set => SemiMajorAxis = value;
        }
        public double LongitudeAscendingNode
        {
            get => LongitudeOfAscendingNode;
            set => LongitudeOfAscendingNode = value;
        }

        public double ArgumentPeriapsis
        {
            get => ArgumentOfPeriapsis;
            set => ArgumentOfPeriapsis = value;
        }
        public double MeanAnomaly
        {
            get => MeanAnomalyAtEpoch;
            set => MeanAnomalyAtEpoch = value;
        }

        // ═══════════════════════════════════════════════════════════════
        // CONSTRUCTOR AND HELPERS
        // ═══════════════════════════════════════════════════════════════

        public Planet_Objects()
        {
            Name = string.Empty;
            Color = string.Empty;
        }

        /// <summary>
        /// Calculates the Mean Motion (average angular speed) in degrees per day 
        /// from the Orbital Period (n = 360 / T).
        /// </summary>
        public void CalculateMeanMotion()
        {
            if (OrbitalPeriod > 0)
            {
                MeanMotion = 360.0 / OrbitalPeriod; // degrees per day
            }
        }

        /// <summary>
        /// Calculates the Orbital Period using a simplified form of Kepler's Third Law 
        /// for solar system bodies: $T^2 \approx a^3$.
        /// Assumes Period (T) is in Earth years and Semi-Major Axis (a) is in AU.
        /// </summary>
        public void CalculateOrbitalPeriod()
        {
            if (SemiMajorAxis > 0)
            {
                // T (years) = a^(1.5)
                double periodYears = Math.Pow(SemiMajorAxis, 1.5);
                OrbitalPeriod = periodYears * 365.25; // convert to days
            }
        }
    }
}