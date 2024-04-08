using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Models.Toy
{
    public struct ShellCommand
    {
        public string CommandName { get; set; }
        public Guid CommandInstanceID { get; set; }
        public List<Pattern> Instructions { get; set; }
    }
}
