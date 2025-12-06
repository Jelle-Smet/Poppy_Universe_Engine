using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poppy_Universe_Engine
{
    internal class User_Object
    {
        public int ID { get; set; }
        public string Name { get; set; }
        // Stars
        public List<string> LikedStars { get; set; } = new List<string>(); // IDs of liked stars
        public List<string> FavoriteSpectralTypes { get; set; } = new List<string>(); // optional later

        // Planets
        public List<string> LikedPlanets { get; set; } = new List<string>();
        public List<string> FavoritePlanetColors { get; set; } = new List<string>(); // optional, can use color as preference

        // Moons
        public List<string> LikedMoons { get; set; } = new List<string>();
        public List<string> FavoriteMoonCompositions { get; set; } = new List<string>(); // optional

        // Optional: location & observation time for personalized visibility checks
        public double Latitude { get; set; } = 0.0;
        public double Longitude { get; set; } = 0.0;
        public DateTime ObservationTime { get; set; } = DateTime.UtcNow;
    }
}
