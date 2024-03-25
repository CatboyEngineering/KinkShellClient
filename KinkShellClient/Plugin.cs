﻿using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using KinkShellClient.Network;
using XivCommon;

namespace KinkShellClient
{
    public sealed class Plugin : IDalamudPlugin
    {
        private DalamudPluginInterface PluginInterface { get; }
        public Configuration Configuration { get; }

        public CommandHandler CommandHandler { get; }
        public UIHandler UIHandler { get; }
        public ConnectionHandler ConnectionHandler { get; }
        public HTTPHandler HTTP { get; }
        public XivCommonBase Common { get; set; }

        public IChatGui ChatGUI { get; set; }

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] ICommandManager commandManager, [RequiredVersion("1.0")] IChatGui chatGui)
        {
            this.Common = new(pluginInterface);
            this.PluginInterface = pluginInterface;
            this.ChatGUI = chatGui;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            HTTP = new HTTPHandler(this);
            CommandHandler = new CommandHandler(this, commandManager);
            UIHandler = new UIHandler(this, this.PluginInterface);
            ConnectionHandler = new ConnectionHandler(this);
        }

        public void Dispose()
        {
            CommandHandler.Dispose();
            UIHandler.Dispose();
        }
    }
}