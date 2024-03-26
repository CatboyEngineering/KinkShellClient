﻿using CatboyEngineering.KinkShellClient.Models.API.Response;
using CatboyEngineering.KinkShellClient.Models.Toy;
using CatboyEngineering.KinkShellClient.ShellData;
using CatboyEngineering.KinkShellClient.Toy;
using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;

namespace CatboyEngineering.KinkShellClient
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public string KinkShellServerAddress { get; set; } = "localhost";
        public string KinkShellServerUsername { get; set; } = "";
        public string KinkShellServerPassword { get; set; } = "";
        public string IntifaceServerAddress { get; set; } = "localhost:12345";
        public List<StoredShellCommand> SavedPatterns { get; set; } = new List<StoredShellCommand>();

        [NonSerialized]
        public AccountAuthenticatedResponse KinkShellAuthenticatedUserData;

        [NonSerialized]
        public List<KinkShell> Shells;

        [NonSerialized]
        private DalamudPluginInterface PluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.PluginInterface = pluginInterface;
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

            return clone;
        }

        public void Import(Configuration configuration)
        {
            this.KinkShellServerAddress = configuration.KinkShellServerAddress;
            this.KinkShellServerUsername = configuration.KinkShellServerUsername;
            this.KinkShellServerPassword = configuration.KinkShellServerPassword;
            this.IntifaceServerAddress = configuration.IntifaceServerAddress;
            this.SavedPatterns = configuration.SavedPatterns;
        }
    }
}
