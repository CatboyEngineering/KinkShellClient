﻿using Dalamud.Interface.Windowing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CatboyEngineering.KinkShellClient.Models;
using CatboyEngineering.KinkShellClient.Models.Shell;
using CatboyEngineering.KinkShellClient.Windows.States;

namespace CatboyEngineering.KinkShellClient.Windows.Utilities
{
    public class MainWindowUtilities
    {
        public static async Task HandleWithIndicator(MainWindowState state, Task task, int delay = 0)
        {
            state.isRequestInFlight = true;

            await task.ContinueWith(t =>
            {
                Task.Delay(delay).ContinueWith(t =>
                {
                    state.isRequestInFlight = false;
                });
            });
        }

        public static async Task LogInAndRetrieve(Plugin plugin, MainWindow window)
        {
            var result = await plugin.ConnectionHandler.Authenticate();

            if(result == HttpStatusCode.OK)
            {
                await GetUserShells(plugin, window);
                window.State.IsAuthenticated = true;
            }
            else
            {
                window.State.OnError("Incorrect login or outdated client");
            }
        }

        public static async Task LogOut(Plugin plugin, MainWindow window)
        {
            window.State.SetDefauts();

            var result = await plugin.ConnectionHandler.LogOut();

            if (result == HttpStatusCode.OK)
            {
                window.State.IsAuthenticated = false;
            }
            else
            {
                FixStateIfUnauthenticated(result, window);
            }
        }

        public static async Task GetUserShells(Plugin plugin, MainWindow window)
        {
            window.State.ClearErrors();

            var result = await plugin.ConnectionHandler.GetKinkShells();

            if (result != HttpStatusCode.OK)
            {
                window.State.OnError("Error retrieving Kinkshell list");
                FixStateIfUnauthenticated(result, window);
            }
        }

        public static async Task CreateShell(Plugin plugin, MainWindow window, string name)
        {
            window.State.ClearErrors();

            var result = await plugin.ConnectionHandler.CreateShell(name);

            if (result != HttpStatusCode.Created)
            {
                if(result == HttpStatusCode.PaymentRequired)
                {
                    window.State.OnError("You cannot create any more shells!");
                }
                else
                {
                    window.State.OnError("Error creating a new shell");
                    FixStateIfUnauthenticated(result, window);
                }
            }
        }

        public static async Task DeleteLeaveShell(Plugin plugin, MainWindow window, KinkShell kinkShell)
        {
            window.State.ClearErrors();

            var result = await plugin.ConnectionHandler.DeleteShell(kinkShell.ShellID);

            if (result != HttpStatusCode.OK)
            {
                window.State.OnError("Error removing the shell");
                FixStateIfUnauthenticated(result, window);
            }
        }

        public static async Task UpdateShellUsers(Plugin plugin, MainWindow window, Guid shell, List<ShellNewUser> users)
        {
            window.State.ClearErrors();

            var result = await plugin.ConnectionHandler.UpdateShell(shell, users);

            if (result != HttpStatusCode.OK)
            {
                window.State.OnError("Error updating the shell");
                FixStateIfUnauthenticated(result, window);
            }
        }

        public static void LaunchShellWindow(Plugin plugin, MainWindow window, KinkShell kinkShell)
        {
            var session = plugin.ConnectionHandler.CreateShellSession(kinkShell);
            var shellWindow = plugin.UIHandler.CreateShellWindow(session);

            _ = ShellWindowUtilities.Launch(plugin, shellWindow);
        }

        private static void FixStateIfUnauthenticated(HttpStatusCode statusCode, MainWindow mainWindow)
        {
            if(statusCode == HttpStatusCode.Unauthorized)
            {
                mainWindow.State.SetDefauts();
            }
        }
    }
}
