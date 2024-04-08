using Buttplug.Client;
using Buttplug.Core.Messages;
using System;

namespace CatboyEngineering.KinkShellClient.Toy
{
    public struct ToyProperties
    {
        public string DisplayName { get; }
        public Guid DeviceInstanceID { get; }
        public int Constrict { get; }
        public int Inflate { get; }
        public int Linear { get; }
        public int Oscillate { get; }
        public int Rotate { get; }
        public int Vibrate { get; }

        public ToyProperties(ButtplugClientDevice device) {
            DisplayName = device.DisplayName;
            DeviceInstanceID = Guid.NewGuid();

            Constrict = device.GenericAcutatorAttributes(ActuatorType.Constrict).Count;
            Inflate = device.GenericAcutatorAttributes(ActuatorType.Inflate).Count;
            Linear = device.GenericAcutatorAttributes(ActuatorType.Position).Count;
            Oscillate = device.GenericAcutatorAttributes(ActuatorType.Oscillate).Count;
            Rotate = device.GenericAcutatorAttributes(ActuatorType.Rotate).Count;
            Vibrate = device.GenericAcutatorAttributes(ActuatorType.Vibrate).Count;
        }
    }
}
