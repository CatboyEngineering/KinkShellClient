using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using CatboyEngineering.KinkShellClient.ShellData;
using CatboyEngineering.KinkShellClient.Windows.Utilities;
using System;
using System.Numerics;
using System.Text;
using CatboyEngineering.KinkShellClient.Models;

namespace CatboyEngineering.KinkShellClient.Windows
{
    public class MainWindow : Window, IDisposable
    {
        private Plugin Plugin;

        public MainWindowState State { get; init; }

        public MainWindow(Plugin plugin) : base("KinkShell", ImGuiWindowFlags.NoResize)
        {
            this.Plugin = plugin;
            State = new MainWindowState(plugin);
        }

        public override void OnClose()
        {
            base.OnClose();
            DisconnectAll();
        }

        public override void Draw()
        {
            ImGui.SetNextWindowSize(new Vector2(410, 300), ImGuiCond.Always);

            if (ImGui.Begin("KinkShell"))
            {
                ImGui.Text("[DEVELOPER BETA]");
                ImGui.Spacing();

                DrawUIWindowBody();
            }

            ImGui.End();
        }

        public void Dispose() { }

        private void DrawUIWindowBody()
        {
            if (!Plugin.Configuration.KinkShellServerUsername.IsNullOrEmpty() && !Plugin.Configuration.KinkShellServerPassword.IsNullOrEmpty() && !Plugin.Configuration.KinkShellServerAddress.IsNullOrEmpty())
            {
                if (!State.IsAuthenticated)
                {
                    DrawUIWindowReadyToConnect();
                }
                else
                {
                    DrawUIWindowLoggedInHomepage();
                }

                if (State.HasError)
                {
                    DrawUIErrorText(State.ErrorText);
                }
            } else
            {
                DrawUIWindowNeedsServer();
            }
        }

        private void DrawUIWindowNeedsServer()
        {
            var text = "Please add your KinkShell Server login credentials.";

            var windowWidth = ImGui.GetWindowSize().X;
            var textWidth = ImGui.CalcTextSize(text).X;

            ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);
            DrawUIErrorText(text);

            var settingsTextWidth = ImGui.CalcTextSize("Settings").X;
            ImGui.SetCursorPosX((windowWidth - settingsTextWidth) * 0.5f);
            DrawUISettingsButton();
        }

        private void DrawUIWindowReadyToConnect()
        {
            var text = "Press the button below to connect.";

            var windowWidth = ImGui.GetWindowSize().X;
            var textWidth = ImGui.CalcTextSize(text).X;

            ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);
            ImGui.Text(text);

            var settingsTextWidth = ImGui.CalcTextSize("Connect").X;
            ImGui.SetCursorPosX((windowWidth - settingsTextWidth) * 0.5f);
            DrawUIConnectButton();

            ImGui.Spacing();

            if(ImGui.Button("Pattern Builder"))
            {
                Plugin.UIHandler.PatternBuilderWindow.IsOpen = true;
            }

            ImGui.SameLine();

