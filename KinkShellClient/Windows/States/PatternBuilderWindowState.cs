using CatboyEngineering.KinkShellClient.Models.Toy;
using CatboyEngineering.KinkShellClient.Windows.States.Models;
using System.Collections.Generic;

namespace CatboyEngineering.KinkShellClient.Windows.States
{
    public class PatternBuilderWindowState
    {
        public Plugin Plugin { get; set; }
        public List<StoredShellCommand> WorkingCommandCopy { get; set; }

        public string stringBuffer;
        public int intBuffer = 0;
        public int selectedPattern;
        public List<PatternStateItem> patternStateItems;

        public PatternBuilderWindowState(Plugin plugin)
        {
            Plugin = plugin;

            SetDefauts();
        }
        public void LoadPatternSteps(StoredShellCommand command)
        {
            patternStateItems = new List<PatternStateItem>();

            foreach (var pattern in command.Instructions)
            {
                patternStateItems.Add(new PatternStateItem(pattern));
            }
        }

        public void SetDefauts()
        {
            ResetBuffers();
            selectedPattern = -1;
            patternStateItems = new List<PatternStateItem>();
            WorkingCommandCopy = new List<StoredShellCommand>(Plugin.Configuration.SavedPatterns);
        }

        public void ResetBuffers()
        {
            intBuffer = 0;
            stringBuffer = "";
        }
    }
}
