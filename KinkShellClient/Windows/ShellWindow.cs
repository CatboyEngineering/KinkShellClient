using CatboyEngineering.KinkShellClient.ShellData;
using CatboyEngineering.KinkShellClient.Windows.Utilities;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
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
            State = new ShellWindowState(plugin, kinkShell);

            State.Session = session;
        }

        public override void OnClose()
        {
            base.OnClose();

            _ = ShellWindowUtilities.DisconnectFromShellWebSocket(Plugin, State.KinkShell);
            State = new ShellWindowState(Plugin, State.KinkShell);

            Plugin.UIHandler.RemoveShellWindow(this);
        }

        public override void Draw()
        {   
            ImGui.SetNextWindowSize(new Vector2(500, 500), ImGuiCond.Always);

            if (ImGui.Begin(this.WindowName))
            {
                DrawUIWindowBody();
            }

            ImGui.End();
        }

        public void Dispose() { }

        private void DrawUIWindowBody()
        {
            if(ImGui.Button("Leave"))
            {
                _ = ShellWindowUtilities.DisconnectFromShellWebSocket(Plugin, State.KinkShell);
                this.IsOpen = false;
            }

            ImGui.Spacing();
            ImGui.Text("Connected Users:");

            foreach(var user in State.Session.ConnectedUsers)
            {
                ImGui.BulletText(user.DisplayName);
            }

            ImGui.Spacing();
            
            DrawUIPatternCenter();

            ImGui.Spacing();

            DrawUIChatWindow();
        }

        private void DrawUIPatternCenter()
        {
            var width = ImGui.GetWindowWidth();
            ImGui.BeginChild("ToyControlCenter", new Vector2(width - 15, 100), true);

            var userList = ShellWindowUtilities.GetListOfUsers(State.Session);
            if (ImGui.Combo("Target", ref State.intBuffer, userList, userList.Length)) {
                ImGui.Text($"Selected {userList[State.intBuffer]}");
            }

            foreach (var pattern in Plugin.Configuration.SavedPatterns)
            {
                if (ImGui.Button(pattern.Name))
                {
                    var targets = ShellWindowUtilities.GetTargetList(State.intBuffer, userList, State.Session);
                    _ = ShellWindowUtilities.SendCommand(Plugin, State.Session, targets, pattern);
                }
            }

            ImGui.EndChild();
        }

        private void DrawUIChatWindow()
        {
            var width = ImGui.GetWindowWidth();
            ImGui.BeginChild("ChatWindow", new Vector2(width-15, 175), true);

            foreach(var message in State.Session.Messages)
            {
                var time = message.DateTime.ToLocalTime().ToString("t");
                ImGui.TextWrapped($"[{time}] {message.DisplayName}: {message.Message}");
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
    }
}
