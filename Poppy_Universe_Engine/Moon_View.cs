using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poppy_Universe_Engine
{
    internal class Moon_View
    {
        public Moon_Objects Moon { get; set; }
        public int Id { get; set; }
        public double Altitude { get; set; }
        public double Azimuth { get; set; }
        public bool IsVisible { get; set; }
        public double Score { get; set; }
        public double MatchPercentage { get; set; }
        public double VisibilityChance { get; set; } // The chance of visibility taking weather conditions into account.
        public string ChanceReason { get; set; }    // Explanation of the visibility chance
        public string BoostDescription { get; set; }

    }
}
