using KinkShellClient.Models;
using System.Net.WebSockets;

namespace KinkShellClient.ShellData
{
    public class ShellSession
    {
        // An individual connection from this client to a KinkShell on the server (WebSocket)
        public KinkShell KinkShell { get; init; }
        public ClientWebSocket? WebSocket { get; set; } // TODO this will be our socket connection
        public ConnectionStatus Status { get; set; }

        public ShellSession(KinkShell kinkShell)
        {
            KinkShell = kinkShell;
            Status = ConnectionStatus.CLOSED;
        }
    }
}
