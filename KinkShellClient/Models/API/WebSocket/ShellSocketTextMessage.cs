using CatboyEngineering.KinkShellClient.ShellData;
using System;
using System.Collections.Generic;

namespace CatboyEngineering.KinkShellClient.Models.API.WebSocket
{
    public struct ShellSocketTextMessage
    {
        public Guid ShellID { get; set; }
        public DateTime DateTime { get; set; }
        public string MessageText { get; set; }
    }
}
