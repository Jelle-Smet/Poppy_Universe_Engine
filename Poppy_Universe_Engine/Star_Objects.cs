using System;

namespace Poppy_Universe_Engine
{
    // Engine-ready star object
    public class Star_Objects
    {
        public int Id { get; set; }                             // ID
        public string Name { get; set; }        // name, (if no readable name) -> Source 
        public int Source { get; set; }      // Unique star ID 
        public double RA_ICRS { get; set; }     // Right Ascension in hours -> RA_ICRS
        public double DE_ICRS { get; set; }     // Declination in degrees -> DE_ICRS
        public double Gmag { get; set; }        // Gaia G-band magnitude -> Gmag

        // Color info for recommendations
        public double? BPmag { get; set; }      // Blue magnitude -> BPmag
        public double? RPmag { get; set; }      // Red magnitude -> RPmag
        public double? ColorIndexBP_RP => BPmag.HasValue && RPmag.HasValue ? BPmag.Value - RPmag.Value : null; // computed from BPmag - RPmag

        // Physical properties
        public double? Parallax { get; set; }   // in milliarcseconds -> Plx
        public double? DistancePc => Parallax.HasValue && Parallax.Value > 0 ? 1000.0 / Parallax.Value : null; // pc, computed

        public string SpectralType { get; set; } // Spectral type (O, B, A, F, G, K, M etc) -> SpType-ELS
        public double? Teff { get; set; }        // Effective temperature -> Teff

        public double? Luminosity { get; set; } // Optional: could help with scoring -> Lum-Flame
        public double? Mass { get; set; }       // Optional: maybe for future ranking by stellar type -> Mass-Flame

        // Flags / extra info (if needed)
        public bool? IsBinary { get; set; }           // -> Pbin
        public bool? HasPlanetCandidates { get; set; } // -> PQSO
    }
}
