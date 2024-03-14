using Dalamud.Interface.Internal;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Numerics;

namespace KinkShellClient.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin Plugin;

    public MainWindow(Plugin plugin) : base(
        "My Test Window", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.Plugin = plugin;
    }

    public override void Draw()
    {
        // TODO use pattern from configwindow. Use the connect button here
        if (ImGui.Button("Show Settings"))
        {
            this.Plugin.UIHandler.DrawConfigUI();
        }

        ImGui.Spacing();
    }

    public void Dispose() { }
}
