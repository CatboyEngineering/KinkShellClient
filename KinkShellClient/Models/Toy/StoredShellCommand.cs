using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShell.Models.Toy
{
    public struct StoredShellCommand
    {
        public string Name { get; set; }
        public List<Pattern> Instructions { get; set; }
    }
}