            if (ImGui.Button("Open Config"))
            {
                Plugin.UIHandler.ConfigWindow.IsOpen = true;
            }
        }

        private void DrawUIWindowLoggedInHomepage()
        {
            var welcomeText = $"Welcome, {Plugin.Configuration.KinkShellAuthenticatedUserData.DisplayName}!";

            DrawUICenteredText(welcomeText);

            ImGui.Spacing();
            ImGui.Spacing();

            DrawUISectionShellList();
            DrawUIConnectedToys();
            DrawUILogOutButton();
        }

        private void DrawUISectionShellList()
        {
            ImGui.Text("Your Shells");
            ImGui.SameLine();
            ImGui.Indent();
            ImGui.SameLine();
            
            if(ImGui.Button("+ New Shell"))
            {
                ImGui.OpenPopup("kinkshell_createshell_dialog");
            }

            ImGui.SameLine();

            if (ImGui.Button("Refresh"))
            {
                _ = MainWindowUtilities.GetUserShells(Plugin, this);
            }

            BuildUIPopupCreateShell();

            if (Plugin.Configuration.Shells != null)
            {
                foreach (var shell in Plugin.Configuration.Shells)
                {
                    ImGui.Text(shell.ShellName);
                    ImGui.SameLine();

                    if (ImGui.Button($"Join##{shell.ShellID}"))
                    {
                        MainWindowUtilities.LaunchShellWindow(Plugin, this, shell);
                    }

                    if (shell.OwnerID == Plugin.Configuration.KinkShellAuthenticatedUserData.AccountID)
                    {
                        ImGui.SameLine();

                        if(ImGui.Button($"Edit##{shell.ShellID}"))
                        {
                            ImGui.OpenPopup($"kinkshell_editshell_dialog##{shell.ShellID}");
                        }

                        BuildUIPopupEditShell(shell);
                    }
                    else
                    {
                        ImGui.SameLine();

                        if(ImGui.Button($"Leave##{shell.ShellID}"))
                        {
                            _ = MainWindowUtilities.DeleteLeaveShell(Plugin, this, shell);
                        }
                    }
                }
            }

            ImGui.Unindent();
        }

        private void DrawUIConnectedToys()
        {
            ImGui.Spacing();

            if (Plugin.ToyController.Client != null)
            {
                if (Plugin.ToyController.Client.Connected)
                {
                    ImGui.Text("Intiface Connected!");

                    var width = ImGui.GetWindowWidth();
                    ImGui.BeginChild("IntifaceWindow", new Vector2(width - 15, 75), true);

                    if (Plugin.ToyController.Client.Devices.Length > 0)
                    {
                        foreach (var toy in Plugin.ToyController.Client.Devices)
                        {
                            ImGui.BulletText("Connected to " + toy.Name);
                        }
                    }
                    else
                    {
                        ImGui.Text("No Connected Devices");
                    }

                    if (ImGui.Button("Re-scan"))
                    {
                        _ = Plugin.ToyController.Scan();
                    }

                    ImGui.EndChild();
                }
                else
                {
                    ImGui.Text("Intiface not connected");
                    ImGui.SameLine();

                    if (ImGui.Button("Retry"))
                    {
                        _ = Plugin.ToyController.Connect();
                    }
                }
            }
            else
            {
                ImGui.Text("Intiface connecting...");
            }
        }

        private void BuildUIPopupCreateShell()
        {
            if(ImGui.BeginPopup("kinkshell_createshell_dialog"))
            {
                DrawUICenteredText("New Kinkshell");
                ImGui.Spacing();
                ImGui.InputText("Shell name", ref State.stringBuffer, 64);

                if (ImGui.Button("Create Shell"))
                {
                    var newShellName = State.stringBuffer.Trim();

                    if (newShellName.Length > 0)
                    {
                        State.ResetBuffers();
                        _ = MainWindowUtilities.CreateShell(Plugin, this, newShellName);
                        ImGui.CloseCurrentPopup();
                    }
                    else
                    {
                        State.OnError("Shell name cannot be blank");
                    }
                }

                if(State.HasError)
                {
                    DrawUIErrorText(State.ErrorText);
                }

                ImGui.EndPopup();
            }
        }

        private void BuildUIPopupEditShell(KinkShell kinkShell)
        {
            if (ImGui.BeginPopup($"kinkshell_editshell_dialog##{kinkShell.ShellID}"))
            {
                DrawUICenteredText("Edit Kinkshell Members");
                ImGui.Spacing();

                ImGui.Text("New User ID:");
                ImGui.InputText("", ref State.stringBuffer, 40, ImGuiInputTextFlags.EnterReturnsTrue);
                ImGui.Checkbox("Allow Commands?", ref State.canSendCommands);
                ImGui.SameLine();

                if (ImGui.Button("Add User"))
                {
                    var newUser = State.stringBuffer.Trim();

                    if (newUser.Length > 0 && Guid.TryParse(newUser, out Guid newGuid))
                    {
                        State.UsersToAdd.Add(new ShellNewUser
                        {
                            UserID = newGuid,
                            SendCommands = State.canSendCommands
                        });
                        State.ResetBuffers();
                    }
                    else
                    {
                        State.OnError("Please enter a valid Kinkshell User ID.");
                    }
                }

                ImGui.Spacing();
                ImGui.BeginChild("##ShellUserEditList", new Vector2(625, 175), true);

                foreach (var userToAdd in State.UsersToAdd)
                {
                    ImGui.Text(userToAdd.UserID.ToString());
                    ImGui.SameLine();
                    ImGui.Text("(pending)");
                    ImGui.SameLine();

                    DrawUICommandsEnabled(userToAdd.SendCommands);
                }

                foreach (var currentUser in kinkShell.Users)
                {
                    ImGui.Text(currentUser.AccountID.ToString());
                    ImGui.SameLine();
                    ImGui.Text($"({currentUser.DisplayName})");
                    ImGui.SameLine();

                    DrawUICommandsEnabled(currentUser.SendCommands);

                    ImGui.SameLine();

                    if (!State.GuidsToDelete.Contains(currentUser.AccountID))
                    {
                        if (ImGui.Button($"Remove##{currentUser.AccountID}"))
                        {
                            State.GuidsToDelete.Add(currentUser.AccountID);
                        }
                    }
                    else
                    {
                        ImGui.Text($"(removing)");
                    }
                }

                ImGui.EndChild();
                ImGui.Spacing();

                if (ImGui.Button("Cancel"))
                {
                    State.ResetBuffers();
                    State.UsersToAdd.Clear();
                    State.GuidsToDelete.Clear();

                    ImGui.CloseCurrentPopup();
                }

                ImGui.SameLine();

                if (ImGui.Button("Save Changes"))
                {
                    _ = MainWindowUtilities.UpdateShellUsers(Plugin, this, kinkShell.ShellID, State.GetShellMembers(kinkShell));

                    State.UsersToAdd.Clear();
                    State.GuidsToDelete.Clear();

                    ImGui.CloseCurrentPopup();
                }

                ImGui.SameLine();
                if (ImGui.Button("Delete Shell"))
                {
                    _ = MainWindowUtilities.DeleteLeaveShell(Plugin, this, kinkShell);

                    State.UsersToAdd.Clear();
                    State.GuidsToDelete.Clear();

                    ImGui.CloseCurrentPopup();
                }

                    if (State.HasError)
                {
                    DrawUIErrorText(State.ErrorText);
                }

                ImGui.EndPopup();
            }
        }

        private void DrawUICommandsEnabled(bool enabled)
        {
            if (enabled)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0f, 1f, 0f, 1f));
                ImGui.Text($"[Commands enabled]");
                ImGui.PopStyleColor();
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 0f, 0f, 1f));
                ImGui.Text($"[Commands disabled]");
                ImGui.PopStyleColor();
            }
        }

        private void DrawUIConnectButton()
        {
            if (!Plugin.Configuration.KinkShellServerUsername.IsNullOrEmpty())
            {
                if (ImGui.Button("Connect"))
                {
                    _ = MainWindowUtilities.LogInAndRetrieve(Plugin, this);
                }
            }
        }

        private void DrawUILogOutButton()
        {
            // Put logout button in right corner.
            var buttonTextWidth = ImGui.CalcTextSize("Log Out").X;
            var windowWidth = ImGui.GetWindowSize().X;
            ImGui.SetCursorPosX((windowWidth - buttonTextWidth) * 0.90f);

            if (ImGui.Button("Log Out"))
            {
                DisconnectAll();
            }
        }

        private void DrawUISettingsButton()
        {
            if (ImGui.Button("Settings"))
            {
                Plugin.UIHandler.DrawConfigUI();
            }
        }

        private void DrawUIErrorText(string text)
        {
            ImGui.TextColored(new Vector4(1, 0, 0, 1), text);
        }

        private void DrawUICenteredText(string text)
        {
            var windowWidth = ImGui.GetWindowSize().X;
            var textWidth = ImGui.CalcTextSize(text).X;

            ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);
            ImGui.Text(text);
        }

        private void DisconnectAll()
        {
            _ = MainWindowUtilities.LogOut(Plugin, this);
            _ = Plugin.ToyController.Disconnect();
        }
    }
}