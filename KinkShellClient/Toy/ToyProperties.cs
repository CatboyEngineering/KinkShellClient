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
        public uint Index { get; }

        public ToyProperties(ButtplugClientDevice device) {
            DisplayName = device.Name;
            DeviceInstanceID = Guid.NewGuid();
            Index = device.Index;

            Constrict = device.GenericAcutatorAttributes(ActuatorType.Constrict).Count;
            Inflate = device.GenericAcutatorAttributes(ActuatorType.Inflate).Count;
            Linear = device.GenericAcutatorAttributes(ActuatorType.Position).Count;
            Oscillate = device.GenericAcutatorAttributes(ActuatorType.Oscillate).Count;
            Rotate = device.GenericAcutatorAttributes(ActuatorType.Rotate).Count;
            Vibrate = device.GenericAcutatorAttributes(ActuatorType.Vibrate).Count;
        }
    }
}
