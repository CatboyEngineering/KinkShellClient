﻿using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using KinkShellClient.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinkShellClient
{
    public class CommandHandler : IDisposable
    {
        private const string CommandName = "/kinkshell";

        public Plugin Plugin { get; init; }

        private ICommandManager CommandManager { get; init; }

        public CommandHandler(Plugin plugin, ICommandManager commandManager) {
            this.Plugin = plugin;
            this.CommandManager = commandManager;
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