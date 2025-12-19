using System;
using System.Text.Json.Serialization;

namespace Poppy_Universe_Engine
{
    public class Star_Objects
    {
        // ═══════════════════════════════════════════════════════════════
        // IDENTIFICATION & ASTROMETRIC DATA
        // ═══════════════════════════════════════════════════════════════

        // Changed to string to be safe with large database IDs or UUIDs
        public string Id { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }

        public double? RA_ICRS { get; set; } // Nullable in case of missing coordinates
        public double? DE_ICRS { get; set; }
        public double? Gmag { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // COLOR & PHOTOMETRIC DATA 
        // ═══════════════════════════════════════════════════════════════

        public double? BPmag { get; set; }
        public double? RPmag { get; set; }

        [JsonIgnore] // C# calculates this, it's not in the JSON input
        public double? ColorIndexBP_RP => BPmag.HasValue && RPmag.HasValue ? BPmag.Value - RPmag.Value : null;

        // ═══════════════════════════════════════════════════════════════
        // PHYSICAL PROPERTIES
        // ═══════════════════════════════════════════════════════════════

        public double? Parallax { get; set; }

        [JsonIgnore] // C# calculates this
        public double? DistancePc => (Parallax.HasValue && Parallax.Value > 0) ? 1000.0 / Parallax.Value : null;

        public string SpectralType { get; set; }
        public double? Teff { get; set; }
        public double? Luminosity { get; set; }
        public double? Mass { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // FLAGS / EXTRA INFO (Booleans handled by your new BoolConverter)
        // ═══════════════════════════════════════════════════════════════
        public bool? IsBinary { get; set; }
        public bool? HasPlanetCandidates { get; set; }
    }
}