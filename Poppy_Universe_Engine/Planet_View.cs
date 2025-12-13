using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poppy_Universe_Engine
{
    /// <summary>
    /// Represents the contextual view and calculated recommendation score for a single planet 
    /// at the observer's location and time. This combines static planet data with 
    /// computed dynamic values (position, scores).
    /// </summary>
    public class Planet_View
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE DATA & IDENTIFICATION
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// A reference to the underlying static data object for the planet.
        /// </summary>
        public Planet_Objects Planet { get; set; }

        /// <summary>
        /// Unique system identifier of the planet (copied from Planet_Objects).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Classification of the planet (e.g., "Terrestrial", "Gas Giant").
        /// </summary>
        public string Type { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // DYNAMIC OBSERVATION DATA (Time/Location Dependent)
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Calculated angular height above the horizon (degrees, 0-90).
        /// </summary>
        public double Altitude { get; set; }     // Angle above horizon

        /// <summary>
        /// Calculated compass bearing (degrees, 0-360, North=0) .
        /// </summary>
        public double Azimuth { get; set; }      // Direction along horizon

        /// <summary>
        /// True if the planet is currently visible (Altitude >= minimum threshold).
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Apparent magnitude of the planet (used in L1 scoring). Lower = brighter.
        /// </summary>
        public double Magnitude { get; set; }

        /// <summary>
        /// Current distance from the Sun (usually in $10^6$ km).
        /// </summary>
        public double DistanceFromSun { get; set; }

        /// <summary>
        /// Current distance from the Earth (usually in $10^6$ km).
        /// </summary>
        public double DistanceFromEarth { get; set; }

        // -------------------------------
        // 3D Geocentric coordinates (Earth-centered, often in AU)
        // These are essential for moon position calculations (Layer 1 Moon Engine).
        // -------------------------------
        public double GeocentricX { get; set; }
        public double GeocentricY { get; set; }
        public double GeocentricZ { get; set; }

        // -------------------------------
        // Equatorial Sky Coordinates (Reference for position/tracking)
        // -------------------------------

        /// <summary>
        /// Right Ascension (position along the celestial equator, typically in degrees or hours).
        /// </summary>
        public double RightAscension { get; set; }

        /// <summary>
        /// Declination (angular distance north or south of the celestial equator, in degrees).
        /// </summary>
        public double Declination { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // RECOMMENDATION & SCORING DATA
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// The final aggregated recommendation score (L1 through L4).
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// The score normalized as a percentage match against the theoretical maximum score.
        /// </summary>
        public double MatchPercentage { get; set; }

        /// <summary>
        /// The chance of visibility taking local weather conditions into account (0.0 to 100.0).
        /// </summary>
        public double VisibilityChance { get; set; }

        /// <summary>
        /// Explanation of the visibility chance (e.g., "Clear Skies," "Light Fog").
        /// </summary>
        public string ChanceReason { get; set; }

        /// <summary>
        /// Description of any score boost applied by higher layers (e.g., "NN Boosted by 15%").
        /// </summary>
        public string BoostDescription { get; set; }
    }
}