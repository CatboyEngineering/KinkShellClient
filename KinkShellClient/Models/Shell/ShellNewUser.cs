using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Models.Shell
{
    public struct ShellNewUser
    {
        public Guid UserID { get; set; }
        public bool SendCommands { get; set; }
    }
}
