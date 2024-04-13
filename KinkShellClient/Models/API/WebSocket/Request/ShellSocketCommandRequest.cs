using CatboyEngineering.KinkShellClient.Models.Toy;
using System;
using System.Collections.Generic;

namespace CatboyEngineering.KinkShellClient.Models.API.WebSocket.Request
{
    public struct ShellSocketCommandRequest
    {
        public Guid ShellID { get; set; }
        public Guid Target { get; set; }
        public Guid ToyID { get; set; }
        public ShellCommand Command { get; set; }
    }
}
