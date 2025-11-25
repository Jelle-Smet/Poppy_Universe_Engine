using System;

namespace Poppy_Universe_Engine
{
    public class Moon_Objects
    {
        // Basic Properties
        public int Id { get; set; }                             // ID
        public string Name { get; set; }                        // Name
        public string Parent { get; set; }                      // Parent
        public string Color { get; set; }                       // Color
        public double Diameter { get; set; }                    // Diameter_(km)
        public double Mass { get; set; }                        // Mass_(10^24kg)
        public double SurfaceTemperature { get; set; }          // Surface_Temperature_(C)
        public string Composition { get; set; }                 // Composition
        public string SurfaceFeatures { get; set; }             // Surface_Features
        public double Magnitude { get; set; }                   // Reflectivity, Lower = brighter

        public double DistanceFromEarth { get; set; }


        // Extra properties for recommendations
        public double? Density { get; set; }                // Density_(kg/m^3)
        public double? SurfaceGravity { get; set; }         // Surface_Gravity_(m/s^2)
        public double? EscapeVelocity { get; set; }         // Escape_Velocity_(km/s)
        public double? RotationPeriod { get; set; }         // Rotation_Period_(hours)
        public int? NumberOfRings { get; set; }             // Number_of_Rings

        // Orbital Elements (required for position calculations)
        public double SemiMajorAxisKm { get; set; }         // SemiMajorAxisKm
        public double Eccentricity { get; set; }            // Orbital_Eccentricity
        public double Inclination { get; set; }             // Inclination_(Deg)
        public double LongitudeOfAscendingNode { get; set; } // Longitude of ascending node (not in CSV, optional)
        public double ArgumentOfPeriapsis { get; set; }     // Argument of periapsis (not in CSV, optional)
        public double MeanAnomalyAtEpoch { get; set; }      // Mean anomaly at epoch (not in CSV, optional)
        public double MeanMotion { get; set; }              // degrees per day - n (calculated)
        public double OrbitalPeriod { get; set; }           // Orbital_Period_(days)


        // Constructor
        public Moon_Objects()
        {
            Name = string.Empty;
            Parent = string.Empty;
            Color = string.Empty;
            Composition = string.Empty;
            SurfaceFeatures = string.Empty;
        }

        // Helper method to calculate mean motion from orbital period
        public void CalculateMeanMotion()
        {
            if (OrbitalPeriod > 0)
            {
                MeanMotion = 360.0 / OrbitalPeriod; // degrees per day
            }
        }

        // Helper method to calculate orbital period from semi-major axis and parent planet mass
        // Using Kepler's 3rd law: T² = (4π²/GM) × a³
        public void CalculateOrbitalPeriod(double parentMassKg)
        {
            if (SemiMajorAxisKm > 0 && parentMassKg > 0)
            {
                const double G = 6.67430e-11; // Gravitational constant m³/(kg·s²)
                double a_meters = SemiMajorAxisKm * 1000.0; // Convert km to meters

                // T in seconds
                double periodSeconds = 2 * Math.PI * Math.Sqrt(Math.Pow(a_meters, 3) / (G * parentMassKg));

                // Convert to days
                OrbitalPeriod = periodSeconds / 86400.0;
            }
        }
    }
}