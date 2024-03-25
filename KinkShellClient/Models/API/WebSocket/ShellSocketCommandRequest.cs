using CatboyEngineering.KinkShell.Models.Toy;
using System;
using System.Collections.Generic;

namespace CatboyEngineering.KinkShell.Models.API.WebSocket
{
    public struct ShellSocketCommandRequest
    {
        public Guid ShellID { get; set; }
        public ShellCommand Command { get; set; }
    }
}
