using Buttplug.Client;
using Buttplug.Client.Connectors.WebsocketConnector;
using Buttplug.Core.Messages;
using CatboyEngineering.KinkShellClient.Models;
using CatboyEngineering.KinkShellClient.Models.API.WebSocket;
using CatboyEngineering.KinkShellClient.Models.Shell;
using CatboyEngineering.KinkShellClient.Models.Toy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Buttplug.Core.Messages.ScalarCmd;

namespace CatboyEngineering.KinkShellClient.Toy
{
    public class ToyController : IDisposable
    {
        public Plugin Plugin { get; init; }
        public ButtplugWebsocketConnector Connector { get; private set; }
        public ButtplugClient Client { get; private set; }
        public bool StopRequested { get; set; }
        public List<ToyProperties> ConnectedToys { get; set; }
        public Dictionary<ToyProperties, RunningCommand> RunningCommands { get; set; }

        public ToyController(Plugin plugin)
        {
            Plugin = plugin;
            StopRequested = false;
            RunningCommands = new Dictionary<ToyProperties, RunningCommand>();
            ConnectedToys = new List<ToyProperties>();
        }

        public async Task Connect()
        {
            await Task.Run(() =>
            {
                Connector = new ButtplugWebsocketConnector(new Uri($"{Plugin.Configuration.IntifaceServerAddress}"));
                Client = new ButtplugClient("KinkShell Client");
            });

            Client.DeviceAdded += DeviceAdded;
            Client.DeviceRemoved += DeviceRemoved;

            try
            {
                await Client.ConnectAsync(Connector);
                await Scan();
            }
            catch { }
        }

        private void DeviceAdded(object? sender, DeviceAddedEventArgs args)
        {
            AddConnectedToy(args.Device);
        }

        private void DeviceRemoved(object? sender, DeviceRemovedEventArgs args)
        {
            RemoveConnectedToy(args.Device);
            _ = UpdateToysInShells();
        }

        public async Task Scan()
        {
            if (Client.Connected)
            {
                await Client.StartScanningAsync();
                await Task.Delay(3000);
                await Client.StopScanningAsync();

                ConnectedToys.Clear();

                foreach (var toy in Client.Devices)
                {
                    AddConnectedToy(toy);
                }

                await UpdateToysInShells();
            }
        }

        private void AddConnectedToy(ButtplugClientDevice device)
        {
            ConnectedToys.Add(new ToyProperties(device));
        }

        private void RemoveConnectedToy(ButtplugClientDevice device)
        {
            // Is this potentially dangerous? Will the library shift the indexes?
            ConnectedToys.RemoveAll(ct => ct.Index == device.Index);
        }

        private async Task UpdateToysInShells()
        {
            foreach(var session in Plugin.ConnectionHandler.Connections)
            {
                await Plugin.ConnectionHandler.SendShellToyUpdateRequest(session);
            }
        }

        public async Task Disconnect()
        {
            if (Client.Connected)
            {
                await Client.DisconnectAsync();
            }
        }

        public void StopAllDevices(ShellSession session, KinkShellMember selfUser)
        {
            if (RunningCommands.Count > 0)
            {
                StopRequested = true;
            }

            if (Client.Connected)
            {
                foreach (var device in Client.Devices)
                {
                    _ = device.Stop();
                }
            }

            foreach (var command in selfUser.RunningCommands)
            {
                _ = SendCommandStoppedStatus(session, command.CommandName, command.CommandInstanceID);
            }
        }

        public async Task IssueCommand(ShellSession session, ToyProperties toy, ShellCommand command)
        {
            if (Client.Connected)
            {
                Plugin.Logger.Info("Translating Shell command to Intiface.");

                RunningCommands.Add(toy, new RunningCommand
                {
                    CommandName = command.CommandName,
                    CommandInstanceID = command.CommandInstanceID
                });

                var device = Client.Devices[toy.Index];

                foreach (var pattern in command.Instructions)
                {
                    if(StopRequested)
                    {
                        StopRequested = false;
                        break;
                    }

                    if (pattern.IsValid())
                    {
                        try
                        {
                            switch (pattern.PatternType)
                            {
                                case PatternType.CONSTRICT:
                                    await device.ScalarAsync(new ScalarSubcommand(0, pattern.ConstrictAmount.Value, ActuatorType.Constrict));
                                    await Task.Delay(pattern.Duration);
                                    break;
                                case PatternType.INFLATE:
                                    await device.ScalarAsync(new ScalarSubcommand(0, pattern.InflateAmount.Value, ActuatorType.Inflate));
                                    await Task.Delay(pattern.Duration);
                                    break;
                                case PatternType.LINEAR:
                                    await device.LinearAsync((uint)pattern.Duration, pattern.LinearPosition.Value);
                                    break;
                                case PatternType.OSCILLATE:
                                    await device.OscillateAsync(pattern.OscillateIntensity);
                                    await Task.Delay(pattern.Duration);
                                    break;
                                case PatternType.ROTATE:
                                    await device.RotateAsync(pattern.RotateSpeed.Value, pattern.RotateClockwise.Value);
                                    await Task.Delay(pattern.Duration);
                                    break;
                                case PatternType.VIBRATE:
                                    await device.VibrateAsync(pattern.VibrateIntensity);
                                    await Task.Delay(pattern.Duration);
                                    break;
                                default:
                                    await device.Stop();
                                    await Task.Delay(pattern.Duration);

                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Plugin.Logger.Warning(ex, "KinkShell command error");
                        }
                    }
                    else
                    {
                        Plugin.Logger.Warning("Received an invalid command!");
                    }
                }

                RunningCommands.Remove(toy);
                await device.Stop();
                await SendCommandStoppedStatus(session, command.CommandName, command.CommandInstanceID);
            }
        }

        public async Task SendCommandStoppedStatus(ShellSession session, string commandName, Guid commandID)
        {
            await Plugin.ConnectionHandler.SendShellStatusRequest(session, commandName, commandID, ShellSocketCommandStatus.STOPPED);
        }

        public bool IsCommandRunning(ToyProperties toy)
        {
            return RunningCommands.ContainsKey(toy);
        }

        public void Dispose()
        {
            if (Client != null)
            {
                foreach(var device in Client.Devices)
                {
                    _ = device.Stop();
                }

                if (Client.Connected)
                {
                    Client.DisconnectAsync();
                }

                Client.Dispose();
            }

            Connector.Dispose();
        }
    }
}