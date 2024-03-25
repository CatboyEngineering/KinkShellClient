using KinkShellClient.ShellData;
using System;
using System.Collections.Generic;

namespace KinkShellClient.Models.API.WebSocket
{
    public struct ShellSocketTextMessage
    {
        public Guid ShellID { get; set; }
        public DateTime DateTime { get; set; }
        public string MessageText { get; set; }
    }
}
