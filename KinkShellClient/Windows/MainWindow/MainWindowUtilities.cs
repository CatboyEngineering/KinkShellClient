using Dalamud.Interface.Windowing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CatboyEngineering.KinkShellClient.Models;
using CatboyEngineering.KinkShellClient.Models.Shell;
using CatboyEngineering.KinkShellClient.Windows.Utilities;

namespace CatboyEngineering.KinkShellClient.Windows.MainWindow
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

        [Obsolete]
        public static async Task LogInAndRetrieve(Plugin plugin, MainWindow window)
        {
            var result = await plugin.ConnectionHandler.Authenticate();

            if (result == HttpStatusCode.OK)
            {
                await GetUserShells(plugin, window);
                window.State.Screen = MainWindowScreen.HOME;
            }
            else
            {
                HandleAPIError(result, window, "Incorrect login or outdated client");
            }
        }

        public static async Task LogInV1AndMigrate(Plugin plugin, MainWindow window)
        {
            var result = await plugin.ConnectionHandler.AuthenticateV1Migrate();

            if (result == HttpStatusCode.OK)
            {
                window.State.Screen = MainWindowScreen.VERIFY;
            }
            else
            {
                HandleAPIError(result, window);
            }
        }

        public static async Task LogInV2(Plugin plugin, MainWindow window)
        {
            var result = await plugin.ConnectionHandler.AuthenticateV2();


            if (result == HttpStatusCode.OK)
            {
                if (!plugin.Configuration.KinkShellUserData.IsVerified)
                {
                    window.State.Screen = MainWindowScreen.VERIFY;
                }
                else
                {
                    await GetUserShells(plugin, window);
                    _ = plugin.ToyController.Connect();
                    window.State.Screen = MainWindowScreen.HOME;
                }
            }
            else
            {
                HandleAPIError(result, window);
            }
        }

        public static async Task CreateAccount(Plugin plugin, MainWindow window)
        {
            var result = await plugin.ConnectionHandler.CreateAccount();

            if (result == HttpStatusCode.Created)
            {
                window.State.Screen = MainWindowScreen.VERIFY;
            }
            else
            {
                HandleAPIError(result, window, "An account already exists for this player!");
            }
        }

        public static async Task RecoverAccount(Plugin plugin, MainWindow window)
        {
            var result = await plugin.ConnectionHandler.RecoverAccount();

            if (result == HttpStatusCode.Created)
            {
                window.State.Screen = MainWindowScreen.VERIFY_RECOVERY;
            }
            else
            {
                HandleAPIError(result, window);
            }
        }

        public static async Task RecoverAccountVerify(Plugin plugin, MainWindow window)
        {
            var result = await plugin.ConnectionHandler.VerifyCharacterRecovery(plugin.Configuration.RecoveryIntegrityToken);

            if (result == HttpStatusCode.OK)
            {
                await GetUserShells(plugin, window);
                _ = plugin.ToyController.Connect();

                window.State.Screen = MainWindowScreen.HOME;
            }
            else
            {
                HandleAPIError(result, window);
            }
        }

        public static async Task VerifyCharacter(Plugin plugin, MainWindow window)
        {
            var result = await plugin.ConnectionHandler.VerifyCharacter();

            if (result == HttpStatusCode.OK)
            {
                await GetUserShells(plugin, window);
                _ = plugin.ToyController.Connect();
                window.State.Screen = MainWindowScreen.HOME;
            }
            else
            {
                HandleAPIError(result, window, "Verification was not successful.");
            }
        }

        public static async Task LogOut(Plugin plugin, MainWindow window)
        {
            var result = await plugin.ConnectionHandler.LogOut();

            if (result != HttpStatusCode.OK && result != HttpStatusCode.Unauthorized)
            {
                HandleAPIError(result, window);
            }
        }

        public static async Task GetUserShells(Plugin plugin, MainWindow window)
        {
            window.State.ClearErrors();

            var result = await plugin.ConnectionHandler.GetKinkShells();

            if (result != HttpStatusCode.OK)
            {
                HandleAPIError(result, window);
            }
        }

        public static async Task CreateShell(Plugin plugin, MainWindow window, string name)
        {
            window.State.ClearErrors();

            var result = await plugin.ConnectionHandler.CreateShell(name);

            if (result != HttpStatusCode.Created)
            {
                if (result == HttpStatusCode.PaymentRequired)
                {
                    HandleAPIError(result, window, "You cannot create any more shells!");
                }
                else
                {
                    HandleAPIError(result, window);
                }
            }
        }

        public static async Task DeleteLeaveShell(Plugin plugin, MainWindow window, KinkShell kinkShell)
        {
            window.State.ClearErrors();

            var result = await plugin.ConnectionHandler.DeleteShell(kinkShell.ShellID);

            if (result != HttpStatusCode.OK)
            {
                HandleAPIError(result, window);
            }
        }

        public static async Task UpdateShellUsers(Plugin plugin, MainWindow window, Guid shell, List<ShellNewUser> users)
        {
            window.State.ClearErrors();

            var result = await plugin.ConnectionHandler.UpdateShell(shell, users);

            if (result != HttpStatusCode.OK)
            {
                HandleAPIError(result, window);
            }
        }

        public static void LaunchShellWindow(Plugin plugin, MainWindow window, KinkShell kinkShell)
        {
            var session = plugin.ConnectionHandler.CreateShellSession(kinkShell);
            var shellWindow = plugin.UIHandler.CreateShellWindow(session);

            _ = ShellWindowUtilities.Launch(plugin, shellWindow);
        }

        private static void HandleAPIError(HttpStatusCode statusCode, MainWindow mainWindow, string? errorMessage = null)
        {
            switch(statusCode)
            {
                case HttpStatusCode.BadRequest:
                    mainWindow.State.OnError(errorMessage ?? "There was a problem with the request.");
                    break;
                case HttpStatusCode.Unauthorized:
                    mainWindow.State.OnError(errorMessage ?? "You do not have permission to do that.");
                    break;
                case HttpStatusCode.Conflict:
                    mainWindow.State.OnError(errorMessage ?? "Item already exists.");
                    break;
                case HttpStatusCode.NotFound:
                    mainWindow.State.OnError(errorMessage ?? "The requested item was not found.");
                    break;
                case HttpStatusCode.PaymentRequired:
                    mainWindow.State.OnError(errorMessage ?? "That action is not permitted.");
                    break;
                default:
                    mainWindow.State.OnError(errorMessage ?? "The server encountered an error. Please try again.");
                    break;
            }
        }
    }
}
