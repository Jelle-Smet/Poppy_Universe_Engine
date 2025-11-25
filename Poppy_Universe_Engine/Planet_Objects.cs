using System;

namespace Poppy_Universe_Engine
{
    public class Planet_Objects
    {
        // Basic Properties
        public int Id { get; set; }                             // ID
        public string Name { get; set; }
        public string Color { get; set; }
        public double DistanceFromSun { get; set; } // 10^6 km (average)
        public double DistanceFromEarth { get; set; } // 10^6 km (average)
        public double Diameter { get; set; }        // km
        public double Mass { get; set; }            // 10^24 kg
        public double MeanTemperature { get; set; } // °C
        public int NumberOfMoons { get; set; }
        public bool HasRings { get; set; }
        public bool HasMagneticField { get; set; }
        public double Magnitude { get; set; }                 // Reflectivity (0–1), Lower = brighter

        // Orbital Elements (required for position calculations)
        public double SemiMajorAxis { get; set; }           // AU - semi-major axis of orbit
        public double Eccentricity { get; set; }            // dimensionless - orbital eccentricity (0-1)
        public double OrbitalInclination { get; set; }      // degrees - inclination to ecliptic
        public double LongitudeOfAscendingNode { get; set; } // degrees - Ω (Omega)
        public double ArgumentOfPeriapsis { get; set; }     // degrees - ω (omega)
        public double MeanAnomalyAtEpoch { get; set; }      // degrees - M₀ at J2000.0 epoch
        public double MeanMotion { get; set; }              // degrees per day - n

        // Derived/Legacy Properties
        public double OrbitalPeriod { get; set; }           // days - full orbit around Sun

        // Legacy property name support (maps to SemiMajorAxis)
        public double SemiMajorAxisAU
        {
            get => SemiMajorAxis;
            set => SemiMajorAxis = value;
        }

        // Legacy property name support (maps to LongitudeOfAscendingNode)
        public double LongitudeAscendingNode
        {
            get => LongitudeOfAscendingNode;
            set => LongitudeOfAscendingNode = value;
        }

        // Legacy property name support (maps to ArgumentOfPeriapsis)
        public double ArgumentPeriapsis
        {
            get => ArgumentOfPeriapsis;
            set => ArgumentOfPeriapsis = value;
        }

        // Legacy property name support (maps to MeanAnomalyAtEpoch)
        public double MeanAnomaly
        {
            get => MeanAnomalyAtEpoch;
            set => MeanAnomalyAtEpoch = value;
        }

        // Constructor
        public Planet_Objects()
        {
            Name = string.Empty;
            Color = string.Empty;
        }

        // Helper method to calculate mean motion from orbital period
        public void CalculateMeanMotion()
        {
            if (OrbitalPeriod > 0)
            {
                MeanMotion = 360.0 / OrbitalPeriod; // degrees per day
            }
        }

        // Helper method to calculate orbital period from semi-major axis (Kepler's 3rd law)
        public void CalculateOrbitalPeriod()
        {
            if (SemiMajorAxis > 0)
            {
                // T² = a³ (when T is in years and a is in AU)
                double periodYears = Math.Pow(SemiMajorAxis, 1.5);
                OrbitalPeriod = periodYears * 365.25; // convert to days
            }
        }
    }
}