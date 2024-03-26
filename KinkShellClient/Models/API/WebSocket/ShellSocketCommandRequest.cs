using CatboyEngineering.KinkShellClient.Models.Toy;
using System;
using System.Collections.Generic;

namespace CatboyEngineering.KinkShellClient.Models.API.WebSocket
{
    public struct ShellSocketCommandRequest
    {
        public Guid ShellID { get; set; }
        public List<Guid> Targets { get; set; }
        public ShellCommand Command { get; set; }
    }
}
