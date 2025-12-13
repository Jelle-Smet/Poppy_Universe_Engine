using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poppy_Universe_Engine
{
    /// <summary>
    /// Data model representing an individual user, storing their unique ID, preferences, 
    /// and current geographical location for personalized visibility checks and scoring.
    /// </summary>
    public class User_Object
    {
        /// <summary>
        /// Unique user identifier.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// User's display name.
        /// </summary>
        public string Name { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // STAR PREFERENCES (Used in Layer 1 and for Matrix/NN feature generation)
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// List of star names or IDs explicitly favorited by the user.
        /// </summary>
        public List<string> LikedStars { get; set; } = new List<string>();

        /// <summary>
        /// List of preferred stellar spectral types (e.g., "G", "M").
        /// </summary>
        public List<string> FavoriteSpectralTypes { get; set; } = new List<string>();

        // ═══════════════════════════════════════════════════════════════
        // PLANET PREFERENCES
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// List of planet names or IDs explicitly favorited by the user.
        /// </summary>
        public List<string> LikedPlanets { get; set; } = new List<string>();

        /// <summary>
        /// List of preferred planet colors.
        /// </summary>
        public List<string> FavoritePlanetColors { get; set; } = new List<string>();

        // ═══════════════════════════════════════════════════════════════
        // MOON PREFERENCES
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// List of moon names or IDs explicitly favorited by the user.
        /// </summary>
        public List<string> LikedMoons { get; set; } = new List<string>();

        /// <summary>
        /// List of preferred moon surface compositions (e.g., "Icy," "Rocky").
        /// </summary>
        public List<string> FavoriteMoonCompositions { get; set; } = new List<string>();

        // ═══════════════════════════════════════════════════════════════
        // LOCATION & TIME (Required for Ephemeris and Visibility Calculations)
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Observer's geographical latitude (degrees).
        /// </summary>
        public double Latitude { get; set; } = 0.0;

        /// <summary>
        /// Observer's geographical longitude (degrees).
        /// </summary>
        public double Longitude { get; set; } = 0.0;

        /// <summary>
        /// The current time set for the observation (used for ephemeris calculation).
        /// Defaults to UTC Now.
        /// </summary>
        public DateTime ObservationTime { get; set; } = DateTime.UtcNow;
    }
}