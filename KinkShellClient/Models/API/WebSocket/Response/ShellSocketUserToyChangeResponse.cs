using CatboyEngineering.KinkShellClient.Toy;
using System;
using System.Collections.Generic;

namespace CatboyEngineering.KinkShellClient.Models.API.WebSocket.Response
{
    public struct ShellSocketUserToyChangeResponse
    {
        public Guid ShellID { get; set; }
        public Guid UserID { get; set; }
        public List<ToyProperties> Toys { get; set; }
    }
}
