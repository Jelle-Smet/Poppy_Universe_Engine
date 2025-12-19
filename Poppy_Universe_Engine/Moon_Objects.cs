using System;
using System.Text.Json.Serialization;

namespace Poppy_Universe_Engine
{
    public class Moon_Objects
    {
        // ═══════════════════════════════════════════════════════════════
        // BASIC PHYSICAL PROPERTIES 
        // ═══════════════════════════════════════════════════════════════
        public string Id { get; set; }
        public string Name { get; set; }
        public string Parent { get; set; }
        public string Color { get; set; }

        // 🛡️ Added '?' to all doubles to handle NULLs from DB
        public double? Diameter { get; set; }
        public double? Mass { get; set; }
        public double? SurfaceTemperature { get; set; }
        public string Composition { get; set; }
        public string SurfaceFeatures { get; set; }

        public double? Magnitude { get; set; }
        public double? DistanceFromEarth { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // EXTRA PHYSICAL PROPERTIES 
        // ═══════════════════════════════════════════════════════════════
        public double? Density { get; set; }
        public double? SurfaceGravity { get; set; }
        public double? EscapeVelocity { get; set; }
        public double? RotationPeriod { get; set; }
        public int? NumberOfRings { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // ORBITAL ELEMENTS 
        // ═══════════════════════════════════════════════════════════════
        public double? SemiMajorAxisKm { get; set; }
        public double? Eccentricity { get; set; }

        // ✨ FIXED: This was the property crashing at index [53]
        public double? Inclination { get; set; }

        public double? LongitudeOfAscendingNode { get; set; }
        public double? ArgumentOfPeriapsis { get; set; }
        public double? MeanAnomalyAtEpoch { get; set; }
        public double? MeanMotion { get; set; }
        public double? OrbitalPeriod { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // CONSTRUCTOR
        // ═══════════════════════════════════════════════════════════════
        public Moon_Objects()
        {
            Name = string.Empty;
            Parent = string.Empty;
            Color = string.Empty;
            Composition = string.Empty;
            SurfaceFeatures = string.Empty;
        }

        public void CalculateMeanMotion()
        {
            if (OrbitalPeriod.HasValue && OrbitalPeriod.Value > 0)
            {
                MeanMotion = 360.0 / OrbitalPeriod.Value;
            }
        }

        public void CalculateOrbitalPeriod(double parentMassKg)
        {
            if (SemiMajorAxisKm.HasValue && SemiMajorAxisKm.Value > 0 && parentMassKg > 0)
            {
                const double G = 6.67430e-11;
                double a_meters = SemiMajorAxisKm.Value * 1000.0;
                double periodSeconds = 2 * Math.PI * Math.Sqrt(Math.Pow(a_meters, 3) / (G * parentMassKg));
                OrbitalPeriod = periodSeconds / 86400.0;
            }
        }
    }
}