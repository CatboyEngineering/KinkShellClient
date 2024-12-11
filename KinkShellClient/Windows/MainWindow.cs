using CatboyEngineering.KinkShellClient.Models;
using CatboyEngineering.KinkShellClient.Models.Shell;
using CatboyEngineering.KinkShellClient.Windows.States;
using CatboyEngineering.KinkShellClient.Windows.Utilities;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using System;
using System.Numerics;

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
            ImGui.SetNextWindowSize(new Vector2(410, 420), ImGuiCond.Always);

            if (ImGui.Begin("KinkShell"))
            {
                Plugin.SmallFontHandle.Push();
                ImGui.Text($"v{Plugin.Version}");
                Plugin.SmallFontHandle.Pop();
                ImGui.Spacing();

                DrawUIWindowBody();
            }

            ImGui.End();
        }

        public void Dispose() { }

        private void DrawUIWindowBody()
        {
            Plugin.TitleHeaderFontHandle.Push();
            DrawUICenteredText("KinkShell");
            Plugin.TitleHeaderFontHandle.Pop();
            DrawUICenteredText("A Linkshell for your kinks.");
            DrawUICenteredText(new Vector4(0.5f, 0.6f, 0.8f, 1), "https://kinkshell.catboy.engineering/");

            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Spacing();

            if (State.HasError)
            {
                DrawUIErrorText(State.ErrorText);
            }

            if (State.IsAuthenticated)
            {
                DrawUIWindowLoggedInHomepage();
            }
            else if (State.IsMigratingV2)
            {
                // TODO continue here
                ImGui.Text("Here's your validation token: " + Plugin.Configuration.KinkShellUserData.VerificationToken);
            }
            else
            {
                var width = ImGui.GetWindowWidth();
                ImGui.BeginChild("MainWindowCTA", new Vector2(width - 15, 125), true);

                // If has user/pass and no login token, show migrate and v1 login button
                // If has neither, show v2 create account button
                // If has login token, show v2 login
                if (Plugin.Configuration.KinkShellServerLoginToken.IsNullOrEmpty())
                {
                    if (!Plugin.Configuration.KinkShellServerUsername.IsNullOrEmpty() && !Plugin.Configuration.KinkShellServerPassword.IsNullOrEmpty() && !Plugin.Configuration.KinkShellServerAddress.IsNullOrEmpty())
                    {
                        ImGui.TextColored(new Vector4(0.9f, 0.9f, 0.3f, 1), "Account Migration Notice:");
                        ImGui.TextWrapped("Accounts are being migrated to a passwordless system. You may migrate now, or log in using your legacy credentials.");
                        ImGui.Spacing();

                        DrawUIConnectV1Button();
                        ImGui.SameLine();
                        DrawUIConnectMigrateV2Button();
                    }
                    else
                    {
                        // Create v2 account
                    }
                }
                else
                {
                    // V2 login
                    // TODO continue here
                }

                ImGui.EndChild();
            }

            if (!State.IsMigratingV2)
            {
                ImGui.Spacing();

                if (ImGui.Button("Pattern Builder"))
                {
                    Plugin.UIHandler.PatternBuilderWindow.IsOpen = true;
                }

                ImGui.SameLine();

                if (ImGui.Button("Open Config"))
                {
                    Plugin.UIHandler.ConfigWindow.IsOpen = true;
                }

                if (!Plugin.Configuration.KinkShellServerUsername.IsNullOrEmpty() && !Plugin.Configuration.KinkShellServerPassword.IsNullOrEmpty() && !Plugin.Configuration.KinkShellServerAddress.IsNullOrEmpty())
                {

                }
                else
                {
                    DrawUIWindowNeedsServer();

                }
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

        private void DrawUIWindowLoggedInHomepage()
        {
            var welcomeText = $"Welcome, {Plugin.Configuration.KinkShellAuthenticatedUserData.DisplayName}!";

            DrawUICenteredText(welcomeText);
            DrawUISectionShellList();
            DrawUIConnectedToys();
            DrawUILogOutButton();
        }

        private void DrawUISectionShellList()
        {
            var width = ImGui.GetWindowWidth();
            ImGui.BeginChild("##UserShellConnectList", new Vector2(width - 15, 150), true);

            if (ImGui.Button("+ New Shell"))
            {
                ImGui.OpenPopup("kinkshell_createshell_dialog");
            }

            ImGui.SameLine();

            if (!State.isRequestInFlight)
            {
                if (ImGui.Button("Refresh"))
                {
                    var task = MainWindowUtilities.GetUserShells(Plugin, this);

                    _ = MainWindowUtilities.HandleWithIndicator(State, task, 500);
                }
            }
            else
            {
                ImGui.BeginDisabled();
                ImGui.Button("Refreshing...");
                ImGui.EndDisabled();
            }

            BuildUIPopupCreateShell();

            ImGui.Spacing();

            if (Plugin.Configuration.Shells != null && Plugin.Configuration.Shells.Count > 0)
            {
                foreach (var shell in Plugin.Configuration.Shells)
                {
                    ImGui.BulletText(shell.ShellName);
                    ImGui.SameLine();

                    if (ImGui.Button($"Join##{shell.ShellID}"))
                    {
                        MainWindowUtilities.LaunchShellWindow(Plugin, this, shell);
                    }

                    if (shell.OwnerID == Plugin.Configuration.KinkShellAuthenticatedUserData.AccountID)
                    {
                        ImGui.SameLine();

                        if (ImGui.Button($"Edit##{shell.ShellID}"))
                        {
                            ImGui.OpenPopup($"kinkshell_editshell_dialog##{shell.ShellID}");
                        }

                        BuildUIPopupEditShell(shell);
                    }
                    else
                    {
                        ImGui.SameLine();

                        if (ImGui.Button($"Remove##{shell.ShellID}"))
                        {
                            _ = MainWindowUtilities.DeleteLeaveShell(Plugin, this, shell);
                        }
                    }
                }
            }
            else
            {
                ImGui.Text("You don't belong to any KinkShells yet.");
            }

            ImGui.Unindent();
            ImGui.EndChild();
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

                    if (Plugin.ToyController.ConnectedToys.Count > 0)
                    {
                        foreach (var toy in Plugin.ToyController.ConnectedToys)
                        {
                            ImGui.BulletText("Connected to " + toy.DisplayName);
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
            if (ImGui.BeginPopup("kinkshell_createshell_dialog"))
            {
                DrawUICenteredText("New Kinkshell");
                ImGui.Spacing();
                ImGui.Text("New KinkShell Name:");
                ImGui.SetKeyboardFocusHere(0);

                if (ImGui.InputText("##NewKinkShellName", ref State.stringBuffer, 64, ImGuiInputTextFlags.EnterReturnsTrue))
                {
                    IssueCreateShell();
                }

                if (State.HasError)
                {
                    DrawUIErrorText(State.ErrorText);
                }

                ImGui.EndPopup();
            }
        }

        private void IssueCreateShell()
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

        private void BuildUIPopupEditShell(KinkShell kinkShell)
        {
            if (ImGui.BeginPopup($"kinkshell_editshell_dialog##{kinkShell.ShellID}"))
            {
                DrawUICenteredText("Edit Kinkshell Members");
                ImGui.Spacing();

                ImGui.Text("New User ID:");

                if (ImGui.InputText("##NewKinkShellUser", ref State.stringBuffer, 40, ImGuiInputTextFlags.EnterReturnsTrue))
                {
                    IssueTryAddUser();
                }

                ImGui.Checkbox("Allow Commands?", ref State.canSendCommands);
                ImGui.SameLine();

                if (ImGui.Button("Add User"))
                {
                    IssueTryAddUser();
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
                    State.ClearErrors();

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

        private void IssueTryAddUser()
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

        private void DrawUIConnectV1Button()
        {
            if (!Plugin.Configuration.KinkShellServerUsername.IsNullOrEmpty())
            {
                if (!State.isRequestInFlight)
                {
                    if (ImGui.Button("Legacy Login"))
                    {
                        var task = MainWindowUtilities.LogInAndRetrieve(Plugin, this);

                        _ = Plugin.ToyController.Connect();
                        _ = MainWindowUtilities.HandleWithIndicator(State, task);
                    }
                }
                else
                {
                    ImGui.BeginDisabled();
                    ImGui.Button("Connecting...");
                    ImGui.EndDisabled();
                }
            }
        }

        private void DrawUIConnectMigrateV2Button()
        {
            if (!State.isRequestInFlight)
            {
                if (ImGui.Button("Migrate Account"))
                {
                    var task = MainWindowUtilities.LogInV1AndMigrate(Plugin, this);

                    _ = MainWindowUtilities.HandleWithIndicator(State, task);
                }
            }
            else
            {
                ImGui.BeginDisabled();
                ImGui.Button("Connecting...");
                ImGui.EndDisabled();
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

        private void DrawUICenteredText(Vector4 color, string text)
        {
            var windowWidth = ImGui.GetWindowSize().X;
            var textWidth = ImGui.CalcTextSize(text).X;

            ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);
            ImGui.TextColored(color, text);
        }

        private void DisconnectAll()
        {
            _ = MainWindowUtilities.LogOut(Plugin, this);
            _ = Plugin.ToyController.Disconnect();
        }
    }
}