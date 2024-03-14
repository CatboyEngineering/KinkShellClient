using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using KinkShellClient.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinkShellClient
{
    public class UIHandler : IDisposable
    {
        public WindowSystem WindowSystem = new("KinkShell");

        public Plugin Plugin { get; init; }
        private DalamudPluginInterface PluginInterface { get; init; }

        public ConfigWindow ConfigWindow { get; init; }
        public MainWindow MainWindow { get; init; }

        public UIHandler(Plugin plugin, DalamudPluginInterface pluginInterface)
        {
            this.Plugin = plugin;
            this.PluginInterface = pluginInterface;

            ConfigWindow = new ConfigWindow(this.Plugin);
            MainWindow = new MainWindow(this.Plugin);

            WindowSystem.AddWindow(ConfigWindow);
            WindowSystem.AddWindow(MainWindow);

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            ConfigWindow.IsOpen = true;
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();

            ConfigWindow.Dispose();
            MainWindow.Dispose();
        }
    }
}