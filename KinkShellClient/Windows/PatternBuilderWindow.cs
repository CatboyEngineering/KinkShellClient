using CatboyEngineering.KinkShellClient.Models.Toy;
using CatboyEngineering.KinkShellClient.Windows.Utilities;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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
            ImGui.SetNextWindowSize(new Vector2(600, 800), ImGuiCond.Always);

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
            ImGui.Text("Edit Pattern:");

            var itemList = State.WorkingCommandCopy.Select(c => c.Name).ToArray();
            var label = State.selectedPattern >= 0 ? itemList[State.selectedPattern] : "- Select Pattern -";

            if (ImGui.BeginCombo("##selectPattern", label))
            {
                for (int i= 0; i<itemList.Length; i++)
                {
                    var selected = State.selectedPattern == i;

                    if (ImGui.Selectable(State.WorkingCommandCopy[i].Name, selected))
                    {
                        State.selectedPattern = i;

                        var selectedPattern = PatternBuilderWindowUtilities.GetSelectedPattern(this);

                        State.LoadPatternSteps(selectedPattern.Value);
                    }

                    if (selected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }

                ImGui.EndCombo();
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
                ImGui.SetKeyboardFocusHere(1);
                if (ImGui.InputText("##NewPatternName", ref State.stringBuffer, 64, ImGuiInputTextFlags.EnterReturnsTrue))
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
                State.patternStateItems.Add(new PatternStateItem(new Pattern
                {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.5,
                    Duration = 1000,
                    Delay = 1000,
                }));
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
                foreach (var pattern in new List<PatternStateItem>(State.patternStateItems))
                {
                    DrawUIPatternStep(pattern);
                }
            }
        }

        private void DrawUIPatternStep(PatternStateItem pattern)
        {
            ImGui.BeginChild($"PatternStep##{pattern.TrackingID}", new Vector2(200, 125), true);

            if (ImGui.Combo("", ref pattern.patternIntBuffer, Enum.GetNames<PatternType>(), 4))
            {
                pattern.NewPatternType = Enum.GetValues<PatternType>()[pattern.patternIntBuffer];
            }

            ImGui.SameLine();

            if(ImGui.Button("X"))
            {
                State.patternStateItems.Remove(pattern);
            }

            if (ImGui.InputDouble("Intensity", ref pattern.intensityDoubleBuffer, 0.05, 0.25, "%.2f"))
            {
                if(pattern.intensityDoubleBuffer > 1)
                {
                    pattern.intensityDoubleBuffer = 1;
                }

                if (pattern.intensityDoubleBuffer < 0)
                {
                    pattern.intensityDoubleBuffer = 0;
                }

                pattern.NewIntensity = pattern.intensityDoubleBuffer;
            }

            if (ImGui.InputDouble("Seconds", ref pattern.durationDoubleBuffer, 0.05, 0.25, "%.3f"))
            {
                if (pattern.durationDoubleBuffer < 0)
                {
                    pattern.durationDoubleBuffer = 0;
                }

                pattern.NewDuration = (int)(pattern.durationDoubleBuffer * 1000);
            }

            if (ImGui.InputDouble("Pause", ref pattern.delayDoubleBuffer, 0.05, 0.25, "%.3f"))
            {
                if (pattern.delayDoubleBuffer < 0)
                {
                    pattern.delayDoubleBuffer = 0;
                }

                pattern.NewDelay = (int)(pattern.delayDoubleBuffer * 1000);
            }

            ImGui.EndChild();
        }
    }
}
