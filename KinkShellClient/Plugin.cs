using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using KinkShellClient.Network;
using KinkShellClient.Windows;
using System.IO;
using System.Reflection.Metadata;

namespace KinkShellClient
{
    public sealed class Plugin : IDalamudPlugin
    {
        private DalamudPluginInterface PluginInterface { get; init; }
        public Configuration Configuration { get; init; }

        public CommandHandler CommandHandler { get; init; }
        public UIHandler UIHandler { get; init; }
        public ConnectionHandler ConnectionHandler { get; init; }
        public HTTPHandler HTTPHandler { get; set; }

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] ICommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;

            HTTPHandler = new HTTPHandler(this);
            CommandHandler = new CommandHandler(this, commandManager);
            UIHandler = new UIHandler(this, this.PluginInterface);
            ConnectionHandler = new ConnectionHandler(this);

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);
        }

        public void Dispose()
        {
            CommandHandler.Dispose();
            UIHandler.Dispose();
        }
    }
}