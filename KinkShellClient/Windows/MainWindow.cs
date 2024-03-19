using Dalamud.Interface.Internal;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using System;
using System.Numerics;

namespace KinkShellClient.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin Plugin;
    private Configuration Configuration;

    public MainWindow(Plugin plugin) : base(
        "KinkShell", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.Plugin = plugin;
        this.Configuration = plugin.Configuration;
    }

    public override void Draw()
    {
        ImGui.SetNextWindowSize(new Vector2(640, 400), ImGuiCond.Appearing);

        if (ImGui.Begin("KinkShell"))
        {
            DrawUIWindowBody();
        }

        ImGui.End();
    }

    public void Dispose() { }

    private void DrawUIWindowBody()
    {
        ImGui.BeginChild("body", new Vector2(0, -ImGui.GetFrameHeightWithSpacing()), false);

        DrawUIServerConfigurationText();
        DrawUIConnectButton();

        ImGui.EndChild();
    }

    private void DrawUIServerConfigurationText()
    {
        ImGui.Text("Server: ");
        ImGui.Indent();
        ImGui.Text(Configuration.KinkShellServerAddress);
        ImGui.Unindent();

        ImGui.Text("Username: ");
        ImGui.Indent();
        ImGui.Text(GetCensoredUsername(Configuration.KinkShellServerUsername));
        ImGui.Unindent();
    }

    private void DrawUIConnectButton()
    {
        if (ImGui.Button("Connect"))
        {
            // TODO authenticate with the server, get an auth token, store in memory only, GET a list of shells, draw them on screen.
            var result = Plugin.ConnectionHandler.Authenticate();

            result.Wait();

            if(result.Result)
            {
                ImGui.Indent();
                ImGui.TextColored(new Vector4(0, 1, 0, 1), "Logged in!");
                ImGui.Unindent();
            }
            else
            {
                ImGui.Indent();
                ImGui.TextColored(new Vector4(1, 0, 0, 1), "Login failed");
                ImGui.Unindent();
            }
        }
    }

    private string GetCensoredUsername(string plaintext)
    {
        if(!plaintext.IsNullOrEmpty())
        {
            var length = plaintext.Length;
            var asterisks = length;
            var start = "";
            var end = "";

            if(length > 3)
            {
                start = plaintext.Substring(0, 3);
                end = plaintext.Substring(length - 3, 3);

                asterisks -= 6;
            }
            
            return start + (new string('*', asterisks)) + end;
        }

        return plaintext;
    }
}
