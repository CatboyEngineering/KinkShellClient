using CatboyEngineering.KinkShellClient.Models.Toy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Windows.Utilities
{
    public class PatternBuilderWindowState
    {
        public Plugin Plugin { get; set; }
        public List<StoredShellCommand> WorkingCommandCopy { get; set; }

        public string stringBuffer;
        public int intBuffer = 0;
        public List<PatternStateItem> patternStateItems;

        public PatternBuilderWindowState(Plugin plugin)
        {
            this.Plugin = plugin;

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
