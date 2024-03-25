using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace KinkShellClient.Windows
{
    public class ConfigWindow : Window, IDisposable
    {
        private Configuration Configuration;
        private Configuration WorkingCopy;

        public ConfigWindow(Plugin plugin) : base("KinkShell Configuration", ImGuiWindowFlags.NoResize)
        {
            this.Configuration = plugin.Configuration;
        }

        public override void OnOpen()
        {
            base.OnOpen();
            this.WorkingCopy = Configuration.Clone();
        }

        public override void Draw()
        {   
            ImGui.SetNextWindowSize(new Vector2(410, 190), ImGuiCond.Always);

            if (ImGui.Begin("KinkShell Configuration"))
            {
                DrawUIWindowBody();
                DrawUIWindowFooter();
            }

            ImGui.End();
        }

        public void Dispose() { }

        private void DrawUIWindowBody()
        {
            if (ImGui.BeginTabBar("Connection"))
            {
                DrawUIKinkshellServerTabItem();
                DrawUIIntifaceServerTabItem();
                ImGui.EndTabBar();
            }
        }

        private void DrawUIKinkshellServerTabItem()
        {
            if (ImGui.BeginTabItem("KinkShell Server"))
            {
                var shellServer = this.WorkingCopy.KinkShellServerAddress;
                var shellUser = this.WorkingCopy.KinkShellServerUsername;
                var shellPass = this.WorkingCopy.KinkShellServerPassword;

                if (ImGui.InputText("Server Address", ref shellServer, 64))
                {
                    this.WorkingCopy.KinkShellServerAddress = shellServer;
                }

                if (ImGui.InputText("Server Username", ref shellUser, 64))
                {
                    this.WorkingCopy.KinkShellServerUsername = shellUser;
                }

                if (ImGui.InputText("Server Password", ref shellPass, 64, ImGuiInputTextFlags.Password))
                {
                    this.WorkingCopy.KinkShellServerPassword = shellPass;
                }

                ImGui.EndTabItem();
            }
        }

        private void DrawUIIntifaceServerTabItem()
        {
            if (ImGui.BeginTabItem("Intiface"))
            {
                var intifaceServer = this.WorkingCopy.IntifaceServerAddress;

                if (ImGui.InputText("Address", ref intifaceServer, 64))
                {
                    this.WorkingCopy.IntifaceServerAddress = intifaceServer;
                }

                ImGui.EndTabItem();
            }
        }

        private void DrawUIWindowFooter()
        {
            if (ImGui.Button("Save and Close"))
            {
                Configuration.Import(WorkingCopy);
                Configuration.Save();
                this.IsOpen = false;
            }
        }
    }
}
