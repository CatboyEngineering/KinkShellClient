﻿using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace KinkShellClient.Windows
{
    public class ConfigWindow : Window, IDisposable
    {
        private Configuration Configuration;

        public ConfigWindow(Plugin plugin) : base("KinkShell Configuration")
        {
            this.Configuration = plugin.Configuration;
        }

        public override void Draw()
        {
            if (ImGui.Begin("KinkShell Configuration"))
            {
                DrawUIWindowBody();
            }

            ImGui.End();
        }

        public void Dispose() { }

        private void DrawUIWindowBody()
        {
            ImGui.BeginChild("body");
            ImGui.Indent(1);

            if (ImGui.BeginTabBar("Connection", ImGuiTabBarFlags.None))
            {
                DrawUIServerTabItem();
                ImGui.EndTabBar();
            }

            ImGui.Unindent(1);
            ImGui.EndChild();

            DrawUISaveButton();
        }

        private void DrawUIServerTabItem()
        {
            if (ImGui.BeginTabItem("Server"))
            {
                var shellServer = this.Configuration.KinkShellServerAddress;
                var shellUser = this.Configuration.KinkShellServerUsername;
                var shellPass = this.Configuration.KinkShellServerPassword;

                if (ImGui.InputText("KinkShell Server Address", ref shellServer, 64))
                {
                    this.Configuration.KinkShellServerAddress = shellServer;
                }

                if (ImGui.InputText("KinkShell Server Username", ref shellUser, 64))
                {
                    this.Configuration.KinkShellServerUsername = shellUser;
                }

                if (ImGui.InputText("KinkShell Server Password", ref shellPass, 64))
                {
                    this.Configuration.KinkShellServerPassword = shellPass;
                }

                //if (ImGui.Button("Connect"))
                //{
                //    // TODO connect
                //}

                ImGui.EndTabItem();
            }
        }

        private void DrawUISaveButton()
        {
            if (ImGui.Button("Save"))
            {
                Configuration.Save();
            }
        }
    }
}
