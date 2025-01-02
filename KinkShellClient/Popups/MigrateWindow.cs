using System;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace CatboyEngineering.KinkShellClient.Popups;

public class MigrateWindow : Window
{
    private Plugin plugin;
    private Configuration Configuration;

    public MigrateWindow(Plugin plugin) : base("Ready to Migrate", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.Popup)
    {
        this.Configuration = plugin.Configuration;
        this.plugin = plugin;
    }

    public override void Draw()
    {
        ImGui.SetNextWindowSize(new Vector2(500, 500), ImGuiCond.Always);

        if (ImGui.Begin("Ready to Migrate"))
        {
            DrawPopupBody();
        }

        ImGui.End();
    }

    private void DrawPopupBody()
    {
        plugin.TitleHeaderFontHandle.Push();
        DrawUICenteredText("KinkShell");
        plugin.TitleHeaderFontHandle.Pop();
        DrawUICenteredText("A Linkshell for your kinks.");
        DrawUICenteredText(new Vector4(0.5f, 0.6f, 0.8f, 1), "https://kinkshell.catboy.engineering/");

        ImGui.Spacing();

        DrawBorderedBody();

        ImGui.Spacing();

        DrawCTA();
    }

    private void DrawCTA()
    {
        ImGui.Text("Ready to get started?");
        ImGui.Spacing();
        ImGui.Text("Choose \"Migrate Now\" to upgrade your account. It even takes less than 5 minutes!");
        ImGui.Spacing();

        DrawMigrateButton();
        ImGui.Spacing();
        DrawDismissButtons();
    }

    private void DrawBorderedBody()
    {
        var width = ImGui.GetWindowWidth();
        ImGui.BeginChild("PopupBodyBorder#Migrate", new Vector2(width - 15, 400), true);

        DrawUICenteredText("KinkShell is getting easier!");

        ImGui.Spacing();

        ImGui.TextColored(new Vector4(0.9f, 0.9f, 0.3f, 1), "What's Happening?");
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 1f, 1f, 0.7f));
        ImGui.TextWrapped("KinkShell has released a major update, making accounts easier than ever to use. Along with an enhanced interface, the new KinkShell is ready for primetime.");
        ImGui.PopStyleColor();

        ImGui.Spacing();

        ImGui.TextColored(new Vector4(0.3f, 0.9f, 0.3f, 1), "No More Passwords");
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 1f, 1f, 0.7f));
        ImGui.TextWrapped("You no longer need to create an account or log in on a separate website. Simply verify your character on the FFXIV Lodestone one time, and you're set!");
        ImGui.PopStyleColor();

        ImGui.Spacing();

        ImGui.TextColored(new Vector4(0.3f, 0.6f, 0.9f, 1), "Improved Interface");
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 1f, 1f, 0.7f));
        ImGui.TextWrapped("KinkShell menus are easier to understand, include more help text, and have helpful icons to make buttons more apparent.");
        ImGui.PopStyleColor();

        ImGui.Spacing();

        ImGui.TextColored(new Vector4(0.9f, 0.5f, 0.5f, 1), "Improved Stability");
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 1f, 1f, 0.7f));
        ImGui.TextWrapped("Several bug fixes and improvements have been added to ensure everything runs as smoothly as possible.");
        ImGui.PopStyleColor();

        ImGui.EndChild();
    }

    private void DrawUICenteredText(string text)
    {
        var windowWidth = ImGui.GetWindowSize().X;
        var textWidth = ImGui.CalcTextSize(text).X;

        ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);
        ImGui.Text(text);
    }

    private void DrawUICenteredText(Vector4 color, string text)
    {
        var windowWidth = ImGui.GetWindowSize().X;
        var textWidth = ImGui.CalcTextSize(text).X;

        ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);
        ImGui.TextColored(color, text);
    }

    private void DrawMigrateButton()
    {
        var windowWidth = ImGui.GetWindowSize().X;
        var settingsTextWidth = ImGui.CalcTextSize("XX Migrate Now").X;

        ImGui.SetCursorPosX((windowWidth - settingsTextWidth) * 0.5f);

        if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.Play, "Migrate Now", new Vector2(145f, 24f)))
        {
            this.IsOpen = false;
            plugin.UIHandler.MainWindow.IsOpen = true;
        }
    }

    private void DrawDismissButtons()
    {
        var windowWidth = ImGui.GetWindowSize().X;
        var settingsTextWidth = ImGui.CalcTextSize("Don't show this againXRemind me later").X;

        ImGui.SetCursorPosX((windowWidth - settingsTextWidth) * 0.5f);

        if (ImGui.Button("Don't show this again"))
        {
            plugin.Configuration.ShowMigrationPopup = false;
            plugin.Configuration.Save();
            this.IsOpen = false;
        }

        ImGui.SameLine();

        if (ImGui.Button("Remind me later"))
        {
            this.IsOpen = false;
        }
    }
}
