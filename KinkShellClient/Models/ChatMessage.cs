using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Models
{
    public struct ChatMessage
    {
        public string DisplayName { get; set; }
        public DateTime DateTime { get; set; }
        public string Message { get; set; }
        public Vector4 TextColor { get; set; }
    }
}
