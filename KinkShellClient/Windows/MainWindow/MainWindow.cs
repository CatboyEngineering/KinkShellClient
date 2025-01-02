using CatboyEngineering.KinkShellClient.Models;
using CatboyEngineering.KinkShellClient.Models.Shell;
using Dalamud.Interface.Components;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using System;
using System.Diagnostics;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace CatboyEngineering.KinkShellClient.Windows.MainWindow
{
    public class MainWindow : Window, IDisposable
    {
        private Plugin Plugin;

        public MainWindowState State { get; init; }

        public MainWindow(Plugin plugin) : base("KinkShell", ImGuiWindowFlags.NoResize)
        {
            Plugin = plugin;
            State = new MainWindowState(plugin);
        }

        public override void Draw()
        {
            ImGui.SetNextWindowSize(new Vector2(410, 575), ImGuiCond.Always);

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

            switch (State.Screen)
            {
                case MainWindowScreen.CREATE:
                    DrawScreenCreateAccount();
                    ImGui.Spacing();
                    BtnConfiguration();
                    break;
                case MainWindowScreen.LOGIN:
                    DrawScreenLogIn();
                    ImGui.Spacing();
                    DrawBtnToolbar();
                    break;
                case MainWindowScreen.MIGRATE:
                    DrawScreenMigrate();
                    ImGui.Spacing();
                    DrawBtnToolbar();
                    break;
                case MainWindowScreen.VERIFY:
                    DrawScreenVerify();
                    break;
                case MainWindowScreen.VERIFY_RECOVERY:
                    DrawScreenVerifyRecovery();
                    break;
                case MainWindowScreen.HOME:
                    DrawUIWindowLoggedInHomepage();
                    ImGui.Spacing();
                    DrawBtnToolbar();
                    break;
                case MainWindowScreen.ADMIN:
                    DrawScreenAdmin();
                    break;
            }
        }

        private void DrawScreenMigrate()
        {
            var width = ImGui.GetWindowWidth();
            ImGui.BeginChild("MainWindowCTA#Migrate", new Vector2(width - 15, 125), true);

            ImGui.TextColored(new Vector4(0.9f, 0.9f, 0.3f, 1), "Account Migration Notice:");
            ImGui.TextWrapped("Accounts are being migrated to a passwordless system. You may migrate now, or log in using your legacy credentials.");
            ImGui.Spacing();

            BtnLoginV1();
            ImGui.SameLine();
            BtnMigrateV2();

            ImGui.EndChild();
        }

        private void DrawScreenCreateAccount()
        {
            var width = ImGui.GetWindowWidth();
            ImGui.BeginChild("MainWindowCTA#CreateAccount", new Vector2(width - 15, 200), true);

            Plugin.HeaderFontHandle.Push();
            DrawUICenteredText("Welcome");
            Plugin.HeaderFontHandle.Pop();

            ImGui.TextWrapped("Welcome to KinkShell! Click the button below to get started, or visit us at the link above to learn more.");
            BtnCreateAccount();
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.TextColored(new Vector4(0.9f, 0.9f, 0.3f, 1), "Account Recovery:");
            ImGui.TextWrapped("If you already have a KinkShell account, you can regain access to it using the button below.");

            BtnRecoverAccount();

            ImGui.EndChild();
        }

        private void DrawScreenVerify()
        {
            var width = ImGui.GetWindowWidth();
            ImGui.BeginChild("MainWindowCTA#Verify", new Vector2(width - 15, 250), true);

            Plugin.HeaderFontHandle.Push();
            DrawUICenteredText("Verify Character");
            Plugin.HeaderFontHandle.Pop();
            DrawUICenteredText($"{Plugin.Configuration.KinkShellUserData.CharacterName} • {Plugin.Configuration.KinkShellUserData.CharacterServer}");

            ImGui.Spacing();

            ImGui.TextWrapped("Verify your character to finish setting up KinkShell. Add the following code to your Lodestone Profile Bio, and once ready, click Verify to confirm.");

            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Text("Validation Code:");

            if (!string.IsNullOrEmpty(Plugin.Configuration.KinkShellUserData.VerificationToken))
            {
                ImGui.Text(Plugin.Configuration.KinkShellUserData.VerificationToken);
                ImGui.SameLine();

                if (ImGui.Button("Copy"))
                {
                    ImGui.SetClipboardText(Plugin.Configuration.KinkShellUserData.VerificationToken);
                }
            }

            ImGui.Spacing();
            ImGui.Spacing();

            if (ImGui.Button("Open Lodestone Profile"))
            {
                Process proc = new Process();
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.FileName = $"https://na.finalfantasyxiv.com/lodestone/character/{Plugin.Configuration.KinkShellUserData.CharacterID}";
                proc.Start();
            }

            ImGui.SameLine();

            BtnVerifyCharacter();

            ImGui.Spacing();
            ImGui.Spacing();

            Plugin.SmallFontHandle.Push();
            ImGui.TextWrapped("Your KinkShell account will be bound to this character. While you can log in using alts, this character will be used for your identity.");
            Plugin.SmallFontHandle.Pop();

            ImGui.EndChild();
        }

        private void DrawScreenVerifyRecovery()
        {
            var width = ImGui.GetWindowWidth();
            ImGui.BeginChild("MainWindowCTA#VerifyRecovery", new Vector2(width - 15, 250), true);

            Plugin.HeaderFontHandle.Push();
            DrawUICenteredText("Verify Character Recovery");
            Plugin.HeaderFontHandle.Pop();

            ImGui.Spacing();

            ImGui.TextWrapped("Verify your character to finalize recovery. Add the following code to your Lodestone Profile Bio, and once ready, click Verify to confirm.");

            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Text("Validation Code:");

            if (!string.IsNullOrEmpty(Plugin.Configuration.KinkShellUserData.VerificationToken))
            {
                ImGui.Text(Plugin.Configuration.KinkShellUserData.VerificationToken);
                ImGui.SameLine();

                if (ImGui.Button("Copy"))
                {
                    ImGui.SetClipboardText(Plugin.Configuration.KinkShellUserData.VerificationToken);
                }
            }

            ImGui.Spacing();
            ImGui.Spacing();

            if (ImGui.Button("Open Lodestone Profile"))
            {
                Process proc = new Process();
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.FileName = $"https://na.finalfantasyxiv.com/lodestone/character/{Plugin.Configuration.KinkShellUserData.CharacterID}";
                proc.Start();
            }

            ImGui.SameLine();

            BtnVerifyCharacterRecovery();

            ImGui.EndChild();
        }

        private void DrawScreenLogIn()
        {
            var width = ImGui.GetWindowWidth();
            ImGui.BeginChild("MainWindowCTA#LogIn", new Vector2(width - 15, 125), true);

            Plugin.HeaderFontHandle.Push();
            DrawUICenteredText("Welcome Back!");
            Plugin.HeaderFontHandle.Pop();
            DrawUICenteredText("Click the button below to log back into KinkShell.");

            BtnLoginV2();

            ImGui.EndChild();
        }

        private void DrawScreenAdmin()
        {
            // TODO update max shells

            if (Plugin.Configuration.AdminUserList != null)
            {
                ImGui.Text("Users:");

                foreach (var user in Plugin.Configuration.AdminUserList)
                {
                    ImGui.Text($"{user.DisplayName} {(user.IsVerified ? "(Verified)" : "(Unverified)")} - {user.AccountID[..10]}...");
                    ImGui.SameLine();

                    if (ImGui.Button($"Copy##{user.AccountID}"))
                    {
                        ImGui.SetClipboardText(user.AccountID);
                    }
                }
            }

            if (!State.isRequestInFlight)
            {
                if (ImGui.Button("View Accounts"))
                {
                    var task = MainWindowUtilities.GetAllAccounts(Plugin, this);

                    _ = MainWindowUtilities.HandleWithIndicator(State, task);
                }
            }
            else
            {
                ImGui.BeginDisabled();
                ImGui.Button("Connecting...");
                ImGui.EndDisabled();
            }

            if (ImGui.Button("Back"))
            {
                State.Screen = MainWindowScreen.HOME;
            }
        }

        private void DrawBtnToolbar()
        {
            BtnPatternBuilder();
            ImGui.SameLine();
            BtnConfiguration();
        }

        private void BtnPatternBuilder()
        {
            if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.PuzzlePiece, "Pattern Builder", new Vector2(125f, 24f)))
            {
                Plugin.UIHandler.PatternBuilderWindow.IsOpen = true;
            }
        }

        private void BtnConfiguration()
        {
            if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.Cog, "Configuration", new Vector2(115f, 24f)))
            {
                Plugin.UIHandler.ConfigWindow.IsOpen = true;
            }
        }

        private void BtnAdmin()
        {
            if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.Toolbox, "Admin", new Vector2(75f, 24f)))
            {
                State.Screen = MainWindowScreen.ADMIN;
            }
        }

        private void DrawUIWindowLoggedInHomepage()
        {
            var welcomeText = $"Welcome, {Plugin.Configuration.KinkShellAuthenticatedUserData.DisplayName}!";

            DrawUICenteredText(welcomeText);

            ImGui.Spacing();
            ImGui.TextColored(new Vector4(0, 1, 0, 1), "Your ID:");
            ImGui.TextColored(new Vector4(0.75f, 0.75f, 0.75f, 1), Plugin.Configuration.KinkShellAuthenticatedUserData.AccountID.ToString());
            ImGui.SameLine();

            if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.Copy, "Copy", new Vector2(60f, 24f)))
            {
                ImGui.SetClipboardText(Plugin.Configuration.KinkShellAuthenticatedUserData.AccountID.ToString());
            }

            ImGui.Spacing();

            DrawUISectionShellList();
            DrawUIConnectedToys();
            BtnLogOut();

            if (Plugin.Configuration.KinkShellUserData.IsAdmin != null && Plugin.Configuration.KinkShellUserData.IsAdmin! == true)
            {
                BtnAdmin();
            }

            ImGui.Spacing();

            BtnDeleteAccount();
        }

        private void DrawUISectionShellList()
        {
            var width = ImGui.GetWindowWidth();
            ImGui.BeginChild("##UserShellConnectList", new Vector2(width - 15, 150), true);

            if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.Plus, "New Shell", new Vector2(90f, 24f)))
            {
                ImGui.OpenPopup("kinkshell_createshell_dialog");
            }

            ImGui.SameLine();

            if (!State.isRequestInFlight)
            {
                if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.Sync, "Refresh", new Vector2(75f, 24f)))
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

                    if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.Walking, $"Join##{shell.ShellID}", new Vector2(55f, 24f)))
                    {
                        MainWindowUtilities.LaunchShellWindow(Plugin, this, shell);
                        this.IsOpen = false;
                    }

                    if (shell.OwnerID == Plugin.Configuration.KinkShellAuthenticatedUserData.AccountID)
                    {
                        ImGui.SameLine();

                        if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.PencilAlt, $"Edit##{shell.ShellID}", new Vector2(55f, 24f)))
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
                if (Plugin.ToyController.IsConnecting)
                {
                    ImGui.Text("Intiface connecting...");
                }
                else
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

                        if (Plugin.ToyController.IsScanning)
                        {
                            ImGui.Text("Scanning for devices...");
                        }
                        else
                        {
                            if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.Wifi, "Re-scan", new Vector2(80f, 24f)))
                            {
                                _ = Plugin.ToyController.Scan();
                            }
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
                Plugin.HeaderFontHandle.Push();
                DrawUICenteredText("Edit Kinkshell");
                Plugin.HeaderFontHandle.Pop();

                ImGui.Spacing();

                ImGui.Text("Add a New User:");
                ImGui.BeginChild("##EditShellAddUserSection", new Vector2(350, 100), true);
                ImGui.Text("New User ID:");

                if (ImGui.InputText("##NewKinkShellUser", ref State.stringBuffer, 40, ImGuiInputTextFlags.EnterReturnsTrue))
                {
                    IssueTryAddUser();
                }

                ImGui.SameLine();

                if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.Plus, "Add User", new Vector2(85f, 24f)))
                {
                    IssueTryAddUser();
                }

                ImGui.Spacing();
                ImGui.Checkbox("Allow Commands?", ref State.canSendCommands);
                ImGuiComponents.HelpMarker("Whether this user is allowed to send pattern commands to other KinkShell members.");
                ImGui.EndChild();

                ImGui.Spacing();

                ImGui.Text("KinkShell Users:");
                ImGui.BeginChild("##ShellUserEditList", new Vector2(500, 175), true);

                if (State.UsersToAdd.Count > 0)
                {
                    ImGui.Text("New Users:");
                    foreach (var userToAdd in State.UsersToAdd)
                    {
                        ImGui.BulletText($"({userToAdd.UserID.ToString().Substring(0, 7)}...)");
                        ImGui.SameLine();

                        DrawUICommandsEnabled(userToAdd.SendCommands);

                        ImGui.SameLine();
                        if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.Times, "Cancel", new Vector2(70f, 24f)))
                        {
                            State.UsersToAdd.Remove(userToAdd);
                        }
                    }
                }

                if (kinkShell.Users.Count > 0)
                {
                    ImGui.Text("KinkShell Members:");
                    foreach (var currentUser in kinkShell.Users)
                    {
                        ImGui.BulletText($"{currentUser.DisplayName}");
                        ImGui.SameLine();
                        ImGui.Text($"({currentUser.AccountID.ToString().Substring(0, 7)}...)");
                        ImGui.SameLine();

                        DrawUICommandsEnabled(currentUser.SendCommands);

                        ImGui.SameLine();

                        if (!State.GuidsToDelete.Contains(currentUser.AccountID))
                        {
                            if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.Trash, $"Remove##{currentUser.AccountID}", new Vector2(80f, 24f)))
                            {
                                State.GuidsToDelete.Add(currentUser.AccountID);
                            }
                        }
                        else
                        {
                            ImGui.Text($"(removing)");
                        }
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

        [Obsolete]
        private void BtnLoginV1()
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

        private void BtnMigrateV2()
        {
            if (State.isRequestInFlight)
            {
                ImGui.BeginDisabled();
            }

            if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.ArrowUp, "Migrate Account", new Vector2(125f, 24f)))
            {
                var task = MainWindowUtilities.LogInV1AndMigrate(Plugin, this);

                _ = MainWindowUtilities.HandleWithIndicator(State, task);
            }

            if (State.isRequestInFlight)
            {
                ImGui.EndDisabled();
            }
        }

        private void BtnCreateAccount()
        {
            if (State.isRequestInFlight)
            {
                ImGui.BeginDisabled();
            }

            if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.Plus, "Get Started", new Vector2(120f, 24f)))
            {
                var task = MainWindowUtilities.CreateAccount(Plugin, this);

                _ = MainWindowUtilities.HandleWithIndicator(State, task);
            }

            if (State.isRequestInFlight)
            {
                ImGui.EndDisabled();
            }
        }

        private void BtnDeleteAccount()
        {
            if (State.isDeletingAccount)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 1f, 1f, 0.7f));
                ImGui.TextWrapped("Deleting your account is irreversible! All of your KinkShells will be erased.");
                ImGui.PopStyleColor();

                if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.XmarksLines, "Cancel", new Vector2(75f, 24f)))
                {
                    State.isDeletingAccount = false;
                }

                ImGui.SameLine();

                if (State.isRequestInFlight)
                {
                    ImGui.BeginDisabled();
                }

                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.9f, 0.2f, 0.2f, 1f));
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 1f, 1f, 1f));
                if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.Trash, "Confirm", new Vector2(80f, 24f)))
                {
                    var task = MainWindowUtilities.DeleteAccount(Plugin, this);

                    _ = MainWindowUtilities.HandleWithIndicator(State, task);
                }
                ImGui.PopStyleColor();
                ImGui.PopStyleColor();

                if (State.isRequestInFlight)
                {
                    ImGui.EndDisabled();
                }
            }
            else
            {
                if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.Trash, "Delete Account", new Vector2(100f, 24f)))
                {
                    State.isDeletingAccount = true;
                }
            }
        }

        private void BtnVerifyCharacter()
        {
            if (State.isRequestInFlight)
            {
                ImGui.BeginDisabled();
            }

            if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.CheckSquare, "Verify", new Vector2(75f, 24f)))
            {
                var task = MainWindowUtilities.VerifyCharacter(Plugin, this);

                _ = MainWindowUtilities.HandleWithIndicator(State, task);
            }

            if (State.isRequestInFlight)
            {
                ImGui.EndDisabled();
            }
        }

        private void BtnVerifyCharacterRecovery()
        {
            if (State.isRequestInFlight)
            {
                ImGui.BeginDisabled();
            }

            if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.CheckSquare, "Verify", new Vector2(75f, 24f)))
            {
                var task = MainWindowUtilities.RecoverAccountVerify(Plugin, this);

                _ = MainWindowUtilities.HandleWithIndicator(State, task);
            }

            if (State.isRequestInFlight)
            {
                ImGui.EndDisabled();
            }
        }

        private void BtnLoginV2()
        {
            var windowWidth = ImGui.GetWindowSize().X;
            var settingsTextWidth = ImGui.CalcTextSize("XX Connect to KinkShell").X;

            ImGui.SetCursorPosX((windowWidth - settingsTextWidth) * 0.5f);

            if (State.isRequestInFlight)
            {
                ImGui.BeginDisabled();
            }

            if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.SignInAlt, "Connect to KinkShell", new Vector2(155f, 24f)))
            {
                var task = MainWindowUtilities.LogInV2(Plugin, this);

                _ = MainWindowUtilities.HandleWithIndicator(State, task);
            }

            if (State.isRequestInFlight)
            {
                ImGui.EndDisabled();
            }
        }

        private void BtnRecoverAccount()
        {
            if (!State.isRequestInFlight)
            {
                if (ImGui.Button("Recover Account"))
                {
                    var task = MainWindowUtilities.RecoverAccount(Plugin, this);

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

        private void BtnLogOut()
        {
            // Put logout button in right corner.
            var buttonTextWidth = ImGui.CalcTextSize("Log Out").X;
            var windowWidth = ImGui.GetWindowSize().X;
            ImGui.SetCursorPosX((windowWidth - buttonTextWidth) * 0.90f);

            if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.SignOutAlt, "Log Out", new Vector2(75f, 24f)))
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
            State.SetDefauts();
        }
    }
}