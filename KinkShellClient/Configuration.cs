using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace KinkShellClient
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public string KinkShellServerAddress { get; set; } = "http://localhost";
        public string KinkShellServerUsername { get; set; } = "";
        public string KinkShellServerPassword { get; set; } = "";

        [NonSerialized]
        private DalamudPluginInterface? PluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.PluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.PluginInterface!.SavePluginConfig(this);
        }
    }
}
