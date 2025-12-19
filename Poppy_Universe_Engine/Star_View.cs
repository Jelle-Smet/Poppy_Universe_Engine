using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Poppy_Universe_Engine
{
    /// <summary>
    /// Represents the contextual view and calculated recommendation score for a single star 
    /// at the observer's location and time. This object is used for presentation and ranking.
    /// </summary>
    public class Star_View
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE DATA & IDENTIFICATION
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// A reference to the underlying static data object for the star.
        /// </summary>
        public Star_Objects Star { get; set; }

        /// <summary>
        /// Unique system identifier of the star.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Unique source catalog identifier (e.g., Gaia ID).
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Spectral Type (e.g., G2V) copied for convenience in scoring layers.
        /// </summary>
        public string SpectralType { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // DYNAMIC OBSERVATION DATA (Time/Location Dependent)
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Calculated angular height above the horizon (degrees, 0-90).
        /// </summary>
        public double Altitude { get; set; }

        /// <summary>
        /// Calculated compass bearing (degrees, 0-360, North=0) .
        /// </summary>
        public double Azimuth { get; set; }

        /// <summary>
        /// True if the star is above the minimum visibility altitude.
        /// </summary>
        public bool IsVisible { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // RECOMMENDATION SCORES
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// The final aggregated score from the recommendation engine (L1-L4).
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// The score normalized as a percentage match against the theoretical maximum score (0-100%).
        /// </summary>
        public double MatchPercentage { get; set; }

        /// <summary>
        /// The calculated chance of visibility taking local weather conditions into account (0.0 to 100.0).
        /// </summary>
        public double VisibilityChance { get; set; }

        /// <summary>
        /// Explanation of the visibility chance (e.g., "Clear Skies," "High Humidity").
        /// </summary>
        public string ChanceReason { get; set; }

        /// <summary>
        /// Description of any score boost applied by higher layers (e.g., "NN Boosted by 12%").
        /// </summary>
        public string BoostDescription { get; set; }

    }
}