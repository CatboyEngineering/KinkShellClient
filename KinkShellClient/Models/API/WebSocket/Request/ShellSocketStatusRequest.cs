using CatboyEngineering.KinkShellClient.Toy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Models.API.WebSocket.Request
{
    public struct ShellSocketStatusRequest
    {
        public Guid ShellID { get; set; }
        public string CommandName { get; set; }
        public Guid CommandInstanceID { get; set; }
        public ShellSocketCommandStatus Status { get; set; }
    }
}
