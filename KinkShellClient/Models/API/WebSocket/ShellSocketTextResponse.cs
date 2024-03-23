using KinkShellClient.ShellData;
using System;
using System.Collections.Generic;

namespace KinkShellClient.Models.API.WebSocket
{
    public struct ShellSocketConnectResponse
    {
        public Guid ShellID { get; set; }
        public List<KinkShellMember> ConnectedUsers { get; set; }
    }
}
