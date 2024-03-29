using CatboyEngineering.KinkShellClient.Network;
using CatboyEngineering.KinkShellClient.Toy;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using System.Reflection;
using XivCommon;

namespace CatboyEngineering.KinkShellClient
{
    public sealed class Plugin : IDalamudPlugin
    {
        private DalamudPluginInterface PluginInterface { get; }
        public Configuration Configuration { get; }

        public CommandHandler CommandHandler { get; }
        public UIHandler UIHandler { get; }
        public ConnectionHandler ConnectionHandler { get; }
        public HTTPHandler HTTP { get; }
        public XivCommonBase Common { get; }
        public ToyController ToyController { get; }
        public IPluginLog Logger { get; set; }

        public static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
        public string Name => "KinkShellClient";

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] ICommandManager commandManager,
            [RequiredVersion("1.0")] IPluginLog pluginLog)
        {
            this.Logger = pluginLog;
            this.Common = new(pluginInterface);
            this.PluginInterface = pluginInterface;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            HTTP = new HTTPHandler(this);
            CommandHandler = new CommandHandler(this, commandManager);
            UIHandler = new UIHandler(this, this.PluginInterface);
            ConnectionHandler = new ConnectionHandler(this);

            ToyController = new ToyController(this);
        }

        public void Dispose()
        {
            CommandHandler.Dispose();
            UIHandler.Dispose();
            ConnectionHandler.Dispose();
            ToyController.Dispose();
        }
    }
}