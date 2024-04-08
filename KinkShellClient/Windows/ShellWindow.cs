using CatboyEngineering.KinkShellClient.Models;
using CatboyEngineering.KinkShellClient.Models.Shell;
using CatboyEngineering.KinkShellClient.Models.Toy;
using CatboyEngineering.KinkShellClient.Toy;
using CatboyEngineering.KinkShellClient.Windows.Utilities;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace CatboyEngineering.KinkShellClient.Windows
{
    public class ShellWindow : Window, IDisposable
    {
        public Plugin Plugin { get; set; }
        public ShellWindowState State { get; set; }

        public ShellWindow(Plugin plugin, KinkShell kinkShell, ShellSession session) : base(kinkShell.ShellName, ImGuiWindowFlags.NoResize)
        {
            Plugin = plugin;
            State = new ShellWindowState(plugin, kinkShell, this);

            State.Session = session;
        }

        public override void OnClose()
        {
            base.OnClose();

            _ = ShellWindowUtilities.DisconnectFromShellWebSocket(Plugin, State.KinkShell);
            State = new ShellWindowState(Plugin, State.KinkShell, this);

            Plugin.UIHandler.RemoveShellWindow(this);
        }

        public override void Draw()
        {   
            ImGui.SetNextWindowSize(new Vector2(500, 675), ImGuiCond.Always);

            if (ImGui.Begin(this.WindowName))
            {
                DrawUIWindowBody();
            }

            ImGui.End();
        }

        public void Dispose() { }

        private void DrawUIWindowBody()
        {
            if(ImGui.Button("Leave Session"))
            {
                _ = ShellWindowUtilities.DisconnectFromShellWebSocket(Plugin, State.KinkShell);
                this.IsOpen = false;
            }

            ImGui.SameLine();

            if (ImGui.Button("Pattern Builder"))
            {
                Plugin.UIHandler.PatternBuilderWindow.IsOpen = true;
            }

            ImGui.Spacing();
            ImGui.Text("Connected Users:");

            foreach(var user in State.Session.ConnectedUsers)
            {
                ImGui.BulletText(user.DisplayName);
            }

            ImGui.Spacing();

            if (ShellWindowUtilities.GetSelf(Plugin, State.Session).SendCommands)
            {
                DrawUIPatternCenter();
            }

            ImGui.Spacing();

            DrawUIChatWindow();
            DrawUISafetyWindow();
        }

        private void DrawUIPatternCenter()
        {
            ImGui.Text("Command Center:");
            var width = ImGui.GetWindowWidth();
            ImGui.BeginChild("ToyControlCenter", new Vector2(width - 15, 150), true);

            foreach(var user in State.Session.ConnectedUsers)
            {
                DrawUIUserCommand(user);
            }

            ImGui.EndChild();
        }

        private void DrawUIUserCommand(KinkShellMember user)
        {
            ImGui.BeginChild($"{user.DisplayName}", new Vector2(250, 150), true);
            DrawPopupCommandUser(user);

            ImGui.Text(user.DisplayName);

            // TODO would be cool to keep the Everyone option. Or, if not, change the List to be a single GUID
            if(ImGui.Button("Send Command"))
            {
                ImGui.OpenPopup($"command_user_{user.AccountID}");
            }

            ImGui.Text("Running commands:");
            foreach(var runningCommand in user.RunningCommands)
            {
                ImGui.BulletText($"{runningCommand.CommandName}##{runningCommand.CommandInstanceID}");
            }

            ImGui.EndChild();
        }

        private void DrawPopupCommandUser(KinkShellMember user)
        {
            if (ImGui.BeginPopup($"command_user_{user.AccountID}"))
            {
                ImGui.Text($"Command {user.DisplayName}");

                ImGui.Text("Toy:");
                ImGui.SameLine();

                foreach(var toy in user.Toys)
                {
                    ImGui.Text("Toy: " + toy.DisplayName);
                }

                // TODO: following line fails with "Value cannot be null: parameter chars
                //ImGui.Combo("##UserToyCombo", ref State.intBuffer, user.Toys.Select(t => t.DisplayName).ToArray(), user.Toys.Length);

                if (State.onCooldown)
                {
                    ImGui.BeginDisabled();
                }

                // TODO
                // gray out buttons that aren't able to be run based on what this user has connected
                // patterns may need a UsesVibrate() function to equal toy's Vibrate > 0
                foreach (var pattern in ShellWindowUtilities.GetAvailableShellCommands(Plugin))
                {
                    if (ImGui.Button($"{pattern.Name}"))
                    {
                        var targets = new List<Guid>() { user.AccountID };
                        var toy = user.Toys[State.intBuffer];

                        _ = ShellWindowUtilities.SendCommand(Plugin, State.Session, targets, toy.DeviceInstanceID, pattern);
                        _ = ShellWindowUtilities.Cooldown(this);
                    }
                }

                if (State.onCooldown)
                {
                    ImGui.EndDisabled();
                }

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
                        string content = Plugin.Common.Functions.Chat.SanitiseText(message);
                        Plugin.Common.Functions.Chat.SendMessage(content);
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
            ImGui.BeginChild("##SafetyCenterWindow", new Vector2(width - 15, 75), true);

            ImGui.Text("Receive Intiface Commands:");
            ImGui.SameLine();

            if(ImGui.Checkbox("##ReceiveCommands", ref State.receiveCommands))
            {
                State.Session.SelfUserReceiveCommands = State.receiveCommands;
            }

            if(ImGui.Button("Stop Current Pattern##StopPattern"))
            {
                Plugin.ToyController.StopAllDevices();
            }

            ImGui.EndChild();
        }
    }
}
