﻿using CatboyEngineering.KinkShellClient.Models;
using CatboyEngineering.KinkShellClient.Models.API.Response;
using CatboyEngineering.KinkShellClient.Models.API.Response.V2;
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
        public string KinkShellServerAddress { get; set; } = "api2.galsim.io";
        [Obsolete]
        public string KinkShellServerUsername { get; set; } = "";
        [Obsolete]
        public string KinkShellServerPassword { get; set; } = "";
        public string KinkShellServerLoginToken { get; set; } = "";
        public bool KinkShellSecure { get; set; } = true;
        public string IntifaceServerAddress { get; set; } = "ws://localhost:12345";
        public List<StoredShellCommand> SavedPatterns { get; set; } = new List<StoredShellCommand>();
        public Vector4 SelfTextColor { get; set; } = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        public bool ShowMigrationPopup { get; set; } = true;

        // Config Versioning Constants
        // Version will be serialized and restored by the client, but also provides a default value.
        // CurrentVersion exists only in memory to compare the client's deserialized version and perform upgrades.
        public int Version { get; set; } = 4;

        [NonSerialized]
        private readonly int CurrentVersion = 4;
        // End Config Versioning Constants

        [NonSerialized]
        [Obsolete]
        public readonly string CaptchaToken = "DalamudClient";

        [NonSerialized]
        [Obsolete]
        public Models.API.Response.AccountAuthenticatedResponse KinkShellAuthenticatedUserData;

        [NonSerialized]
        public Models.API.Response.V2.AccountAuthenticatedResponse KinkShellUserData;

        [NonSerialized]
        public string RecoveryIntegrityToken;

        [NonSerialized]
        public List<KinkShell> Shells;

        [NonSerialized]
        public List<AccountInfoResponse> AdminUserList;

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
            clone.KinkShellServerLoginToken = this.KinkShellServerLoginToken;
            clone.IntifaceServerAddress = this.IntifaceServerAddress;
            clone.SavedPatterns = this.SavedPatterns;
            clone.SelfTextColor = this.SelfTextColor;
            clone.KinkShellSecure = this.KinkShellSecure;
            clone.ShowMigrationPopup = this.ShowMigrationPopup;

            return clone;
        }

        public void Import(Configuration configuration)
        {
            this.KinkShellServerAddress = configuration.KinkShellServerAddress;
            this.KinkShellServerUsername = configuration.KinkShellServerUsername;
            this.KinkShellServerPassword = configuration.KinkShellServerPassword;
            this.IntifaceServerAddress = configuration.IntifaceServerAddress;
            this.KinkShellServerLoginToken = configuration.KinkShellServerLoginToken;
            this.SavedPatterns = configuration.SavedPatterns;
            this.SelfTextColor = configuration.SelfTextColor;
            this.KinkShellSecure = configuration.KinkShellSecure;
            this.ShowMigrationPopup = configuration.ShowMigrationPopup;
        }

        private void PerformVersionUpdates()
        {
            if (Version == 0)
            {
                Plugin.Logger.Debug($"Upgrading Config to version {1}");

                // This update moves the Intiface server protocol into the configuration.
                Version = 1;

                IntifaceServerAddress = "ws://" + IntifaceServerAddress;
            }

            if (Version == 1)
            {
                Plugin.Logger.Debug($"Upgrading Config to version {2}");

                // This update changes the schema for stored patterns and the API URL
                Version = 2;

                KinkShellServerAddress = "api.catboy.engineering";
                SavedPatterns.Clear();
            }

            if (Version == 2)
            {
                Plugin.Logger.Debug($"Upgrading Config to version {3}");

                // This update adds a popup control
                Version = 3;

                ShowMigrationPopup = true;
            }

            if (Version == 3)
            {
                Plugin.Logger.Debug($"Upgrading Config to version {4}");

                // This update changes the API URL
                Version = 4;

                KinkShellServerAddress = "api2.galsim.io";
            }

            Version = CurrentVersion;

            Save();
        }
    }
}
