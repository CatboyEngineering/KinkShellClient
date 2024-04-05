using CatboyEngineering.KinkShellClient.Models;
using CatboyEngineering.KinkShellClient.Models.Shell;

namespace CatboyEngineering.KinkShellClient.Windows.Utilities
{
    public class ShellWindowState
    {
        public Plugin Plugin { get; set; }
        public KinkShell KinkShell { get; set; }
        public ShellSession Session { get; set; }
        public ShellWindow Window { get; set; }

        public string stringBuffer;
        public int intBuffer = 0;
        public bool receiveCommands;
        public bool onCooldown;

        public ShellWindowState(Plugin plugin, KinkShell kinkShell, ShellWindow window)
        {
            Plugin = plugin;
            Window = window;

            SetDefauts();
            KinkShell = kinkShell;  
        }

        public void SetDefauts()
        {
            receiveCommands = Session == null || Session.SelfUserReceiveCommands;
            onCooldown = false;
            ResetStringBuffer();
        }

        public void ResetStringBuffer()
        {
            intBuffer = 0;
            stringBuffer = "";
        }
    }
}