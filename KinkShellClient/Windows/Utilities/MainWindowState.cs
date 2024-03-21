using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinkShellClient.Windows.Utilities
{
    public class MainWindowState
    {
        public Plugin Plugin { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool HasError { get; set; }
        public string ErrorText { get; set; }

        public MainWindowState(Plugin plugin)
        {
            Plugin = plugin;

            SetDefauts();
        }

        public void SetDefauts()
        {
            IsAuthenticated = false;
            HasError = false;
            ErrorText = string.Empty;
        }

        public void OnError(string error)
        {
            HasError = true;
            ErrorText = error;
        }
    }
}