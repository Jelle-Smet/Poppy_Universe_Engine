using System;
using System.Text.Json.Serialization;

namespace Poppy_Universe_Engine
{
    /// <summary>
    /// Data model representing a single planet or major solar system body. 
    /// Updated to handle nullable database values and JSON deserialization.
    /// </summary>
    public class Planet_Objects
    {
        // ═══════════════════════════════════════════════════════════════
        // BASIC PHYSICAL PROPERTIES (Nullable for DB compatibility)
        // ═══════════════════════════════════════════════════════════════
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }

        public double? DistanceFromSun { get; set; }
        public double? DistanceFromEarth { get; set; }
        public double? Diameter { get; set; }
        public double? Mass { get; set; }
        public double? MeanTemperature { get; set; }
        public int? NumberOfMoons { get; set; }

        // Handled by BoolConverter
        public bool? HasRings { get; set; }
        public bool? HasMagneticField { get; set; }

        public double? Magnitude { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // ORBITAL ELEMENTS (Required for Ephemeris Calculations)
        // ═══════════════════════════════════════════════════════════════

        public double? SemiMajorAxis { get; set; }
        public double? Eccentricity { get; set; }
        public double? OrbitalInclination { get; set; }
        public double? LongitudeOfAscendingNode { get; set; }
        public double? ArgumentOfPeriapsis { get; set; }
        public double? MeanAnomalyAtEpoch { get; set; }
        public double? MeanMotion { get; set; }
        public double? OrbitalPeriod { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // LEGACY PROPERTY ALIASES (Ignored by JSON to prevent conflicts)
        // ═══════════════════════════════════════════════════════════════

        [JsonIgnore]
        public double? SemiMajorAxisAU
        {
            get => SemiMajorAxis;
            set => SemiMajorAxis = value;
        }

        [JsonIgnore]
        public double? LongitudeAscendingNode
        {
            get => LongitudeOfAscendingNode;
            set => LongitudeOfAscendingNode = value;
        }

        [JsonIgnore]
        public double? ArgumentPeriapsis
        {
            get => ArgumentOfPeriapsis;
            set => ArgumentOfPeriapsis = value;
        }

        [JsonIgnore]
        public double? MeanAnomaly
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

        public void CalculateMeanMotion()
        {
            if (OrbitalPeriod.HasValue && OrbitalPeriod.Value > 0)
            {
                MeanMotion = 360.0 / OrbitalPeriod.Value;
            }
        }

        public void CalculateOrbitalPeriod()
        {
            if (SemiMajorAxis.HasValue && SemiMajorAxis.Value > 0)
            {
                double periodYears = Math.Pow(SemiMajorAxis.Value, 1.5);
                OrbitalPeriod = periodYears * 365.25;
            }
        }
    }
}