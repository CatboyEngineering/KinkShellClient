using Buttplug.Client;
using Buttplug.Client.Connectors.WebsocketConnector;
using Buttplug.Core.Messages;
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

        public ToyController(Plugin plugin)
        {
            Plugin = plugin;
            StopRequested = false;
            ConnectedToys = new List<ToyProperties>();
        }

        public async Task Connect()
        {
            await Task.Run(() =>
            {
                Connector = new ButtplugWebsocketConnector(new Uri($"{Plugin.Configuration.IntifaceServerAddress}"));
                Client = new ButtplugClient("KinkShell Client");
            });

            try
            {
                await Client.ConnectAsync(Connector);
                await Scan();
            }
            catch { }
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
                    ConnectedToys.Add(new ToyProperties(toy));
                }
            }
        }

        public async Task Disconnect()
        {
            if (Client.Connected)
            {
                await Client.DisconnectAsync();
            }
        }

        public void StopAllDevices()
        {
            StopRequested = true;

            if (Client.Connected)
            {
                foreach (var device in Client.Devices)
                {
                    _ = device.Stop();
                }
            }
        }

        public async Task IssueCommand(ToyProperties toy, ShellCommand command)
        {
            if (Client.Connected)
            {
                Plugin.Logger.Info("Translating Shell command to Intiface.");
                var device = Client.Devices[toy.Index];

                foreach (var pattern in command.Instructions)
                {
                    if (StopRequested)
                    {
                        Plugin.Logger.Info("User requested to stop current command.");
                        break;
                    }

                    if (pattern.IsValid())
                    {
                        try
                        {
                            switch (pattern.PatternType)
                            {
                                case PatternType.CONSTRICT:
                                    await device.ScalarAsync(new ScalarSubcommand(toy.Index, pattern.ConstrictAmount.Value, ActuatorType.Constrict));
                                    await Task.Delay(pattern.Duration);
                                    break;
                                case PatternType.INFLATE:
                                    await device.ScalarAsync(new ScalarSubcommand(toy.Index, pattern.InflateAmount.Value, ActuatorType.Inflate));
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

                StopRequested = false;
            }
        }

        public void Dispose()
        {
            if (Client != null)
            {
                if (Client.Connected)
                {
                    Client.DisconnectAsync();
                }
            }
        }
    }
}