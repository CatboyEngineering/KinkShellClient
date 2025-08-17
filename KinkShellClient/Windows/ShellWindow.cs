using CatboyEngineering.KinkShellClient.Models;
using CatboyEngineering.KinkShellClient.Models.Shell;
using CatboyEngineering.KinkShellClient.Windows.MainWindow;
using CatboyEngineering.KinkShellClient.Windows.Utilities;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using System;
using System.Linq;
using System.Numerics;

namespace CatboyEngineering.KinkShellClient.Windows
{
    public class ShellWindow : Window, IDisposable
    {
        public Plugin Plugin { get; set; }
        public ShellWindowState State { get; set; }
        public KinkShellMember SelfUser { get; set; }

        public ShellWindow(Plugin plugin, KinkShell kinkShell, ShellSession session) : base(kinkShell.ShellName, ImGuiWindowFlags.NoResize)
        {
            Plugin = plugin;
            State = new ShellWindowState(plugin, kinkShell, this);

            State.Session = session;
        }

        public override void OnClose()
        {
            base.OnClose();

            DisconnectFromShell();

            Plugin.UIHandler.OpenMainWindow();
        }

        public override void Draw()
        {   
            ImGui.SetNextWindowSize(new Vector2(505, 775), ImGuiCond.Always);

            if (ImGui.Begin(this.WindowName))
            {
                SelfUser = ShellWindowUtilities.GetSelf(Plugin, State.Session);

                DrawUIWindowBody();
            }

            ImGui.End();
        }

        public void Dispose() { }

        private void DrawUIWindowBody()
        {
            Plugin.TitleHeaderFontHandle.Push();
            DrawUICenteredText(State.KinkShell.ShellName);
            Plugin.TitleHeaderFontHandle.Pop();

            ImGui.Spacing();

            if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.SignOutAlt, "Leave Session", new Vector2(120f, 24f)))
            {
                this.IsOpen = false;

                return;
            }

            ImGui.Spacing();
            ImGui.Spacing();

            DrawUIPatternCenter();

            ImGui.Spacing();

            DrawUIChatWindow();
            DrawUISafetyWindow();

            ImGui.Spacing();

