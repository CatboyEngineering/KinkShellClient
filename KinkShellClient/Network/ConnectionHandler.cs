using FFXIVClientStructs.Havok;
using KinkShellClient.Models;
using KinkShellClient.Models.API.Request;
using KinkShellClient.Models.API.Response;
using KinkShellClient.Models.API.WebSocket;
using KinkShellClient.ShellData;
using KinkShellClient.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KinkShellClient.Network
{
    public class ConnectionHandler
    {
        // Organizes each connection this client has to the KinkShell server
        public List<ShellSession> Connections { get; }
        public Plugin Plugin { get; }
        public ServerConnectionStatus ServerConnectionStatus { get; set; }

        public ConnectionHandler(Plugin plugin)
        {
            Plugin = plugin;
            Connections = new List<ShellSession>();
        }

        public async Task<HttpStatusCode> Authenticate()
        {
            var loginRequest = new AccountLoginRequest
            {
                Email = Plugin.Configuration.KinkShellServerUsername,
                Password = Plugin.Configuration.KinkShellServerPassword
            };

            var response = await Plugin.HTTP.Post<AccountAuthenticatedResponse>("account", JObject.FromObject(loginRequest));

            if(response.StatusCode == HttpStatusCode.OK)
            {
                Plugin.Configuration.KinkShellAuthenticatedUserData = response.Result.Value;
                Plugin.HTTP.SetAuthenticationToken(response.Result.Value.AuthToken);
            }

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> GetKinkShells()
        {
            var response = await Plugin.HTTP.Get<ShellListResponse>("shell");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Plugin.Configuration.Shells = response.Result.Value.Shells;
            }

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> LogOut()
        {
            var response = await Plugin.HTTP.Post<AccountAuthenticatedResponse>("logout", null);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Plugin.Configuration.KinkShellAuthenticatedUserData = new AccountAuthenticatedResponse();
            }

            return response.StatusCode;
        }

        public async Task OpenConnection(KinkShell kinkShell)
        {
            var newSession = new ShellSession(kinkShell);
            Connections.Add(newSession);

            await Plugin.HTTP.ConnectWebSocket("ws", newSession, (message) => HandleWebSocketResponse(message, newSession));
        }

        // TODO need to work on this
        private void HandleWebSocketResponse(string message, ShellSession session)
        {
            var messageBody = JObject.Parse(message);
            var response = APIRequestMapper.MapRequestToModel<ShellSocketMessage>(messageBody);

            if (response != null)
            {
                var baseResponse = response.Value;

                switch (baseResponse.MessageType)
                {
                    case ShellSocketMessageType.COMMAND:
                        break;
                    case ShellSocketMessageType.CONNECT:
                        break;
                    case ShellSocketMessageType.DISCONNECT:
                        break;
                    case ShellSocketMessageType.INFO:
                        // Not implemented currently
                        break;
                    default:
                        // Text
                        break;
                }
            }
        }
    }
}
