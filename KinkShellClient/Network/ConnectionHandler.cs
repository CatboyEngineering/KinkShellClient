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
using System.Net.WebSockets;
using System.Text;
using System.Threading;
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
            var request = new AccountLoginRequest
            {
                Email = Plugin.Configuration.KinkShellServerUsername,
                Password = Plugin.Configuration.KinkShellServerPassword
            };

            var response = await Plugin.HTTP.Post<AccountAuthenticatedResponse>("account", JObject.FromObject(request));

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

        public async Task<HttpStatusCode> CreateShell(string name)
        {
            var request = new ShellCreateRequest
            {
                ShellName = name
            };

            var response = await Plugin.HTTP.Put<KinkShell>("shell", JObject.FromObject(request));

            if (response.StatusCode == HttpStatusCode.Created)
            {
                Plugin.Configuration.Shells.Add(response.Result.Value);
            }

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> UpdateShell(Guid shellID, List<Guid> users)
        {
            var request = new ShellUpdateUsersRequest
            {
                Users = users
            };

            var response = await Plugin.HTTP.Patch<KinkShell>($"shell/{shellID}", JObject.FromObject(request));

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var shell = Plugin.Configuration.Shells.Find(s => s.ShellID == shellID);

                shell.Users = response.Result.Value.Users;
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

        public ShellSession CreateShellSession(KinkShell kinkShell)
        {
            var session = Connections.Find(c => c.KinkShell.ShellID == kinkShell.ShellID);
            if (session == null)
            {
                var newSession = new ShellSession(kinkShell);
                Connections.Add(newSession);

                return newSession;
            } else
            {
                return session;
            }
        }

        public async Task OpenConnection(ShellSession shellSession)
        {
            await Plugin.HTTP.ConnectWebSocket("ws", shellSession, async (message) => await HandleWebSocketResponse(message, shellSession));
        }

        public async Task CloseConnection(KinkShell kinkShell)
        {
            var session = Connections.Find(c => c.KinkShell.ShellID == kinkShell.ShellID);

            if (session != null && session.WebSocket != null)
            {
                await session.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnected safely.", CancellationToken.None);
            }
        }

        public async Task SendShellConnectRequest(ShellSession shellSession)
        {
            var connectMessage = new ShellSocketMessage
            {
                MessageType = ShellSocketMessageType.CONNECT,
                MessageData = JObject.FromObject(new ShellSocketConnectRequest
                {
                    ShellID = shellSession.KinkShell.ShellID
                })
            };

            await Plugin.HTTP.SendWebSocketMessage(shellSession, connectMessage);
        }

        // TODO need to work on this
        private async Task HandleWebSocketResponse(string message, ShellSession session)
        {
            if(session.Status == ShellConnectionStatus.CONNECTING)
            {
                await SendShellConnectRequest(session);
                session.Status = ShellConnectionStatus.CONNECTED;
            }

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
