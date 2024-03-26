using CatboyEngineering.KinkShellClient.ShellData;

namespace CatboyEngineering.KinkShellClient.Windows.Utilities
{
    public class ShellWindowState
    {
        public Plugin Plugin { get; set; }
        public KinkShell KinkShell { get; set; }
        public ShellSession Session { get; set; }

        public string stringBuffer;
        public int intBuffer = 0;

        public ShellWindowState(Plugin plugin, KinkShell kinkShell)
        {
            Plugin = plugin;

            SetDefauts();
            KinkShell = kinkShell;  
        }

        public void SetDefauts()
        {
            ResetStringBuffer();
        }

        public void ResetStringBuffer()
        {
            intBuffer = 0;
            stringBuffer = "";
        }
    }
}