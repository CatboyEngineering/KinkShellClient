using System;
using System.Collections.Generic;

namespace CatboyEngineering.KinkShellClient.Models.API.WebSocket.Response
{
    public struct ShellSocketConnectedUsersResponse
    {
        public Guid ShellID { get; set; }
        public List<KinkShellMember> ConnectedUsers { get; set; }
    }
}
