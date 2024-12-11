using System;
using System.Numerics;
using System.Text.RegularExpressions;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace CatboyEngineering.KinkShellClient.Windows
{
    public class ConfigWindow : Window, IDisposable
    {
        private Plugin plugin;
        private Configuration Configuration;
        private Configuration WorkingCopy;
        private readonly Regex WebServerProtocol = new("^(https?)(:\\/\\/)");
        private readonly Regex IntifacePath = new("^(wss?)(:\\/\\/)[\\w\\d]+[:.\\w\\d/]+$");

        public ConfigWindow(Plugin plugin) : base("KinkShell Configuration", ImGuiWindowFlags.NoResize)
        {
            this.Configuration = plugin.Configuration;
            this.plugin = plugin;
        }

        public override void OnOpen()
        {
            base.OnOpen();
            this.WorkingCopy = Configuration.Clone();
        }

        public override void Draw()
        {
            ImGui.SetNextWindowSize(new Vector2(400, 425), ImGuiCond.Always);

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
                DrawUIChatSettingsTabItem();

                if (plugin.IsDev)
                {
                    DrawUIDevLoginToken();
                }

                ImGui.EndTabBar();
            }
        }

        private void DrawUIKinkshellServerTabItem()
        {
            if (ImGui.BeginTabItem("KinkShell Server"))
            {
                var shellUser = this.WorkingCopy.KinkShellServerUsername;
                var shellPass = this.WorkingCopy.KinkShellServerPassword;

                if (plugin.IsDev)
                {
                    var shellServer = this.WorkingCopy.KinkShellServerAddress;
                    var secure = this.WorkingCopy.KinkShellSecure;

                    if (ImGui.InputText("Server Address", ref shellServer, 64))
                    {
                        this.WorkingCopy.KinkShellServerAddress = WebServerProtocol.Replace(shellServer, "");
                    }

                    if (ImGui.Checkbox("Secure", ref secure))
                    {
                        this.WorkingCopy.KinkShellSecure = secure;
                    }
                }

                ImGui.Text("KinkShell Username:");
                if (ImGui.InputText("##KSUsername", ref shellUser, 64))
                {
                    this.WorkingCopy.KinkShellServerUsername = shellUser;
                }

                ImGui.Text("KinkShell Password:");
                if (ImGui.InputText("##KSPassword", ref shellPass, 64, ImGuiInputTextFlags.Password))
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
                    if (IntifacePath.IsMatch(intifaceServer))
                    {
                        this.WorkingCopy.IntifaceServerAddress = intifaceServer;
                    }
                }

                ImGui.EndTabItem();
            }
        }

        private void DrawUIChatSettingsTabItem()
        {
            if (ImGui.BeginTabItem("Chat Settings"))
            {
                var selfTextColor = this.WorkingCopy.SelfTextColor;

                ImGui.Text("Your Chat Message Color:");

                if (ImGui.ColorPicker4("##ChatColorPicker", ref selfTextColor))
                {
                    this.WorkingCopy.SelfTextColor = selfTextColor;
                }

                ImGui.EndTabItem();
            }
        }

        private void DrawUIDevLoginToken()
        {
            if (ImGui.BeginTabItem("Login"))
            {
                var token = this.WorkingCopy.KinkShellServerLoginToken;

                if (ImGui.InputText("Login Token", ref token, 256))
                {
                    this.WorkingCopy.KinkShellServerLoginToken = token;
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