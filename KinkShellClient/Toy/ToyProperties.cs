using Buttplug.Client;
using Buttplug.Core.Messages;
using System;

namespace CatboyEngineering.KinkShellClient.Toy
{
    public struct ToyProperties
    {
        public string DisplayName { get; set; }
        public Guid DeviceInstanceID { get; set; }
        public int Constrict { get; set; }
        public int Inflate { get; set; }
        public int Linear { get; set; }
        public int Oscillate { get; set; }
        public int Rotate { get; set; }
        public int Vibrate { get; set; }
        public uint Index { get; set; }

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
