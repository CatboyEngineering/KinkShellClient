using CatboyEngineering.KinkShellClient.ShellData;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace CatboyEngineering.KinkShellClient.Models.API.WebSocket
{
    public struct ShellSocketTextResponse
    {
        public Guid ShellID { get; set; }
        public KinkShellMember UserFrom { get; set; }
        public DateTime DateTime { get; set; }
        public string MessageText { get; set; }
        public Vector4 TextColor { get; set; }
    }
}
