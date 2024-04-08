using CatboyEngineering.KinkShellClient.Models.Toy;
using CatboyEngineering.KinkShellClient.Windows.States;
using CatboyEngineering.KinkShellClient.Windows.States.Models;
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
                    VibrateIntensity = new double[] { 0, 0 },
                    Duration = 1000
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

            ImGui.SameLine();

            if(ImGui.Button("Copy"))
            {
                State.patternStateItems.Add(new PatternStateItem(new Pattern
                {
                    PatternType = pattern.NewPatternType,
                    VibrateIntensity = pattern.NewVibrateIntensity,
                    OscillateIntensity = pattern.NewOscillateIntensity,
                    LinearPosition = pattern.NewLinearPosition,
                    RotateClockwise = pattern.NewRotateClockwise,
                    RotateSpeed = pattern.NewRotateSpeed,
                    InflateAmount = pattern.NewInflateAmount,
                    ConstrictAmount = pattern.NewConstrictAmount,
                    Duration = pattern.NewDuration
                }));
            }

            switch(pattern.NewPatternType)
            {
                case PatternType.CONSTRICT:
                    if (ImGui.InputDouble("Intensity##ConstrictIntensity", ref pattern.constrictAmountBuffer, 0.05, 0.25, "%.2f"))
                    {
                        if (pattern.constrictAmountBuffer > 1)
                        {
                            pattern.constrictAmountBuffer = 1;
                        }

                        if (pattern.constrictAmountBuffer < 0)
                        {
                            pattern.constrictAmountBuffer = 0;
                        }

                        pattern.NewConstrictAmount = pattern.constrictAmountBuffer;
                    }

                    break;
                case PatternType.INFLATE:
                    if (ImGui.InputDouble("Intensity##InflateIntensity", ref pattern.inflateAmountBuffer, 0.05, 0.25, "%.2f"))
                    {
                        if (pattern.inflateAmountBuffer > 1)
                        {
                            pattern.inflateAmountBuffer = 1;
                        }

                        if (pattern.inflateAmountBuffer < 0)
                        {
                            pattern.inflateAmountBuffer = 0;
                        }

                        pattern.NewInflateAmount = pattern.inflateAmountBuffer;
                    }

                    break;
                case PatternType.LINEAR:
                    if (ImGui.InputDouble("Position##LinearPosition", ref pattern.linearPositionBuffer, 0.05, 0.25, "%.2f"))
                    {
                        if (pattern.linearPositionBuffer > 1)
                        {
                            pattern.linearPositionBuffer = 1;
                        }

                        if (pattern.linearPositionBuffer < 0)
                        {
                            pattern.linearPositionBuffer = 0;
                        }

                        pattern.NewLinearPosition = pattern.linearPositionBuffer;
                    }

                    break;
                case PatternType.OSCILLATE:
                    if (ImGui.InputDouble("Intensity 1##OscillateIntensityA", ref pattern.oscillateIntensityBuffer1, 0.05, 0.25, "%.2f"))
                    {
                        if (pattern.oscillateIntensityBuffer1 > 1)
                        {
                            pattern.oscillateIntensityBuffer1 = 1;
                        }

                        if (pattern.oscillateIntensityBuffer1 < 0)
                        {
                            pattern.oscillateIntensityBuffer1 = 0;
                        }

                        pattern.NewOscillateIntensity = new double[] { pattern.oscillateIntensityBuffer1, pattern.oscillateIntensityBuffer2 };
                    }

                    ImGui.SameLine();

                    if (ImGui.InputDouble("Intensity 2##OscillateIntensityB", ref pattern.oscillateIntensityBuffer2, 0.05, 0.25, "%.2f"))
                    {
                        if (pattern.oscillateIntensityBuffer2 > 1)
                        {
                            pattern.oscillateIntensityBuffer2 = 1;
                        }

                        if (pattern.oscillateIntensityBuffer2 < 0)
                        {
                            pattern.oscillateIntensityBuffer2 = 0;
                        }

                        pattern.NewOscillateIntensity = new double[] { pattern.oscillateIntensityBuffer1, pattern.oscillateIntensityBuffer2 };
                    }

                    break;
                case PatternType.ROTATE:
                    if (ImGui.InputDouble("Speed##RotateSpeed", ref pattern.rotateSpeedBuffer, 0.05, 0.25, "%.2f"))
                    {
                        if (pattern.rotateSpeedBuffer > 1)
                        {
                            pattern.rotateSpeedBuffer = 1;
                        }

                        if (pattern.rotateSpeedBuffer < 0)
                        {
                            pattern.rotateSpeedBuffer = 0;
                        }

                        pattern.NewRotateSpeed = pattern.rotateSpeedBuffer;
                    }

                    if(ImGui.Checkbox("Clockwise##RotateClockwise", ref pattern.rotateClockwiseBuffer))
                    {
                        pattern.NewRotateClockwise = pattern.rotateClockwiseBuffer;
                    }

                    break;
                case PatternType.VIBRATE:
                    if (ImGui.InputDouble("Intensity 1##VibrateIntensityA", ref pattern.vibrateIntensityBuffer1, 0.05, 0.25, "%.2f"))
                    {
                        if (pattern.vibrateIntensityBuffer1 > 1)
                        {
                            pattern.vibrateIntensityBuffer1 = 1;
                        }

                        if (pattern.vibrateIntensityBuffer1 < 0)
                        {
                            pattern.vibrateIntensityBuffer1 = 0;
                        }

                        pattern.NewVibrateIntensity = new double[] { pattern.vibrateIntensityBuffer1, pattern.vibrateIntensityBuffer2 };
                    }

                    ImGui.SameLine();

                    if (ImGui.InputDouble("Intensity 2##VibrateIntensityB", ref pattern.vibrateIntensityBuffer2, 0.05, 0.25, "%.2f"))
                    {
                        if (pattern.vibrateIntensityBuffer2 > 1)
                        {
                            pattern.vibrateIntensityBuffer2 = 1;
                        }

                        if (pattern.vibrateIntensityBuffer2 < 0)
                        {
                            pattern.vibrateIntensityBuffer2 = 0;
                        }

                        pattern.NewVibrateIntensity = new double[] { pattern.vibrateIntensityBuffer1, pattern.vibrateIntensityBuffer2 };
                    }

                    break;
            }

            if (ImGui.InputDouble("Seconds", ref pattern.durationDoubleBuffer, 0.05, 0.25, "%.2f"))
            {
                if (pattern.durationDoubleBuffer < 0)
                {
                    pattern.durationDoubleBuffer = 0;
                }

                pattern.NewDuration = (int)(pattern.durationDoubleBuffer * 1000);
            }

            ImGui.EndChild();
        }
    }
}
