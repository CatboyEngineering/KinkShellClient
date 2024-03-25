using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShell.Models.Toy
{
    public struct Pattern
    {
        public PatternType PatternType { get; set; }
        public double Intensity { get; set; }
        public int Duration { get; set; }
        public int Delay { get; set; }
    }
}
