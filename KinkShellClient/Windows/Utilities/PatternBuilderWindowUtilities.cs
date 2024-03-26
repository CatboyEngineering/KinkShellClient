using CatboyEngineering.KinkShellClient.Models.Toy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Windows.Utilities
{
    public class PatternBuilderWindowUtilities
    {
        public static StoredShellCommand? GetSelectedPattern(PatternBuilderWindow window)
        {
            try
            {
                return window.State.WorkingCommandCopy[window.State.intBuffer];
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
            var newPattern = new StoredShellCommand
            {
                Name = name,
                Instructions = new List<Pattern>()
            };

            window.Plugin.Configuration.SavedPatterns.Add(newPattern);
            window.State.SetDefauts();
            window.State.intBuffer = window.Plugin.Configuration.SavedPatterns.IndexOf(newPattern);
        }

        public static void SavePattern(PatternBuilderWindow window, StoredShellCommand storedShellCommand)
        {
            var newPatternSteps = window.State.patternStateItems;

            storedShellCommand.Instructions.Clear();

            foreach (var step in newPatternSteps)
            {
                storedShellCommand.Instructions.Add(new Pattern
                {
                    PatternType = step.NewPatternType,
                    Intensity = step.NewIntensity,
                    Duration = step.NewDuration,
                    Delay = step.NewDelay
                });
            }

            window.Plugin.Configuration.SavedPatterns = window.State.WorkingCommandCopy;
            window.Plugin.Configuration.Save();
            window.State.SetDefauts();
        }
    }
}
