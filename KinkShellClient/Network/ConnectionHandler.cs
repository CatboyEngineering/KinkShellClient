using CatboyEngineering.KinkShellClient.Models;
using CatboyEngineering.KinkShellClient.Models.API.Request;
using CatboyEngineering.KinkShellClient.Models.API.Response;
using CatboyEngineering.KinkShellClient.Models.API.Response.V2;
using CatboyEngineering.KinkShellClient.Models.API.WebSocket;
using CatboyEngineering.KinkShellClient.Models.API.WebSocket.Request;
using CatboyEngineering.KinkShellClient.Models.API.WebSocket.Response;
using CatboyEngineering.KinkShellClient.Models.Shell;
using CatboyEngineering.KinkShellClient.Models.Toy;
using CatboyEngineering.KinkShellClient.Utilities;
using CatboyEngineering.KinkShellClient.Windows;
using Dalamud.Game.ClientState.Statuses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Numerics;
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

        [Obsolete]
        public async Task<HttpStatusCode> Authenticate()
        {
            var request = new AccountLoginRequest
            {
                Username = Plugin.Configuration.KinkShellServerUsername,
                Password = Plugin.Configuration.KinkShellServerPassword,
                ClientVersionString = Plugin.Version
            };

            var response = await Plugin.HTTP.Post<Models.API.Response.AccountAuthenticatedResponse>("v1/account", JObject.FromObject(request));

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Plugin.Configuration.KinkShellAuthenticatedUserData = response.Result.Value;
                Plugin.HTTP.SetAuthenticationToken(response.Result.Value.AuthToken);
            }

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> AuthenticateV1Migrate()
        {
            var request = new Models.API.Request.V2.AccountLoginRequestMigrate
            {
                Username = Plugin.Configuration.KinkShellServerUsername,
                Password = Plugin.Configuration.KinkShellServerPassword,
                CharacterName = Plugin.ClientState.LocalPlayer.Name.TextValue,
                CharacterServer = Plugin.ClientState.LocalPlayer.HomeWorld.Value.Name.ExtractText(),
                ClientVersionString = Plugin.Version
            };

            var response = await Plugin.HTTP.Post<Models.API.Response.V2.AccountAuthenticatedResponse>("v2/account", JObject.FromObject(request));

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Plugin.Configuration.KinkShellUserData = response.Result.Value;
                Plugin.Configuration.KinkShellServerLoginToken = response.Result.Value.LoginToken;
                Plugin.Configuration.KinkShellServerUsername = "";
                Plugin.Configuration.KinkShellServerPassword = "";

                // For legacy support.
                Plugin.Configuration.KinkShellAuthenticatedUserData = new Models.API.Response.AccountAuthenticatedResponse
                {
                    AuthToken = response.Result.Value.AuthToken,
                    DisplayName = response.Result.Value.CharacterName,
                    AccountID = response.Result.Value.AccountID,
                };

                Plugin.Configuration.Save();
                Plugin.HTTP.SetAuthenticationToken(response.Result.Value.AuthToken);
            }

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> AuthenticateV2()
        {
            var request = new Models.API.Request.V2.AccountLoginRequest
            {
                CharacterName = Plugin.ClientState.LocalPlayer.Name.TextValue,
                CharacterServer = Plugin.ClientState.LocalPlayer.HomeWorld.Value.Name.ExtractText(),
                LoginToken = Plugin.Configuration.KinkShellServerLoginToken,
                ClientVersion = Plugin.Version
            };

            var response = await Plugin.HTTP.Post<Models.API.Response.V2.AccountAuthenticatedResponse>("v2/account", JObject.FromObject(request));


            if (response.StatusCode == HttpStatusCode.OK)
            {
                Plugin.Configuration.KinkShellUserData = response.Result.Value;

                // For legacy support.
                Plugin.Configuration.KinkShellAuthenticatedUserData = new Models.API.Response.AccountAuthenticatedResponse
                {
                    AuthToken = response.Result.Value.AuthToken,
                    DisplayName = response.Result.Value.CharacterName,
                    AccountID = response.Result.Value.AccountID,
                };

                Plugin.HTTP.SetAuthenticationToken(response.Result.Value.AuthToken);
            }

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> CreateAccount()
        {
            var request = new Models.API.Request.V2.AccountLoginRequest
            {
                CharacterName = Plugin.ClientState.LocalPlayer.Name.TextValue,
                CharacterServer = Plugin.ClientState.LocalPlayer.HomeWorld.Value.Name.ExtractText(),
                ClientVersion = Plugin.Version
            };

            var response = await Plugin.HTTP.Put<Models.API.Response.V2.AccountAuthenticatedResponse>("v2/account", JObject.FromObject(request));

            if (response.StatusCode == HttpStatusCode.Created)
            {
                Plugin.Configuration.KinkShellUserData = response.Result.Value;
                Plugin.Configuration.KinkShellServerLoginToken = response.Result.Value.LoginToken;

                // For legacy support.
                Plugin.Configuration.KinkShellAuthenticatedUserData = new Models.API.Response.AccountAuthenticatedResponse
                {
                    AuthToken = response.Result.Value.AuthToken,
                    DisplayName = response.Result.Value.CharacterName,
                    AccountID = response.Result.Value.AccountID,
                };

                Plugin.Configuration.Save();
                Plugin.HTTP.SetAuthenticationToken(response.Result.Value.AuthToken);
            }

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> RecoverAccount()
        {
            var request = new Models.API.Request.V2.AccountLoginRequest
            {
                CharacterName = Plugin.ClientState.LocalPlayer.Name.TextValue,
                CharacterServer = Plugin.ClientState.LocalPlayer.HomeWorld.Value.Name.ExtractText(),
                ClientVersion = Plugin.Version
            };

            var response = await Plugin.HTTP.Put<AccountRecoverStartedResponse>("v2/account/recover", JObject.FromObject(request));

            if (response.StatusCode == HttpStatusCode.Created)
            {
                Plugin.Configuration.RecoveryIntegrityToken = response.Result.Value.IntegrityToken;
                Plugin.Configuration.KinkShellUserData = new Models.API.Response.V2.AccountAuthenticatedResponse
                {
                    VerificationToken = response.Result.Value.VerificationToken,
                    CharacterID = response.Result.Value.CharacterID
                };

                Plugin.Configuration.Save();
            }

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> VerifyCharacterRecovery(string token)
        {
            var response = await Plugin.HTTP.Get<Models.API.Response.V2.AccountAuthenticatedResponse>($"v2/account/recover/{token}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Plugin.Configuration.KinkShellUserData = response.Result.Value;
                Plugin.Configuration.KinkShellServerLoginToken = response.Result.Value.LoginToken;

                // For legacy support.
                Plugin.Configuration.KinkShellAuthenticatedUserData = new Models.API.Response.AccountAuthenticatedResponse
                {
                    AuthToken = response.Result.Value.AuthToken,
                    DisplayName = response.Result.Value.CharacterName,
                    AccountID = response.Result.Value.AccountID,
                };

                Plugin.Configuration.Save();
                Plugin.HTTP.SetAuthenticationToken(response.Result.Value.AuthToken);
            }

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> VerifyCharacter()
        {
            var response = await Plugin.HTTP.Get<Models.API.Response.V2.AccountAuthenticatedResponse>("v2/account/verify");

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> DeleteAccount()
        {
            var response = await Plugin.HTTP.Delete<IEmpty>("v1/account");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Plugin.Configuration.KinkShellServerLoginToken = "";
                Plugin.Configuration.KinkShellServerUsername = "";
                Plugin.Configuration.KinkShellServerPassword = "";
            }

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> GetKinkShells()
        {
            var response = await Plugin.HTTP.Get<ShellListResponse>("v1/shell");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Plugin.Configuration.Shells = response.Result.Value.Shells;
            }

            return response.StatusCode;
        }

        public async Task<bool> GetAccounts()
        {
            var response = await Plugin.HTTP.Get<AccountListResponse>("v1/admin/users");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Plugin.Configuration.AdminUserList = response.Result.Value.Accounts;

                return true;
            }

            return false;
        }

        public async Task<HttpStatusCode> CreateShell(string name)
        {
            var request = new ShellCreateRequest
            {
                ShellName = name
            };

            var response = await Plugin.HTTP.Put<KinkShell>("v1/shell", JObject.FromObject(request));

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

            var response = await Plugin.HTTP.Patch<KinkShell>($"v1/shell/{shellID}", JObject.FromObject(request));

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Plugin.Configuration.Shells.RemoveAll(s => s.ShellID == shellID);
                Plugin.Configuration.Shells.Add(response.Result.Value);
            }

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> DeleteShell(Guid shellID)
        {
            var response = await Plugin.HTTP.Delete<KinkShell>($"v1/shell/{shellID}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var shell = Plugin.Configuration.Shells.Find(s => s.ShellID == shellID);

                Plugin.Configuration.Shells.Remove(shell);
            }

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> LogOut()
        {
            var response = await Plugin.HTTP.Post<Models.API.Response.AccountAuthenticatedResponse>("v1/logout", null);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Plugin.Configuration.KinkShellAuthenticatedUserData = new Models.API.Response.AccountAuthenticatedResponse();
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
            }
            else
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
                    case ShellSocketMessageType.STATUS:
                        HandleUserStatusMessage(baseResponse, session);
                        break;
                    case ShellSocketMessageType.TOY:
                        HandleUserToyMessage(baseResponse, session);
                        break;
                    default:
                        HandleUserTextMessage(baseResponse, session);
                        break;
                }
            }
        }

        private async Task SendShellConnectRequest(ShellSession shellSession)
        {
            var connectMessage = new ShellSocketMessage
            {
                MessageType = ShellSocketMessageType.CONNECT,
                MessageData = JObject.FromObject(new ShellSocketConnectRequest
                {
                    ShellID = shellSession.KinkShell.ShellID,
                    TextColor = Plugin.Configuration.SelfTextColor,
                    Toys = Plugin.ToyController.ConnectedToys
                })
            };

            await Plugin.HTTP.SendWebSocketMessage(shellSession, connectMessage);
        }

        public async Task SendShellToyUpdateRequest(ShellSession shellSession)
        {
            var connectMessage = new ShellSocketMessage
            {
                MessageType = ShellSocketMessageType.TOY,
                MessageData = JObject.FromObject(new ShellSocketConnectRequest
                {
                    ShellID = shellSession.KinkShell.ShellID,
                    TextColor = Plugin.Configuration.SelfTextColor,
                    Toys = Plugin.ToyController.ConnectedToys
                })
            };

            await Plugin.HTTP.SendWebSocketMessage(shellSession, connectMessage);
        }

        public async Task SendShellStatusRequest(ShellSession shellSession, string commandName, Guid commandID, ShellSocketCommandStatus status)
        {
            var connectMessage = new ShellSocketMessage
            {
                MessageType = ShellSocketMessageType.STATUS,
                MessageData = JObject.FromObject(new ShellSocketStatusRequest
                {
                    ShellID = shellSession.KinkShell.ShellID,
                    CommandName = commandName,
                    CommandInstanceID = commandID,
                    Status = status
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

        public async Task SendShellCommand(ShellSession shellSession, Guid target, Guid toyID, StoredShellCommand storedShellCommand)
        {
            var connectMessage = new ShellSocketMessage
            {
                MessageType = ShellSocketMessageType.COMMAND,
                MessageData = JObject.FromObject(new ShellSocketCommandRequest
                {
                    ShellID = shellSession.KinkShell.ShellID,
                    Target = target,
                    ToyID = toyID,
                    Command = new ShellCommand
                    {
                        CommandName = storedShellCommand.Name,
                        CommandInstanceID = Guid.NewGuid(),
                        Instructions = storedShellCommand.Instructions
                    }
                })
            };

            await Plugin.HTTP.SendWebSocketMessage(shellSession, connectMessage);
        }

        private void HandleUserConnectedMessage(ShellSocketMessage message, ShellSession session)
        {
            var request = APIRequestMapper.MapRequestToModel<ShellSocketConnectedUsersResponse>(message.MessageData);

            if (request != null)
            {
                var userChangeDetected = false;

                // Incoming
                foreach (var newUser in request.Value.ConnectedUsers)
                {
                    if (!session.ConnectedUsers.Exists(cu => cu.AccountID == newUser.AccountID))
                    {
                        // User connected.

                        session.ReceivedNewMessage(new ChatMessage
                        {
                            DisplayName = "[SYSTEM]",
                            DateTime = DateTime.Now,
                            Message = $"{newUser.DisplayName} connected",
                            TextColor = new Vector4(1f, 1f, 1f, 1f)
                        });

                        userChangeDetected = true;

                        break;
                    }
                }

                if (!userChangeDetected)
                {
                    foreach (var oldUser in session.ConnectedUsers)
                    {
                        if (!request.Value.ConnectedUsers.Exists(cu => cu.AccountID == oldUser.AccountID))
                        {
                            // User disconnected.

                            session.ReceivedNewMessage(new ChatMessage
                            {
                                DisplayName = "[SYSTEM]",
                                DateTime = DateTime.Now,
                                Message = $"{oldUser.DisplayName} disconnected",
                                TextColor = new Vector4(1f, 1f, 1f, 1f)
                            });

                            userChangeDetected = true;

                            break;
                        }
                    }
                }

                session.ConnectedUsers.Clear();
                session.ConnectedUsers.AddRange(request.Value.ConnectedUsers);
            }
        }

        private void HandleUserStatusMessage(ShellSocketMessage message, ShellSession session)
        {
            var request = APIRequestMapper.MapRequestToModel<ShellSocketStatusResponse>(message.MessageData);

            if (request != null)
            {
                var user = session.ConnectedUsers.Find(cu => cu.AccountID == request.Value.UserID);

                user.RunningCommands.Clear();
                user.RunningCommands.AddRange(request.Value.RunningCommands);
            }
        }

        private void HandleUserToyMessage(ShellSocketMessage message, ShellSession session)
        {
            var request = APIRequestMapper.MapRequestToModel<ShellSocketUserToyChangeResponse>(message.MessageData);

            if (request != null)
            {
                var user = session.ConnectedUsers.Find(cu => cu.AccountID == request.Value.UserID);

                user.Toys.Clear();
                user.Toys.AddRange(request.Value.Toys);
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
                    if (Plugin.ToyController.ConnectedToys.Exists(ct => ct.DeviceInstanceID == request.Value.ToyID))
                    {
                        var toy = Plugin.ToyController.ConnectedToys.Find(ct => ct.DeviceInstanceID == request.Value.ToyID);

                        if (!Plugin.ToyController.IsCommandRunning(toy))
                        {
                            var command = request.Value.Command;

                            await SendShellStatusRequest(session, command.CommandName, command.CommandInstanceID, ShellSocketCommandStatus.RUNNING);
                            await Plugin.ToyController.IssueCommand(session, toy, command);
                        }
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
            foreach (var connection in Connections)
            {
                if (connection.WebSocket != null)
                {
                    connection.WebSocket.Abort();
                    connection.WebSocket.Dispose();
                }
            }
        }
    }
}
