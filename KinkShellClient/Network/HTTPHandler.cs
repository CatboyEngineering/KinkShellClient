using KinkShellClient.ShellData;
using KinkShellClient.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KinkShellClient.Network
{
    public class HTTPHandler
    {
        public HttpClient Http { get; }
        public Plugin Plugin { get; }

        public HTTPHandler(Plugin plugin)
        {
            Plugin = plugin;
            Http = new HttpClient()
            {
                BaseAddress = new Uri("http://" + Plugin.Configuration.KinkShellServerAddress + "/v1/")
            };
        }

        public async Task<T?> Get<T>(string uri) where T : struct
        {
            using HttpResponseMessage response = await Http.GetAsync(uri);
            return MapJSONToType<T>(await response.Content.ReadAsStringAsync());
        }

        public async Task<T?> Post<T>(string uri, JObject body) where T : struct
        {
            using StringContent jsonContent = new(
               body.ToString(),
               Encoding.UTF8,
               "application/json");

            using HttpResponseMessage response = await Http.PostAsync(uri, jsonContent);
            return MapJSONToType<T>(await response.Content.ReadAsStringAsync());
        }

        public async Task<T?> Put<T>(string uri, JObject body) where T : struct
        {
            using StringContent jsonContent = new(
               body.ToString(),
               Encoding.UTF8,
               "application/json");

            using HttpResponseMessage response = await Http.PutAsync(uri, jsonContent);
            return MapJSONToType<T>(await response.Content.ReadAsStringAsync());
        }

        public async Task<T?> Patch<T>(string uri, JObject body) where T : struct
        {
            using StringContent jsonContent = new(
               body.ToString(),
               Encoding.UTF8,
               "application/json");

            using HttpResponseMessage response = await Http.PatchAsync(uri, jsonContent);
            return MapJSONToType<T>(await response.Content.ReadAsStringAsync());
        }

        public async Task<T?> Delete<T>(string uri) where T : struct
        {
            using HttpResponseMessage response = await Http.DeleteAsync(uri);
            return MapJSONToType<T>(await response.Content.ReadAsStringAsync());
        }

        public async Task ConnectWebSocket(string uri, ShellSession shellSession, Action<string> websocketResponseCallback)
        {
            var fqdn = "ws://" + Plugin.Configuration.KinkShellServerAddress + "/" + uri;
            var ws = new ClientWebSocket();

            shellSession.Status = Models.ConnectionStatus.CONNECTING;
            await ws.ConnectAsync(new Uri(fqdn), CancellationToken.None);
            shellSession.Status = Models.ConnectionStatus.CONNECTED;

            await ReceiveWebSocketData(ws, shellSession, websocketResponseCallback);
        }

        private T? MapJSONToType<T>(string json) where T : struct
        {
            try
            {
                var jObj = JObject.Parse(json);

                return APIRequestMapper.MapRequestToModel<T>(jObj);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task ReceiveWebSocketData(ClientWebSocket ws, ShellSession shellSession, Action<string> websocketResponseCallback)
        {
            var buffer = new byte[1024 * 4];

            while (true)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close || shellSession.Status == Models.ConnectionStatus.CLOSED)
                {
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                websocketResponseCallback(message);
            }
        }
    }
}