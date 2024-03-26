using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using CatboyEngineering.KinkShellClient.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient
{
    public class CommandHandler : IDisposable
    {
        private const string CommandName = "/kinkshell";

        public Plugin Plugin { get; }

        private ICommandManager CommandManager { get; }

        public CommandHandler(Plugin plugin, ICommandManager commandManager) {
            this.Plugin = plugin;
            this.CommandManager = commandManager;

            RegisterCommands();
        }

        public void RegisterCommands()
        {
            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Log in and configure KinkShells"
            });
        }

        private void OnCommand(string command, string args)
        {
            Plugin.UIHandler.MainWindow.IsOpen = true;
        }

        public void Dispose()
        {
            this.CommandManager.RemoveHandler(CommandName);
        }
    }
}
