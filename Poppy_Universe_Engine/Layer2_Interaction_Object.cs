namespace Poppy_Universe_Engine
{
    /// <summary>
    /// Represents engagement statistics and trending scores for a single astronomical object.
    /// This object is likely used at Layer 2 (Data Processing/Business Logic) of the engine
    /// to prioritize or rank objects based on user interaction trends.
    /// </summary>
    public class Layer2_Interaction_Object
    {
        /// <summary>
        /// The type of object being tracked (e.g., "Star", "Planet", "Moon").
        /// </summary>
        public string Object_Type { get; set; }

        /// <summary>
        /// The unique identifier for the specific astronomical object.
        /// </summary>
        public int Object_ID { get; set; }

        // --- Interaction Metrics ---

        /// <summary>
        /// A composite metric representing the sum of weighted interactions (views, clicks, favorites).
        /// </summary>
        public double total_interactions { get; set; }

        /// <summary>
        /// The raw count of times this object was viewed by users.
        /// </summary>
        public double num_views { get; set; }

        /// <summary>
        /// The raw count of times a user clicked on this object (e.g., to see details).
        /// </summary>
        public double num_clicks { get; set; }

        /// <summary>
        /// The raw count of times this object was added to a user's favorite list.
        /// </summary>
        public double num_favorites { get; set; }

        // --- Output Score ---

        /// <summary>
        /// A calculated score used for ranking, derived from the interaction metrics 
        /// and potentially time decay or other trending factors.
        /// </summary>
        public double trending_score { get; set; }
    }
}