﻿using CatboyEngineering.KinkShellClient.Models;
using CatboyEngineering.KinkShellClient.Models.Toy;
using CatboyEngineering.KinkShellClient.ShellData;
using CatboyEngineering.KinkShellClient.Toy;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Windows.Utilities
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

        public static async Task SendCommand(Plugin plugin, ShellSession session, List<Guid> targets, StoredShellCommand storedShellCommand)
        {
            await plugin.ConnectionHandler.SendShellCommand(session, targets, storedShellCommand);
        }

        public static async Task DisconnectFromShellWebSocket(Plugin plugin, KinkShell kinkShell)
        {
            await plugin.ConnectionHandler.CloseConnection(kinkShell);
        }

        public static string[] GetListOfUsers(ShellSession session)
        {
            var userList = session.ConnectedUsers.Select(cu => cu.DisplayName).ToList();

            userList.Insert(0, "Everyone");

            return userList.ToArray();
        }

        public static List<Guid> GetTargetList(int selected, string[] list, ShellSession session)
        {
            if(selected == 0)
            {
                return session.ConnectedUsers.Select(cu => cu.AccountID).ToList();
            }
            else
            {
                return new List<Guid> { session.ConnectedUsers.Find(cu => cu.DisplayName.Equals(list[selected])).AccountID };
            }
        }

        public static List<StoredShellCommand> GetAvailableShellCommands(Plugin plugin)
        {
            var list = new List<StoredShellCommand>();

            list.AddRange(plugin.Configuration.SavedPatterns);
            list.Add(DefaultPatterns.Ripple);
            list.Add(DefaultPatterns.Shockwave);

            return list;
        }
    }
}
