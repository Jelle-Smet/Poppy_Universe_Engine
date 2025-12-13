using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poppy_Universe_Engine
{
    /// <summary>
    /// Data structure representing a user's detailed, quantitative preference matrix.
    /// These values (typically 0-10) are used in Layer 3 of the scoring engine 
    /// to boost recommendations for objects matching the user's specific interests.
    /// </summary>
    public class Layer3_User_Matrix_Object
    {
        /// <summary>
        /// The unique identifier for the user to whom this preference matrix belongs.
        /// </summary>
        public int User_ID { get; set; }

        // --- STARS: Spectral Type Preferences ---
        // These fields quantify the user's affinity for stars belonging to the 
        // main stellar spectral classes (O, B, A, F, G, K, M).
        // 
        public double A { get; set; } // Blue-White Stars (e.g., Sirius)
        public double B { get; set; } // Blue Stars (e.g., Rigel)
        public double F { get; set; } // White Stars (e.g., Polaris)
        public double G { get; set; } // Yellow Stars (e.g., The Sun)
        public double K { get; set; } // Orange Stars (e.g., Arcturus)
        public double M { get; set; } // Red Dwarf Stars (most common)
        public double O { get; set; } // Blue Giant Stars (hottest, rarest)

        // --- PLANETS: Planet Category Preferences ---
        // Quantifies the user's affinity for different planetary classifications.
        public double DwarfPlanet { get; set; }
        public double GasGiant { get; set; } // Jupiter, Saturn
        public double IceGiant { get; set; } // Uranus, Neptune
        public double Terrestrial { get; set; } // Mercury, Venus, Earth, Mars

        // --- MOONS: Parent Planet Preferences ---
        // Quantifies the user's affinity for the moon systems associated with specific parent bodies.
        public double Earth { get; set; } // Preference for the Moon
        public double Eris { get; set; }
        public double Haumea { get; set; }
        public double Jupiter { get; set; } // Preference for Galilean Moons (Io, Europa, etc.)
        public double Makemake { get; set; }
        public double Mars { get; set; } // Preference for Phobos and Deimos
        public double Neptune { get; set; } // Preference for Triton
        public double Pluto { get; set; } // Preference for Charon
        public double Saturn { get; set; } // Preference for Titan, Enceladus, etc.
        public double Uranus { get; set; }
    }
}