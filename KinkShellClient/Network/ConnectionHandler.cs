using CatboyEngineering.KinkShellClient.Models;
using CatboyEngineering.KinkShellClient.Models.API.Request;
using CatboyEngineering.KinkShellClient.Models.API.Response;
using CatboyEngineering.KinkShellClient.Models.API.WebSocket;
using CatboyEngineering.KinkShellClient.Models.API.WebSocket.Request;
using CatboyEngineering.KinkShellClient.Models.API.WebSocket.Response;
using CatboyEngineering.KinkShellClient.Models.Shell;
using CatboyEngineering.KinkShellClient.Models.Toy;
using CatboyEngineering.KinkShellClient.Utilities;
using CatboyEngineering.KinkShellClient.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Network
{
    public class ConnectionHandler : IDisposable
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
                Username = Plugin.Configuration.KinkShellServerUsername,
                Password = Plugin.Configuration.KinkShellServerPassword,
                ClientVersionString = Plugin.Version
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

        public async Task<HttpStatusCode> UpdateShell(Guid shellID, List<ShellNewUser> users)
        {
            var request = new ShellAdjustUsersRequest
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

        public async Task<HttpStatusCode> DeleteShell(Guid shellID)
        {
            var response = await Plugin.HTTP.Delete<KinkShell>($"shell/{shellID}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var shell = Plugin.Configuration.Shells.Find(s => s.ShellID == shellID);

                Plugin.Configuration.Shells.Remove(shell);
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

        public async Task OpenConnection(ShellWindow window)
        {
            try
            {
                var socket = await Plugin.HTTP.ConnectWebSocket("ws", window.State.Session);

                await ListenWebSocket(socket, window);
            }
            catch
            {
                window.OnClose();
            }
        }

        public async Task CloseConnection(KinkShell kinkShell)
        {
            var session = Connections.Find(c => c.KinkShell.ShellID == kinkShell.ShellID);

            if (session != null && session.WebSocket != null)
            {
                try
                {
                    await session.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnected safely.", CancellationToken.None);
                }
                catch { }
            }

            Connections.Remove(session);
        }

        private async Task ListenWebSocket(ClientWebSocket ws, ShellWindow window)
        {
            var session = window.State.Session;

            if (session.Status == ShellConnectionStatus.CONNECTING)
            {
                await SendShellConnectRequest(session);
                session.Status = ShellConnectionStatus.CONNECTED;
            }

            var buffer = new byte[1024 * 4];

            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close || session.Status == ShellConnectionStatus.CLOSED)
                {
                    window.OnClose();
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                
                // We won't await the code to handle this request; continue the loop
                _ = HandleWebSocketResponse(message, session);
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

        public async Task SendShellChatMessage(ShellSession shellSession, string message)
        {
            var connectMessage = new ShellSocketMessage
            {
                MessageType = ShellSocketMessageType.TEXT,
                MessageData = JObject.FromObject(new ShellSocketTextMessageRequest
                {
                    ShellID = shellSession.KinkShell.ShellID,
                    DateTime = DateTime.UtcNow,
                    MessageText = message,
                    TextColor = Plugin.Configuration.SelfTextColor
                })
            };

            await Plugin.HTTP.SendWebSocketMessage(shellSession, connectMessage);
        }

        public async Task SendShellCommand(ShellSession shellSession, List<Guid> targets, StoredShellCommand storedShellCommand)
        {
            var connectMessage = new ShellSocketMessage
            {
                MessageType = ShellSocketMessageType.COMMAND,
                MessageData = JObject.FromObject(new ShellSocketCommandRequest
                {
                    ShellID = shellSession.KinkShell.ShellID,
                    Targets = targets,
                    Command = new ShellCommand
                    {
                        Instructions = storedShellCommand.Instructions
                    }
                })
            };

            await Plugin.HTTP.SendWebSocketMessage(shellSession, connectMessage);
        }

        private async Task HandleWebSocketResponse(string message, ShellSession session)
        {
            var messageBody = JObject.Parse(message);
            var response = APIRequestMapper.MapRequestToModel<ShellSocketMessage>(messageBody);

            if (response != null)
            {
                var baseResponse = response.Value;

                Plugin.Logger.Info($"Received Shell message [{baseResponse.MessageType}]");
                switch (baseResponse.MessageType)
                {
                    case ShellSocketMessageType.COMMAND:
                        await HandleUserCommandMessage(baseResponse, session);
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
            var request = APIRequestMapper.MapRequestToModel<ShellSocketConnectedUsersResponse>(message.MessageData);

            if (request != null)
            {
                session.ConnectedUsers.Clear();
                session.ConnectedUsers.AddRange(request.Value.ConnectedUsers);
            }
        }

        private void HandleUserTextMessage(ShellSocketMessage message, ShellSession session)
        {
            var request = APIRequestMapper.MapRequestToModel<ShellSocketTextMessageResponse>(message.MessageData);

            if (request != null)
            {
                session.ReceivedNewMessage(new ChatMessage
                {
                    DisplayName = request.Value.UserFrom.DisplayName,
                    DateTime = request.Value.DateTime,
                    Message = request.Value.MessageText,
                    TextColor = request.Value.TextColor
                });
            }
        }

        private async Task HandleUserCommandMessage(ShellSocketMessage message, ShellSession session)
        {
            if (session.SelfUserReceiveCommands)
            {
                var request = APIRequestMapper.MapRequestToModel<ShellSocketCommandResponse>(message.MessageData);

                if (request != null)
                {
                    if (Plugin.ToyController.Client.Devices.Length > 0)
                    {
                        await Plugin.ToyController.IssueCommand(Plugin.ToyController.Client.Devices[0], request.Value.Command);
                    }
                }
            }
            else
            {
                Plugin.Logger.Info("Rejected toy command due to user preferences.");
            }
        }

        public void Dispose()
        {
            foreach(var connection in Connections)
            {
                if(connection.WebSocket != null)
                {
                    connection.WebSocket.Abort();
                    connection.WebSocket.Dispose();
                }
            }
        }
    }
}
