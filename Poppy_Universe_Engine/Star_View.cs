using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Poppy_Universe_Engine
{
    // A view object of the star for recommendations
    public class Star_View
    {
        public Star_Objects Star { get; set; }  // All original star info
        public int Id { get; set; }
        public int Source { get; set; }
        public double Altitude { get; set; }  // Altitude in degrees
        public double Azimuth { get; set; }   // Azimuth in degrees
        public bool IsVisible { get; set; }   // Is the star above the horizon?
        public double Score { get; set; } // for recommendation scoring
        public double MatchPercentage { get; set; } // Match % for user, 0-100
        public double VisibilityChance { get; set; } // The chance of visibility taking weather conditions into account.
        public string ChanceReason { get; set; }    // Explanation of the visibility chance
        public string BoostDescription { get; set; }
        public string SpectralType { get; set; }

    }
}
