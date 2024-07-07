using CatboyEngineering.KinkShellClient.Models;
using CatboyEngineering.KinkShellClient.Models.Shell;
using CatboyEngineering.KinkShellClient.Models.Toy;
using CatboyEngineering.KinkShellClient.Toy;
using CatboyEngineering.KinkShellClient.Windows.Utilities;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

            DisconnectFromShell();
        }

        public override void Draw()
        {   
            ImGui.SetNextWindowSize(new Vector2(500, 750), ImGuiCond.Always);

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
                this.IsOpen = false;
                OnClose();

                return;
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

            var selfUser = ShellWindowUtilities.GetSelf(Plugin, State.Session);

            if (selfUser.SendCommands)
            {
                DrawUIPatternCenter();
            }

            ImGui.Spacing();

            DrawUIChatWindow();
            DrawUISafetyWindow(selfUser);
        }

        private void DrawUIPatternCenter()
        {
            ImGui.Text("Command Center:");
            var width = ImGui.GetWindowWidth();
            ImGui.BeginChild("ToyControlCenter", new Vector2(width - 15, 200), true);


            for(var i=0; i< State.Session.ConnectedUsers.Count; i++)
            {
                var user = State.Session.ConnectedUsers[i];

                if(i % 2 != 0)
                {
                    ImGui.SameLine();
                }

                DrawUIUserCommand(user);
            }

            ImGui.EndChild();
        }

        private void DrawUIUserCommand(KinkShellMember user)
        {
            ImGui.BeginChild($"{user.DisplayName}", new Vector2(200, 100), true);
            DrawPopupCommandUser(user);

            ImGui.Text(user.DisplayName);

            if (user.Toys.Count > 0)
            {
                if (ImGui.Button("Send Command"))
                {
                    ImGui.OpenPopup($"command_user_{user.AccountID}");
                }
            }

            ImGui.Text("Running commands:");
            foreach(var runningCommand in user.RunningCommands)
            {
                ImGui.BulletText($"{runningCommand.CommandName}");
            }

            ImGui.EndChild();
        }

        private void DrawPopupCommandUser(KinkShellMember user)
        {
            if (ImGui.BeginPopup($"command_user_{user.AccountID}"))
            {
                ImGui.Text($"Command {user.DisplayName}");

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
                ImGui.Text($"Disabled commands are not supported by {user.DisplayName}'s device.");

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
                        // TODO: This is disabled temporarily due to XIVCommon dependency issue.
                        //string content = Plugin.PluginInterface.Sanitizer.Sanitize(message);
                        //Plugin.Common.Functions.Chat.SendMessage(content);
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

        private void DrawUISafetyWindow(KinkShellMember selfUser)
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
                Plugin.ToyController.StopAllDevices(State.Session, selfUser);
            }

            ImGui.EndChild();
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
