﻿using Dalamud.Configuration;
using Dalamud.Plugin;
using KinkShellClient.Models.API.Response;
using KinkShellClient.ShellData;
using System;
using System.Collections.Generic;

namespace KinkShellClient
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public string KinkShellServerAddress { get; set; } = "localhost";
        public string KinkShellServerUsername { get; set; } = "";
        public string KinkShellServerPassword { get; set; } = "";

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
    }
}
