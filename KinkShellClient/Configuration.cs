using CatboyEngineering.KinkShellClient.Models;
using CatboyEngineering.KinkShellClient.Models.API.Response;
using CatboyEngineering.KinkShellClient.Models.Toy;
using CatboyEngineering.KinkShellClient.Toy;
using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace CatboyEngineering.KinkShellClient
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public string KinkShellServerAddress { get; set; } = "api.catboy.engineering";
        public string KinkShellServerUsername { get; set; } = "";
        public string KinkShellServerPassword { get; set; } = "";
        public bool KinkShellSecure { get; set; } = true;
        public string IntifaceServerAddress { get; set; } = "ws://localhost:12345";
        public List<StoredShellCommand> SavedPatterns { get; set; } = new List<StoredShellCommand>();
        public Vector4 SelfTextColor { get; set; } = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

        public int Version { get; set; } = 2;

        [NonSerialized]
        private readonly int CurrentVersion = 2;

        [NonSerialized]
        public readonly string CaptchaToken = "DalamudClient";

        [NonSerialized]
        public AccountAuthenticatedResponse KinkShellAuthenticatedUserData;

        [NonSerialized]
        public List<KinkShell> Shells;

        [NonSerialized]
        private IDalamudPluginInterface PluginInterface;

        public void Initialize(IDalamudPluginInterface pluginInterface)
        {
            this.PluginInterface = pluginInterface;

            PerformVersionUpdates();
        }

        public void Save()
        {
            this.PluginInterface!.SavePluginConfig(this);
        }

        public Configuration Clone()
        {
            var clone = new Configuration();

            clone.KinkShellServerAddress = this.KinkShellServerAddress;
            clone.KinkShellServerUsername = this.KinkShellServerUsername;
            clone.KinkShellServerPassword = this.KinkShellServerPassword;
            clone.IntifaceServerAddress = this.IntifaceServerAddress;
            clone.SavedPatterns = this.SavedPatterns;
            clone.SelfTextColor = this.SelfTextColor;
            clone.KinkShellSecure = this.KinkShellSecure;

            return clone;
        }

        public void Import(Configuration configuration)
        {
            this.KinkShellServerAddress = configuration.KinkShellServerAddress;
            this.KinkShellServerUsername = configuration.KinkShellServerUsername;
            this.KinkShellServerPassword = configuration.KinkShellServerPassword;
            this.IntifaceServerAddress = configuration.IntifaceServerAddress;
            this.SavedPatterns = configuration.SavedPatterns;
            this.SelfTextColor = configuration.SelfTextColor;
            this.KinkShellSecure = configuration.KinkShellSecure;
        }

        private void PerformVersionUpdates()
        {
            if(Version == 0)
            {
                // This update moves the Intiface server protocol into the configuration.
                Version = CurrentVersion;

                IntifaceServerAddress = "ws://" + IntifaceServerAddress;

                Save();
            }

            if (Version == 1)
            {
                // This update changes the schema for stored patterns and the API URL
                Version = CurrentVersion;

                KinkShellServerAddress = "api.catboy.engineering";
                SavedPatterns.Clear();

                Save();
            }
        }
    }
}
