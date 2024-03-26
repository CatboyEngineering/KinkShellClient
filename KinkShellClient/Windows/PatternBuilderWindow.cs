using System;
using System.Linq;
using System.Numerics;
using CatboyEngineering.KinkShellClient.Models.Toy;
using CatboyEngineering.KinkShellClient.Windows.Utilities;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;

namespace CatboyEngineering.KinkShellClient.Windows
{
    public class PatternBuilderWindow : Window, IDisposable
    {
        public Plugin Plugin { get; set; }
        public PatternBuilderWindowState State { get; set; }

        public PatternBuilderWindow(Plugin plugin) : base("Pattern Builder", ImGuiWindowFlags.NoResize)
        {
            this.Plugin = plugin;
            this.State = new PatternBuilderWindowState(plugin);
        }

        public override void OnClose()
        {
            base.OnClose();
            State.SetDefauts();
        }

        public override void Draw()
        {
            ImGui.SetNextWindowSize(new Vector2(800, 600), ImGuiCond.Always);

            if (ImGui.Begin("Pattern Builder"))
            {
                DrawUIWindowBody();
            }

            ImGui.End();
        }

        public void Dispose() { }

        private void DrawUIWindowBody()
        {
            DrawUISectionSelectPattern();
            ImGui.Spacing();
            DrawUISectionPatternSteps();
        }

        private void DrawUISectionSelectPattern()
        {
            ImGui.Text("Select a pattern");
            if (ImGui.Combo("", ref State.intBuffer, State.WorkingCommandCopy.Select(c => c.Name).ToArray(), State.WorkingCommandCopy.Count))
            {
                var selectedPattern = PatternBuilderWindowUtilities.GetSelectedPattern(this);

                if (selectedPattern != null)
                {
                    State.LoadPatternSteps(selectedPattern.Value);
                }
            }

            ImGui.SameLine();

            if (ImGui.Button("New"))
            {
                ImGui.OpenPopup($"kinkshell_createpattern_dialog");
            }

            ImGui.SameLine();

            if (ImGui.Button("Delete"))
            {
                PatternBuilderWindowUtilities.DeleteSelectedPattern(this);
            }

            BuildUIPopupAddPattern();
        }

        private void BuildUIPopupAddPattern()
        {
            if (ImGui.BeginPopup("kinkshell_createpattern_dialog"))
            {
                ImGui.Text("New Pattern Name:");
                if (ImGui.InputText("Pattern Name", ref State.stringBuffer, 64, ImGuiInputTextFlags.EnterReturnsTrue))
                {
                    var newPatternName = State.stringBuffer.Trim();

                    if (!newPatternName.IsNullOrEmpty())
                    {
                        PatternBuilderWindowUtilities.CreateNewPattern(this, newPatternName);
                        ImGui.CloseCurrentPopup();
                    }
                }

                ImGui.EndPopup();
            }
        }

        private void DrawUISectionPatternSteps()
        {
            var selectedPattern = PatternBuilderWindowUtilities.GetSelectedPattern(this);

            if (ImGui.Button("Add Step"))
            {
                selectedPattern?.Instructions.Add(new Pattern
                {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.5,
                    Duration = 1000,
                    Delay = 1000,
                });
            }

            ImGui.SameLine();

            if (ImGui.Button("Save"))
            {
                if (selectedPattern != null)
                {
                    PatternBuilderWindowUtilities.SavePattern(this, selectedPattern.Value);
                }
            }

            ImGui.Spacing();

            if (selectedPattern != null)
            {
                foreach (var pattern in State.patternStateItems)
                {
                    DrawUIPatternStep(pattern);
                }
            }
        }

        private void DrawUIPatternStep(PatternStateItem pattern)
        {
            ImGui.BeginChild($"PatternStep##{pattern.TrackingID}", new Vector2(200, 150), true);

            if (ImGui.Combo("", ref pattern.patternIntBuffer, Enum.GetNames<PatternType>(), 4))
            {
                pattern.NewPatternType = Enum.GetValues<PatternType>()[pattern.patternIntBuffer];
            }

            ImGui.SameLine();

            if(ImGui.Button("X"))
            {
                State.patternStateItems.Remove(pattern);
            }

            if (ImGui.InputDouble("I", ref pattern.intensityDoubleBuffer))
            {
                pattern.NewIntensity = pattern.intensityDoubleBuffer;
            }

            if (ImGui.InputInt("T", ref pattern.durationIntBuffer))
            {
                pattern.NewDuration = pattern.durationIntBuffer;
            }

            if (ImGui.InputInt("D", ref pattern.delayIntBuffer))
            {
                pattern.NewDelay = pattern.delayIntBuffer;
            }

            ImGui.EndChild();
        }
    }
}
