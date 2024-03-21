using Dalamud.Interface.Internal;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using KinkShellClient.Windows.Utilities;
using System;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace KinkShellClient.Windows
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

        public override void Draw()
        {
            ImGui.SetNextWindowSize(new Vector2(410, 190), ImGuiCond.Always);

            if (ImGui.Begin("KinkShell"))
            {
                DrawUIWindowBody();
            }

            ImGui.End();
        }

        public void Dispose() { }

        private void DrawUIWindowBody()
        {
            if (!Plugin.Configuration.KinkShellServerUsername.IsNullOrEmpty() && !Plugin.Configuration.KinkShellServerPassword.IsNullOrEmpty() && !Plugin.Configuration.KinkShellServerAddress.IsNullOrEmpty())
            {
                if (!State.IsAuthenticated)
                {
                    DrawUIWindowReadyToConnect();

                    if(State.HasError)
                    {
                        DrawUIErrorText(State.ErrorText);
                    }
                }
                else
                {
                    // TODO Draw authenticated screen
                    ImGui.Text($"Welcome, {Plugin.Configuration.KinkShellAuthenticatedUserData.DisplayName}!");
                    if(ImGui.Button("Log out"))
                    {
                        _ = MainWindowUtilities.LogOut(Plugin, this);
                    }
                }
            } else
            {
                DrawUIWindowNeedsServer();
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

        private void DrawUIWindowReadyToConnect()
        {
            var text = "Press the button below to connect.";

            var windowWidth = ImGui.GetWindowSize().X;
            var textWidth = ImGui.CalcTextSize(text).X;

            ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);
            ImGui.Text(text);

            var settingsTextWidth = ImGui.CalcTextSize("Connect").X;
            ImGui.SetCursorPosX((windowWidth - settingsTextWidth) * 0.5f);
            DrawUIConnectButton();
        }

        private void DrawUIConnectButton()
        {
            if (!Plugin.Configuration.KinkShellServerUsername.IsNullOrEmpty())
            {
                if (ImGui.Button("Connect"))
                {
                    _ = MainWindowUtilities.LogInAndRetrieve(Plugin, this);
                }
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
    }
}