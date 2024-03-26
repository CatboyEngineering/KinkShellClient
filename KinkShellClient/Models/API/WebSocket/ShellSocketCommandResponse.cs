﻿using CatboyEngineering.KinkShellClient.Models.Toy;
using System;

namespace CatboyEngineering.KinkShellClient.Models.API.WebSocket
{
    public struct ShellSocketCommandResponse
    {
        public Guid ShellID { get; set; }
        public ShellCommand Command { get; set; }
    }
}
