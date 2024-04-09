using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Models.Toy
{
    public struct StoredShellCommand
    {
        public string Name { get; set; }
        public List<Pattern> Instructions { get; set; }

        public bool UsesConstrict()
        {
            return Instructions.Any(p => p.PatternType == PatternType.CONSTRICT);
        }

        public bool UsesInflate()
        {
            return Instructions.Any(p => p.PatternType == PatternType.INFLATE);
        }

        public bool UsesLinear()
        {
            return Instructions.Any(p => p.PatternType == PatternType.LINEAR);
        }

        public bool UsesOscillate()
        {
            return Instructions.Any(p => p.PatternType == PatternType.OSCILLATE);
        }

        public bool UsesRotate()
        {
            return Instructions.Any(p => p.PatternType == PatternType.ROTATE);
        }

        public bool UsesVibrate()
        {
            return Instructions.Any(p => p.PatternType == PatternType.VIBRATE);
        }
    }
}
