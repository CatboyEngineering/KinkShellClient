using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Models.Shell
{
    public struct RunningCommand
    {
        public string CommandName { get; set; }
        public Guid CommandInstanceID { get; set; }
    }
}
