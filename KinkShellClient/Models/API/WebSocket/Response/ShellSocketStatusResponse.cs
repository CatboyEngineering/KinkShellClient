using CatboyEngineering.KinkShellClient.Models.Shell;
using System;
using System.Collections.Generic;

namespace CatboyEngineering.KinkShellClient.Models.API.WebSocket
{
    public struct ShellSocketStatusResponse
    {
        public Guid UserID { get; set; }
        public Guid ShellID { get; set; }
        public List<RunningCommand> RunningCommands { get; set; }
    }
}
