using CatboyEngineering.KinkShellClient.Models.Shell;
using CatboyEngineering.KinkShellClient.Toy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Models
{
    public struct KinkShellMember
    {
        public Guid AccountID { get; set; }
        public string DisplayName { get; set; }
        public bool SendCommands { get; set; }
        public Vector4 TextColor { get; set; }
        public List<ToyProperties> Toys { get; set; }
        public List<RunningCommand> RunningCommands { get; set; }
    }
}
