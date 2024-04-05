using CatboyEngineering.KinkShellClient.Models;
using System.Collections.Generic;
using System.Net.WebSockets;

namespace CatboyEngineering.KinkShellClient.Models.Shell
{
    public class ShellSession
    {
        public KinkShell KinkShell { get; }
        public ClientWebSocket WebSocket { get; set; }
        public List<KinkShellMember> ConnectedUsers { get; }
        public ShellConnectionStatus Status { get; set; }
        public List<ChatMessage> Messages { get; set; }
        public bool ScrollMessages { get; set; }
        public bool SelfUserReceiveCommands { get; set; }

        public ShellSession(KinkShell kinkShell)
        {
            KinkShell = kinkShell;
            Status = ShellConnectionStatus.CLOSED;
            ConnectedUsers = new List<KinkShellMember>();
            Messages = new List<ChatMessage>();
            ScrollMessages = false;
            SelfUserReceiveCommands = true;
        }

        public void ReceivedNewMessage(ChatMessage message)
        {
            ScrollMessages = true;
            Messages.Add(message);
        }
    }
}
