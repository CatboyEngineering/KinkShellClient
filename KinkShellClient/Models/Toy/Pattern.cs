using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Models.Toy
{
    public struct Pattern
    {
        public PatternType PatternType { get; set; }
        public double[]? VibrateIntensity { get; set; }
        public double[]? OscillateIntensity { get; set; }
        public double? LinearPosition { get; set; }
        public double? RotateSpeed { get; set; }
        public bool? RotateClockwise { get; set; }
        public double? InflateAmount { get; set; }
        public double? ConstrictAmount { get; set; }
        public int Duration { get; set; }

        public bool IsValid()
        {
            if(Duration <= 0)
            {
                return false;
            }

            switch (PatternType)
            {
                case PatternType.CONSTRICT:
                    return ConstrictAmount.HasValue;
                case PatternType.INFLATE:
                    return InflateAmount.HasValue;
                case PatternType.LINEAR:
                    return LinearPosition.HasValue;
                case PatternType.OSCILLATE:
                    return OscillateIntensity != null && OscillateIntensity.Length > 0;
                case PatternType.ROTATE:
                    return RotateClockwise.HasValue && RotateSpeed.HasValue;
                case PatternType.VIBRATE:
                    return VibrateIntensity != null && VibrateIntensity.Length > 0;
                default:
                    return true;
            }
        }
    }
}