            if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.PuzzlePiece, "Pattern Builder", new Vector2(125f, 24f)))
            {
                Plugin.UIHandler.PatternBuilderWindow.IsOpen = true;
            }
        }

        private void DrawUIPatternCenter()
        {
            ImGui.Text("Connected Users:");
            var width = ImGui.GetWindowWidth();
            ImGui.BeginChild("ToyControlCenter", new Vector2(width - 15, 200), true);


            for(var i = 0; i < State.Session.ConnectedUsers.Count; i++)
            {
                var user = State.Session.ConnectedUsers[i];

                if (i % 2 != 0)
                {
                    ImGui.SameLine();
                }

                DrawUIUserCommand(user);
            }

            ImGui.EndChild();
        }

        private void DrawUIUserCommand(KinkShellMember user)
        {
            ImGui.BeginChild($"{user.DisplayName}", new Vector2(225, 100), true);
            DrawPopupCommandUser(user);

            ImGui.PushStyleColor(ImGuiCol.Text, user.TextColor);
            ImGui.Text(user.DisplayName);
            ImGui.PopStyleColor();

            if (user.Toys.Count > 0)
            {
                var disable = !SelfUser.SendCommands || user.RunningCommands.Count > 0;

                if (disable)
                {
                    ImGui.BeginDisabled();
                }

                ImGui.SameLine();

                if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.WaveSquare, "Command", new Vector2(100f, 24f)))
                {
                    ImGui.OpenPopup($"command_user_{user.AccountID}");
                }

                if (disable)
                {
                    ImGui.EndDisabled();
                }
            }

            if (user.RunningCommands.Count > 0)
            {
                ImGui.Text("Running commands:");
                foreach (var runningCommand in user.RunningCommands)
                {
                    ImGui.BulletText($"{runningCommand.CommandName}");
                }
            }


            ImGui.EndChild();
        }

        private void DrawPopupCommandUser(KinkShellMember user)
        {
            if (ImGui.BeginPopup($"command_user_{user.AccountID}"))
            {
                ImGui.BeginChild($"command_user_{user.AccountID}_child", new Vector2(350, 175), true);

                Plugin.HeaderFontHandle.Push();
                ImGui.PushStyleColor(ImGuiCol.Text, user.TextColor);
                ImGui.Text($"Command {user.DisplayName}");
                ImGui.PopStyleColor();
                Plugin.HeaderFontHandle.Pop();

                ImGui.Spacing();
                ImGui.Text("Toy:");
                ImGui.SameLine();
                ImGui.Combo("##UserToyCombo", ref State.intBuffer, user.Toys.Select(t => t.DisplayName).ToArray(), user.Toys.Count);

                if (State.onCooldown)
                {
                    ImGui.BeginDisabled();
                }

                ImGui.Spacing();

                var commands = ShellWindowUtilities.GetAvailableShellCommands(Plugin);
                var toy = user.Toys[State.intBuffer];

                for (var i=0; i<commands.Count; i++)
                {
                    var storedCommand = commands[i];
                    
                    if (i % 3 != 0)
                    {
                        ImGui.SameLine();
                    }

                    var canRun = ShellWindowUtilities.CanRun(toy, storedCommand);

                    if (!canRun)
                    {
                        ImGui.BeginDisabled();
                    }

                    if (ImGui.Button($"{storedCommand.Name}"))
                    {
                        _ = ShellWindowUtilities.SendCommand(Plugin, State.Session, user.AccountID, toy.DeviceInstanceID, storedCommand);
                        _ = ShellWindowUtilities.Cooldown(this);

                        ImGui.CloseCurrentPopup();
                    }

                    if (!canRun)
                    {
                        ImGui.EndDisabled();
                    }
                }

                ImGui.Spacing();
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 1f, 1f, 0.7f));
                ImGui.TextWrapped($"Disabled commands are not supported by the selected device.");
                ImGui.PopStyleColor();

                if (State.onCooldown)
                {
                    ImGui.EndDisabled();
                }

                ImGui.EndChild();
                ImGui.EndPopup();
            }
        }

        private void DrawUIChatWindow()
        {
            ImGui.Text("Chat:");
            var width = ImGui.GetWindowWidth();
            ImGui.BeginChild("ChatWindow", new Vector2(width-15, 175), true);

            foreach(var message in State.Session.Messages)
            {
                var time = message.DateTime.ToLocalTime().ToString("t");

                ImGui.PushStyleColor(ImGuiCol.Text, message.TextColor);
                ImGui.TextWrapped($"[{time}] {message.DisplayName}: {message.Message}");
                ImGui.PopStyleColor();
            }

            if(State.Session.ScrollMessages)
            {
                ImGui.SetScrollHereY();
                State.Session.ScrollMessages = false;
            }

            ImGui.EndChild();

            ImGui.Spacing();

            ImGui.Text("Send Chat:");
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);

            if (ImGui.InputText("", ref State.stringBuffer, 500, ImGuiInputTextFlags.EnterReturnsTrue))
            {
                var message = State.stringBuffer.Trim();

                if (message.Length > 0)
                {
                    if(message.StartsWith("/"))
                    {
                        string content = Plugin.PluginInterface.Sanitizer.Sanitize(message);

                        State.Plugin.Chat.sendMessage(content);
                    }
                    else
                    {
                        _ = ShellWindowUtilities.SendChat(Plugin, State.Session, message);
                    }                    

                    State.ResetStringBuffer();

                    ImGui.SetWindowFocus();
                    ImGui.SetKeyboardFocusHere(-1);
                }
            }
        }

        private void DrawUISafetyWindow()
        {
            ImGui.Spacing();
            ImGui.Text("Safety Center:");
            var width = ImGui.GetWindowWidth();
            ImGui.BeginChild("##SafetyCenterWindow", new Vector2(width - 15, 90), true);

            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 1f, 1f, 0.7f));
            ImGui.TextWrapped("Need a break? Use these controls to pause commands running on your devices.");
            ImGui.PopStyleColor();

            ImGui.Spacing();

            if (State.Session.SelfUserReceiveCommands)
            {
                if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.Pause, "Pause Commands", new Vector2(140f, 24f)))
                {
                    State.Session.SelfUserReceiveCommands = false;
                }
            }
            else
            {
                if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.Play, "Resume Commands", new Vector2(150f, 24f)))
                {
                    State.Session.SelfUserReceiveCommands = true;
                }
            }

            ImGui.SameLine();

            if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.Square, "Stop Commands", new Vector2(130f, 24f)))
            {
                Plugin.ToyController.StopAllDevices(State.Session, SelfUser);
            }

            ImGui.EndChild();
        }

        private void DrawUICenteredText(string text)
        {
            var windowWidth = ImGui.GetWindowSize().X;
            var textWidth = ImGui.CalcTextSize(text).X;

            ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);
            ImGui.Text(text);
        }

        private void DisconnectFromShell()
        {
            _ = ShellWindowUtilities.DisconnectFromShellWebSocket(Plugin, State.KinkShell);
            var selfUser = ShellWindowUtilities.GetSelf(Plugin, State.Session);

            Plugin.ToyController.StopAllDevices(State.Session, selfUser);

            State = new ShellWindowState(Plugin, State.KinkShell, this);

            Plugin.UIHandler.RemoveShellWindow(this);
        }
    }
}
