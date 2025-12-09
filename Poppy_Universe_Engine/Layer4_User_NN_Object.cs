using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poppy_Universe_Engine
{
    public class Layer4_User_NN_Object
    {
        public int User_ID { get; set; }

        // STARS: Spectral types
        public double A { get; set; }
        public double B { get; set; }
        public double F { get; set; }
        public double G { get; set; }
        public double K { get; set; }
        public double M { get; set; }
        public double O { get; set; }

        // PLANETS: Planet categories
        public double DwarfPlanet { get; set; }
        public double GasGiant { get; set; }
        public double IceGiant { get; set; }
        public double Terrestrial { get; set; }

        // MOONS: Parent planet
        public double Earth { get; set; }
        public double Eris { get; set; }
        public double Haumea { get; set; }
        public double Jupiter { get; set; }
        public double Makemake { get; set; }
        public double Mars { get; set; }
        public double Neptune { get; set; }
        public double Pluto { get; set; }
        public double Saturn { get; set; }
        public double Uranus { get; set; }
    }
}
