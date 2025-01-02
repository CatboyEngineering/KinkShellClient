using CatboyEngineering.KinkShellClient.Toy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Models.API.WebSocket.Request
{
    public struct ShellSocketConnectRequest
    {
        public Guid ShellID { get; set; }
        public Vector4 TextColor { get; set; }
        public List<ToyProperties> Toys { get; set; }
    }
}
