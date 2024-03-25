using Dalamud.Interface.Windowing;
using KinkShellClient.ShellData;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace KinkShellClient.Windows.Utilities
{
    public class ShellWindowUtilities
    {
        public static async Task Launch(Plugin plugin, ShellWindow shellWindow)
        {
            shellWindow.IsOpen = true;

            await plugin.ConnectionHandler.OpenConnection(shellWindow.State.Session);
        }

        public static async Task SendChat(Plugin plugin, ShellSession session, string message)
        {
            await plugin.ConnectionHandler.SendShellChatMessage(session, message);
        }

        public static async Task DisconnectFromShellWebSocket(Plugin plugin, KinkShell kinkShell)
        {
            await plugin.ConnectionHandler.CloseConnection(kinkShell);
        }
    }
}
