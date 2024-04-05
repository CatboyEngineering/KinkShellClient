using CatboyEngineering.KinkShellClient.Models.API.Response;
using CatboyEngineering.KinkShellClient.Models.API.WebSocket;
using CatboyEngineering.KinkShellClient.Models.Shell;
using CatboyEngineering.KinkShellClient.Utilities;
using Lumina.Excel.GeneratedSheets2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace CatboyEngineering.KinkShellClient.Network
{
    public class HTTPHandler
    {
        public HttpClient Http { get; }
        public Plugin Plugin { get; }

        public HTTPHandler(Plugin plugin)
        {
            Plugin = plugin;
            Http = new HttpClient();

            Http.DefaultRequestHeaders.Add("X-Captcha-Token", Plugin.Configuration.CaptchaToken);
        }

        public async Task<APIResponse<T>> Get<T>(string uri) where T : struct
        {
            return await GetHTTP<T>(HttpMethod.Get, uri, null);
        }

        public async Task<APIResponse<T>> Post<T>(string uri, JObject body) where T : struct
        {
            return await GetHTTP<T>(HttpMethod.Post, uri, body);
        }

        public async Task<APIResponse<T>> Put<T>(string uri, JObject body) where T : struct
        {
            return await GetHTTP<T>(HttpMethod.Put, uri, body);
        }

        public async Task<APIResponse<T>> Patch<T>(string uri, JObject body) where T : struct
        {
            return await GetHTTP<T>(HttpMethod.Patch, uri, body);
        }

        public async Task<APIResponse<T>> Delete<T>(string uri) where T : struct
        {
            return await GetHTTP<T>(HttpMethod.Delete, uri, null);
        }

        public async Task<ClientWebSocket> ConnectWebSocket(string uri, ShellSession shellSession)
        {
            var fqdn = $"{(Plugin.Configuration.KinkShellSecure ? "wss" : "ws")}://{Plugin.Configuration.KinkShellServerAddress}/{uri}";
            var ws = new ClientWebSocket();

            ws.Options.SetRequestHeader("Authorization", $"Bearer {Plugin.Configuration.KinkShellAuthenticatedUserData.AuthToken}");
            shellSession.WebSocket = ws;

            await ws.ConnectAsync(new Uri(fqdn), CancellationToken.None);
            shellSession.Status = Models.ShellConnectionStatus.CONNECTING;

            return ws;
        }
        
        public async Task SendWebSocketMessage(ShellSession session, ShellSocketMessage message)
        {
            var jsonReply = JsonConvert.SerializeObject(message);
            var bytesReply = Encoding.UTF8.GetBytes(jsonReply);
            var arraySegment = new ArraySegment<byte>(bytesReply, 0, bytesReply.Length);

            await session.WebSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public void SetAuthenticationToken(string token)
        {
            Http.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");
        }

        private async Task<APIResponse<T>> GetHTTP<T>(HttpMethod method, string uri, JObject? body) where T : struct
        {
            uri = $"{(Plugin.Configuration.KinkShellSecure ? "https" : "http")}://{Plugin.Configuration.KinkShellServerAddress}/v1/{uri}";
            StringContent stringContent = null;

            if (body != null)
            {
                stringContent = new(
                body.ToString(),
                Encoding.UTF8,
                "application/json");
            }

            HttpResponseMessage response;

            switch (method.Method.ToUpper())
            {
                case "POST":
                    response = await Http.PostAsync(uri, stringContent);
                    break;
                case "PUT":
                    response = await Http.PutAsync(uri, stringContent);
                    break;
                case "PATCH":
                    response = await Http.PatchAsync(uri, stringContent);
                    break;
                case "DELETE":
                    response = await Http.DeleteAsync(uri);
                    break;
                default:
                    response = await Http.GetAsync(uri);
                    break;
            }

            try
            {
                var responseBody = JObject.Parse(await response.Content.ReadAsStringAsync());

                return new APIResponse<T>
                {
                    StatusCode = response.StatusCode,
                    Response = responseBody,
                    Result = MapJSONToType<T>(responseBody)
                };
            }
            catch (Exception)
            {
                return new APIResponse<T>
                {
                    StatusCode = response.StatusCode,
                    Response = new JObject()
                };
            }
        }

        private T? MapJSONToType<T>(JObject jObj) where T : struct
        {
            try
            {
                return APIRequestMapper.MapRequestToModel<T>(jObj);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}