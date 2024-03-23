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
        public static async Task ConnectToShellWebSocket(Plugin plugin, ShellWindow window, ShellSession shellSession)
        {
            await plugin.ConnectionHandler.OpenConnection(shellSession);
        }

        public static async Task DisconnectFromShellWebSocket(Plugin plugin, KinkShell kinkShell)
        {
            await plugin.ConnectionHandler.CloseConnection(kinkShell);
        }
    }
}
