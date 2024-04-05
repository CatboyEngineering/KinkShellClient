using System;
using System.Numerics;

namespace CatboyEngineering.KinkShellClient.Models.API.WebSocket.Request
{
    public struct ShellSocketTextMessageRequest
    {
        public Guid ShellID { get; set; }
        public DateTime DateTime { get; set; }
        public string MessageText { get; set; }
        public Vector4 TextColor { get; set; }
    }
}
