﻿namespace CatboyEngineering.KinkShellClient.Models.API.Request
{
    public struct AccountLoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientVersionString { get; set; }
    }
}
