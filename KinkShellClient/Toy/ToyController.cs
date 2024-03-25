using Buttplug.Client;
using Buttplug.Client.Connectors.WebsocketConnector;
using CatboyEngineering.KinkShell.Models.Toy;
using KinkShellClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShell.Toy
{
    public class ToyController
    {
        public Plugin Plugin { get; init; }
        public ButtplugWebsocketConnector Connector { get; private set; }
        public ButtplugClient Client { get; private set; }

        public ToyController(Plugin plugin)
        {
            Plugin = plugin;
        }

        public async Task Connect()
        {
            Connector = new ButtplugWebsocketConnector(new Uri($"ws://{Plugin.Configuration.IntifaceServerAddress}"));
            Client = new ButtplugClient("KinkShell Client");
            
            await Client.ConnectAsync(Connector);
            await Scan();
        }

        public async Task Scan()
        {
            await Client.StartScanningAsync();
            await Task.Delay(5000);
            await Client.StopScanningAsync();
        }

        public async Task Disconnect()
        {
            await Client.DisconnectAsync();
        }

        public async Task IssueCommand(ButtplugClientDevice device, ShellCommand command)
        {
            Plugin.Logger.Debug("Translating Shell command to Intiface");

            foreach (var pattern in command.Instructions)
            {
                try
                {
                    switch(pattern.PatternType)
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
                    Plugin.Logger.Warning(ex, "KinkShell Device Compatibility");
                    // Possible that the device does not support the command, or
                    // something happened to the connection.
                }
            }
        }
    }
}
