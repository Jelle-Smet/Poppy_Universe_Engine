using System;

namespace Poppy_Universe_Engine
{
    /// <summary>
    /// Represents a celestial object with rankings from all 4 previous layers.
    /// This object is used as the input and output structure for the Layer 5 
    /// Genetic Algorithm (GA) rank fusion process. 
    /// </summary>
    public class Layer5_Poppys_GA_Object
    {
        // ═══════════════════════════════════════════════════════════════
        // IDENTIFICATION
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// The ID of the user for whom the recommendation was generated.
        /// </summary>
        public int User_ID { get; set; }

        /// <summary>
        /// The unique system ID for the celestial object (used for cross-layer lookup).
        /// </summary>
        public int Object_ID { get; set; }

        /// <summary>
        /// The broad type of the object ("Star", "Planet", or "Moon").
        /// </summary>
        public string Object_Type { get; set; }

        /// <summary>
        /// The common name of the celestial object (for display).
        /// </summary>
        public string Object_Name { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // RANKINGS FROM PREVIOUS LAYERS (Input to the GA)
        // Note: Rank is the 0-based index position in the sorted list (0 = best).
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Rank position derived from Layer 1 (Base Relevance & Visibility).
        /// </summary>
        public int Layer1_Rank { get; set; }

        /// <summary>
        /// Rank position derived from Layer 2 (Trending & Popularity Boost).
        /// </summary>
        public int Layer2_Rank { get; set; }

        /// <summary>
        /// Rank position derived from Layer 3 (Matrix Factorization Personalization).
        /// </summary>
        public int Layer3_Rank { get; set; }

        /// <summary>
        /// Rank position derived from Layer 4 (Neural Network Personalization).
        /// </summary>
        public int Layer4_Rank { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // LAYER 5 OUTPUT (Populated by the GA)
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// The final weighted score computed by the GA (W1*R1 + W2*R2 + ...).
        /// This is an internal consensus score (lower is better, as it represents a rank position).
        /// </summary>
        public double Layer5_FinalScore { get; set; }

        /// <summary>
        /// The final rank position after GA optimization and sorting (0 = best).
        /// </summary>
        public int Layer5_FinalRank { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // DISPLAY INFORMATION (Carried forward from View objects for final presentation)
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// The last calculated Match Percentage (L4/L3 result).
        /// </summary>
        public double MatchPercentage { get; set; }

        /// <summary>
        /// The last calculated raw score (L4/L3 result).
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Whether the object is above the minimum visibility altitude.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Current Altitude above the horizon (degrees).
        /// </summary>
        public double Altitude { get; set; }

        /// <summary>
        /// Current Azimuth (compass bearing) relative to the observer (degrees).
        /// </summary>
        public double Azimuth { get; set; }

        /// <summary>
        /// Local weather-based visibility chance (0-100%).
        /// </summary>
        public int VisibilityChance { get; set; }

        /// <summary>
        /// Reason for the visibility chance (e.g., "Clear Skies").
        /// </summary>
        public string ChanceReason { get; set; }

        /// <summary>
        /// Description of the last applied score boost (e.g., "NN Boosted by 15%").
        /// </summary>
        public string BoostDescription { get; set; }

        // Type-specific properties (ensures necessary data is available for display)
        public string SpectralType { get; set; } // For Stars
        public string Type { get; set; } // For Planets (e.g., Terrestrial, Gas Giant)
        public string Parent { get; set; } // For Moons (e.g., Jupiter, Saturn)
        public string Color { get; set; }
        public double Diameter { get; set; }
        public double Mass { get; set; }
        public string Composition { get; set; }
        public string SurfaceFeatures { get; set; }
        public double Gmag { get; set; } // G-band magnitude (brightness) For Stars

        // ═══════════════════════════════════════════════════════════════
        // HELPER METHODS
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Provides a brief summary showing the object's rankings across all layers.
        /// </summary>
        public override string ToString()
        {
            return $"{Object_Type}: {Object_Name} | Ranks [L1:{Layer1_Rank}, L2:{Layer2_Rank}, L3:{Layer3_Rank}, L4:{Layer4_Rank}] → Final:{Layer5_FinalRank}";
        }
    }
}