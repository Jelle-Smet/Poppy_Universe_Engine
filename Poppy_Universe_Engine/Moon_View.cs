using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poppy_Universe_Engine
{
    /// <summary>
    /// Represents the contextual view and calculated recommendation score for a single moon
    /// at the observer's location and time. This object is used for presentation and ranking.
    /// </summary>
    public class Moon_View
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE DATA
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// A reference to the underlying static data object for the moon.
        /// </summary>
        public Moon_Objects Moon { get; set; }

        /// <summary>
        /// Unique system identifier of the moon (copied from Moon_Objects).
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of the parent body (e.g., "Jupiter") the moon orbits.
        /// </summary>
        public string Parent { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // ASTROMETRIC & VISIBILITY DATA (Dynamic, Time/Location Dependent)
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
        /// True if Altitude is greater than or equal to the minimum required altitude.
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
        /// The score normalized as a percentage match against the theoretical maximum score.
        /// </summary>
        public double MatchPercentage { get; set; }

        /// <summary>
        /// The calculated chance of visibility taking local weather conditions into account (0.0 to 100.0).
        /// </summary>
        public double VisibilityChance { get; set; }

        /// <summary>
        /// Explanation of the visibility chance (e.g., "Clear Skies," "Heavy Clouds").
        /// </summary>
        public string ChanceReason { get; set; }

        /// <summary>
        /// Description of any score boost applied by higher layers (e.g., "NN Boosted by 10%").
        /// </summary>
        public string BoostDescription { get; set; }

    }
}