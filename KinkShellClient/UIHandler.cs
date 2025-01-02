using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using CatboyEngineering.KinkShellClient.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatboyEngineering.KinkShellClient.Models.Shell;
using CatboyEngineering.KinkShellClient.Windows.MainWindow;
using CatboyEngineering.KinkShellClient.Popups;

namespace CatboyEngineering.KinkShellClient
{
    public class UIHandler : IDisposable
    {
        public WindowSystem WindowSystem = new("KinkShell");

        public Plugin Plugin { get; init; }
        private IDalamudPluginInterface PluginInterface { get; init; }

        public ConfigWindow ConfigWindow { get; init; }
        public MainWindow MainWindow { get; init; }
        public PatternBuilderWindow PatternBuilderWindow { get; set; }

        public MigrateWindow MigrateWindow { get; set; }

        public List<ShellWindow> ShellWindows { get; init; }

        public UIHandler(Plugin plugin, IDalamudPluginInterface pluginInterface)
        {
            this.Plugin = plugin;
            this.PluginInterface = pluginInterface;

            ConfigWindow = new ConfigWindow(this.Plugin);
            MainWindow = new MainWindow(this.Plugin);
            PatternBuilderWindow = new PatternBuilderWindow(this.Plugin);
            MigrateWindow = new MigrateWindow(this.Plugin);
            ShellWindows = new List<ShellWindow>();

            WindowSystem.AddWindow(ConfigWindow);
            WindowSystem.AddWindow(MainWindow);
            WindowSystem.AddWindow(PatternBuilderWindow);
            WindowSystem.AddWindow(MigrateWindow);

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            this.PluginInterface.UiBuilder.OpenMainUi += OpenMainWindow;
        }

        public ShellWindow CreateShellWindow(ShellSession shellSession)
        {
            var existingWindow = ShellWindows.Find(sw => sw.State.KinkShell.ShellID == shellSession.KinkShell.ShellID);

            if (existingWindow != null)
            {
                return existingWindow;
            }
            else
            {
                var newWindow = new ShellWindow(Plugin, shellSession.KinkShell, shellSession);

                ShellWindows.Add(newWindow);
                WindowSystem.AddWindow(newWindow);

                return newWindow;
            }
        }

        public void RemoveShellWindow(ShellWindow window)
        {
            if (ShellWindows.Contains(window))
            {
                WindowSystem.RemoveWindow(window);
            }

            ShellWindows.Remove(window);
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void OpenMainWindow()
        {
            MainWindow.IsOpen = true;
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
            PatternBuilderWindow.Dispose();
        }
    }
}