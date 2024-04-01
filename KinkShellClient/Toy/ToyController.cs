using Buttplug.Client;
using Buttplug.Client.Connectors.WebsocketConnector;
using CatboyEngineering.KinkShellClient.Models.Toy;
using System;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Toy
{
    public class ToyController : IDisposable
    {
        public Plugin Plugin { get; init; }
        public ButtplugWebsocketConnector Connector { get; private set; }
        public ButtplugClient Client { get; private set; }
        public bool StopRequested { get; set; }

        public ToyController(Plugin plugin)
        {
            Plugin = plugin;
            StopRequested = false;
        }

        public async Task Connect()
        {
            Connector = new ButtplugWebsocketConnector(new Uri($"{Plugin.Configuration.IntifaceServerAddress}"));
            Client = new ButtplugClient("KinkShell Client");

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

        public async Task IssueCommand(ButtplugClientDevice device, ShellCommand command)
        {
            if (Client.Connected)
            {
                Plugin.Logger.Info("Translating Shell command to Intiface.");

                foreach (var pattern in command.Instructions)
                {
                    if (StopRequested)
                    {
                        Plugin.Logger.Info("User requested to stop current command.");
                        break;
                    }

                    try
                    {
                        switch (pattern.PatternType)
                        {
                            case PatternType.LINEAR:
                                await device.LinearAsync((uint)pattern.Duration, pattern.Intensity);
                                break;
                            case PatternType.ROTATE:
                                await device.RotateAsync(pattern.Intensity, true);
                                await Task.Delay(pattern.Duration);
                                await device.RotateAsync(0, true);
                                break;
                            case PatternType.OSCILLATE:
                                await device.OscillateAsync(pattern.Intensity);
                                await Task.Delay(pattern.Duration);
                                await device.OscillateAsync(0);
                                break;
                            default:
                                await device.VibrateAsync(pattern.Intensity);
                                await Task.Delay(pattern.Duration);
                                await device.VibrateAsync(0);

                                break;
                        }

                        await Task.Delay(pattern.Delay);
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.Warning(ex, "KinkShell device incompatible");
                        // Possible that the device does not support the command, or
                        // something happened to the connection.
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