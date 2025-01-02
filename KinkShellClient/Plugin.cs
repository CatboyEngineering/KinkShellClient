using CatboyEngineering.KinkShellClient.Network;
using CatboyEngineering.KinkShellClient.Toy;
using CatboyEngineering.KinkShellClient.Utilities;
using Dalamud.Game;
using Dalamud.Interface.GameFonts;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using System.Reflection;

namespace CatboyEngineering.KinkShellClient
{
    public sealed class Plugin : IDalamudPlugin
    {
        [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
        [PluginService] internal static IPluginLog Logger { get; private set; } = null!;
        [PluginService] internal static ISigScanner SigScanner { get; private set; } = null!;
        [PluginService] internal static IClientState ClientState { get; private set; } = null!;

        public IFontHandle SmallFontHandle { get; set; }
        public IFontHandle HeaderFontHandle { get; set; }
        public IFontHandle TitleHeaderFontHandle { get; set; }

        public Configuration Configuration { get; }
        public bool IsDev { get; set; }

        public CommandHandler CommandHandler { get; }
        public UIHandler UIHandler { get; }
        public ConnectionHandler ConnectionHandler { get; }
        public HTTPHandler HTTP { get; }
        public ToyController ToyController { get; }
        public Chat Chat { get; set; }

        public static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
        public string Name => "KinkShellClient";

        public Plugin()
        {
            this.Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(PluginInterface);

            IsDev = PluginInterface.IsDev;

            HTTP = new HTTPHandler(this);
            CommandHandler = new CommandHandler(this, CommandManager);
            UIHandler = new UIHandler(this, PluginInterface);
            ConnectionHandler = new ConnectionHandler(this);
            Chat = new Chat();

            ToyController = new ToyController(this);

            SmallFontHandle = PluginInterface.UiBuilder.FontAtlas.NewGameFontHandle(new GameFontStyle(GameFontFamily.Axis, 14f));
            HeaderFontHandle = PluginInterface.UiBuilder.FontAtlas.NewGameFontHandle(new GameFontStyle(GameFontFamily.Axis, 20f));
            TitleHeaderFontHandle = PluginInterface.UiBuilder.FontAtlas.NewGameFontHandle(new GameFontStyle(GameFontFamily.Axis, 36f));

            // TODO: This popup is temporary during the migration period.
            ClientState.Login += OnPlayerLogin;
        }

        public void Dispose()
        {
            CommandHandler.Dispose();
            UIHandler.Dispose();
            ConnectionHandler.Dispose();
            ToyController.Dispose();
        }

        private void OnPlayerLogin()
        {
            if (!Configuration.ShowMigrationPopup)
            {
                return;
            }

            if (!string.IsNullOrEmpty(Configuration.KinkShellServerUsername) && string.IsNullOrEmpty(Configuration.KinkShellServerLoginToken))
            {
                UIHandler.MigrateWindow.IsOpen = true;
            }
        }
    }
}