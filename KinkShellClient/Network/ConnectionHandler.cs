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
using static System.Collections.Specialized.BitVector32;

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
            var socket = await Plugin.HTTP.ConnectWebSocket("ws", shellSession);

            await ListenWebSocket(socket, shellSession);
        }

        public async Task CloseConnection(KinkShell kinkShell)
        {
            var session = Connections.Find(c => c.KinkShell.ShellID == kinkShell.ShellID);

            if (session != null && session.WebSocket != null)
            {
                await session.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnected safely.", CancellationToken.None);
            }
        }

        private async Task ListenWebSocket(ClientWebSocket ws, ShellSession session)
        {
            if (session.Status == ShellConnectionStatus.CONNECTING)
            {
                await SendShellConnectRequest(session);
                session.Status = ShellConnectionStatus.CONNECTED;
            }

            var buffer = new byte[1024 * 4];

            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close || session.Status == Models.ShellConnectionStatus.CLOSED)
                {
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                
                HandleWebSocketResponse(message, session);
            }
        }

        private async Task SendShellConnectRequest(ShellSession shellSession)
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
                        // We won't be receiving these messages
                        break;
                    case ShellSocketMessageType.INFO:
                        HandleUserConnectedMessage(baseResponse, session);
                        break;
                    default:
                        HandleUserTextMessage(baseResponse, session);
                        break;
                }
            }
        }

        private void HandleUserConnectedMessage(ShellSocketMessage message, ShellSession session)
        {
            var request = APIRequestMapper.MapRequestToModel<ShellSocketConnectResponse>(message.MessageData);

            if (request != null)
            {
                session.ConnectedUsers.Clear();
                session.ConnectedUsers.AddRange(request.Value.ConnectedUsers);
            }
        }

        private void HandleUserTextMessage(ShellSocketMessage message, ShellSession session)
        {
            var request = APIRequestMapper.MapRequestToModel<ShellSocketTextResponse>(message.MessageData);

            if (request != null)
            {
                session.Messages.Add(new ChatMessage
                {
                    DisplayName = request.Value.UserFrom.DisplayName,
                    Message = request.Value.MessageText
                });
            }
        }
    }
}
