using System;

namespace Poppy_Universe_Engine
{
    /// <summary>
    /// Data model representing a single star, containing essential data for visibility 
    /// calculation (ICRS coordinates) and recommendation scoring (brightness, color, spectral type).
    /// </summary>
    public class Star_Objects
    {
        // ═══════════════════════════════════════════════════════════════
        // IDENTIFICATION & ASTROMETRIC DATA
        // ═══════════════════════════════════════════════════════════════
        public int Id { get; set; }                             // System ID
        public string Name { get; set; }        // Common name, if available
        public int Source { get; set; }      // Unique source catalog identifier (e.g., Hipparcos, Gaia ID)

        /// <summary>
        /// Right Ascension (RA) in the International Celestial Reference System (ICRS), measured in hours (0-24).
        /// Used with DE_ICRS to determine sky position.
        /// </summary>
        public double RA_ICRS { get; set; }

        /// <summary>
        /// Declination (Dec) in the ICRS, measured in degrees (-90 to +90).
        /// </summary>
        public double DE_ICRS { get; set; }

        /// <summary>
        /// Apparent magnitude in the Gaia G-band (visual brightness from Earth). Lower = brighter.
        /// </summary>
        public double Gmag { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // COLOR & PHOTOMETRIC DATA (Used heavily in recommendation scoring)
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Apparent magnitude in the Blue Photometric band (BPmag).
        /// </summary>
        public double? BPmag { get; set; }

        /// <summary>
        /// Apparent magnitude in the Red Photometric band (RPmag).
        /// </summary>
        public double? RPmag { get; set; }

        /// <summary>
        /// Computed color index (BP - RP). This value correlates directly with the star's effective temperature and color.
        /// (Negative = bluer/hotter, Positive = redder/cooler).
        /// </summary>
        public double? ColorIndexBP_RP => BPmag.HasValue && RPmag.HasValue ? BPmag.Value - RPmag.Value : null;

        // ═══════════════════════════════════════════════════════════════
        // PHYSICAL PROPERTIES
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Parallax (Plx) in milliarcseconds (mas). Used to determine distance.
        /// </summary>
        public double? Parallax { get; set; }

        /// <summary>
        /// Computed distance from Earth in parsecs (pc). Calculated as $1000 / Parallax$ (where Parallax is in mas).
        /// </summary>
        public double? DistancePc => Parallax.HasValue && Parallax.Value > 0 ? 1000.0 / Parallax.Value : null;

        /// <summary>
        /// Spectral Type (e.g., O, B, A, F, G, K, M). Determines the star's surface temperature and color.
        /// </summary>
        public string SpectralType { get; set; }

        /// <summary>
        /// Effective Temperature (Teff) in Kelvin (K).
        /// </summary>
        public double? Teff { get; set; }

        /// <summary>
        /// Optional: Bolometric luminosity (used for advanced classification).
        /// </summary>
        public double? Luminosity { get; set; }

        /// <summary>
        /// Optional: Stellar mass (in solar masses).
        /// </summary>
        public double? Mass { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // FLAGS / EXTRA INFO
        // ═══════════════════════════════════════════════════════════════
        public bool? IsBinary { get; set; }           // Flag for possible binary system
        public bool? HasPlanetCandidates { get; set; } // Flag for known or suspected exoplanets
    }
}