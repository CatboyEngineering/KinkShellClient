using KinkShellClient.ShellData;

namespace KinkShellClient.Windows.Utilities
{
    public class ShellWindowState
    {
        public Plugin Plugin { get; set; }
        public KinkShell KinkShell { get; set; }
        public ShellSession Session { get; set; }

        public byte[] stringByteBuffer;

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
            stringByteBuffer = new byte[500];
        }
    }
}