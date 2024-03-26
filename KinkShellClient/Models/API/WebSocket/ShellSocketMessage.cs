using Newtonsoft.Json.Linq;

namespace CatboyEngineering.KinkShellClient.Models.API.WebSocket
{
    public struct ShellSocketMessage
    {
        public ShellSocketMessageType MessageType { get; set; }
        public JObject MessageData { get; set; }
    }
}
