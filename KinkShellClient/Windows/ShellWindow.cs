using System;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KinkShellClient.ShellData;
using KinkShellClient.Windows.Utilities;

namespace KinkShellClient.Windows
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
        }

        public override void Draw()
        {   
            ImGui.SetNextWindowSize(new Vector2(600, 300), ImGuiCond.Always);

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
            // TODO: intiface actions
            ImGui.Text("PLACEHOLDER FOR INTIFACE ACTIONS");
            ImGui.Spacing();

            DrawUIChatWindow();
        }

        private void DrawUIChatWindow()
        {
            var width = ImGui.GetWindowWidth();
            ImGui.BeginChild("ChatWindow", new Vector2(width-10, 150), true);

            foreach(var message in State.Session.Messages)
            {
                ImGui.TextWrapped($"{message.DisplayName}: {message.Message}");
            }

            ImGui.EndChild();

            ImGui.InputText("", State.stringByteBuffer, (uint)State.stringByteBuffer.Length);
            ImGui.SameLine();

            if(ImGui.Button("Send"))
            {
                var message = Encoding.UTF8.GetString(State.stringByteBuffer, 0, State.stringByteBuffer.Length);
                
                if(message.Length > 0)
                {
                    // TODO send message
                    State.ResetStringBuffer();
                }
            }
        }
    }
}
