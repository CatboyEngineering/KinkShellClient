using CatboyEngineering.KinkShellClient.Models.Toy;
using System;
using System.Collections.Generic;

namespace CatboyEngineering.KinkShellClient.Windows.Utilities
{
    public class PatternBuilderWindowUtilities
    {
        public static StoredShellCommand? GetSelectedPattern(PatternBuilderWindow window)
        {
            try
            {
                return window.State.WorkingCommandCopy[window.State.selectedPattern];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void DeleteSelectedPattern(PatternBuilderWindow window)
        {
            var pattern = GetSelectedPattern(window);

            if (pattern != null)
            {
                window.Plugin.Configuration.SavedPatterns.Remove(pattern.Value);
                window.Plugin.Configuration.Save();

                window.State.SetDefauts();
            }
        }

        public static void CreateNewPattern(PatternBuilderWindow window, string name)
        {
            if (!window.Plugin.Configuration.SavedPatterns.Exists(sp => sp.Name.Equals(name)))
            {
                var newPattern = new StoredShellCommand
                {
                    Name = name,
                    Instructions = new List<Pattern>()
                };

                window.Plugin.Configuration.SavedPatterns.Add(newPattern);
                window.State.SetDefauts();
                window.State.selectedPattern = window.Plugin.Configuration.SavedPatterns.IndexOf(newPattern);
            }
        }

        public static void SavePattern(PatternBuilderWindow window, StoredShellCommand storedShellCommand)
        {
            var newPatternSteps = window.State.patternStateItems;

            if (newPatternSteps.Count > 0)
            {
                storedShellCommand.Instructions.Clear();

                foreach (var step in newPatternSteps)
                {
                    storedShellCommand.Instructions.Add(new Pattern
                    {
                        PatternType = step.NewPatternType,
                        VibrateIntensity = step.NewVibrateIntensity,
                        OscillateIntensity = step.NewOscillateIntensity,
                        LinearPosition = step.NewLinearPosition,
                        RotateSpeed = step.NewRotateSpeed,
                        RotateClockwise = step.NewRotateClockwise,
                        InflateAmount = step.NewInflateAmount,
                        ConstrictAmount = step.NewConstrictAmount,
                        Duration = step.NewDuration
                    });
                }

                window.Plugin.Configuration.SavedPatterns = window.State.WorkingCommandCopy;
                window.Plugin.Configuration.Save();
                window.State.SetDefauts();
            }
        }
    }
}