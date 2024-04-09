using CatboyEngineering.KinkShellClient.Models.Toy;
using System;

namespace CatboyEngineering.KinkShellClient.Windows.States.Models
{
    public class PatternStateItem
    {
        public Guid TrackingID { get; set; }
        public Pattern Pattern { get; set; }

        public PatternType NewPatternType { get; set; }
        public double[]? NewVibrateIntensity { get; set; }
        public double[]? NewOscillateIntensity { get; set; }
        public double? NewLinearPosition { get; set; }
        public double? NewRotateSpeed { get; set; }
        public bool? NewRotateClockwise { get; set; }
        public double? NewInflateAmount { get; set; }
        public double? NewConstrictAmount { get; set; }
        public int NewDuration { get; set; }

        public int patternIntBuffer = 0;
        public double vibrateIntensityBuffer1 = 0;
        public double vibrateIntensityBuffer2 = 0;
        public double oscillateIntensityBuffer1 = 0;
        public double oscillateIntensityBuffer2 = 0;
        public double linearPositionBuffer = 0;
        public double rotateSpeedBuffer = 0;
        public bool rotateClockwiseBuffer = true;
        public double inflateAmountBuffer = 0;
        public double constrictAmountBuffer = 0;
        public double durationDoubleBuffer = 0;

        public PatternStateItem(Pattern pattern)
        {
            TrackingID = Guid.NewGuid();
            Pattern = pattern;

            NewPatternType = pattern.PatternType;
            NewVibrateIntensity = pattern.VibrateIntensity;
            NewOscillateIntensity = pattern.OscillateIntensity;
            NewLinearPosition = pattern.LinearPosition;
            NewRotateClockwise = pattern.RotateClockwise;
            NewRotateSpeed = pattern.RotateSpeed;
            NewInflateAmount = pattern.InflateAmount;
            NewConstrictAmount = pattern.ConstrictAmount;
            NewDuration = pattern.Duration;

            patternIntBuffer = (int)pattern.PatternType;

            vibrateIntensityBuffer1 = pattern.VibrateIntensity != null ? pattern.VibrateIntensity[0] : 0;
            vibrateIntensityBuffer2 = pattern.VibrateIntensity != null ? pattern.VibrateIntensity[1] : 0;
            oscillateIntensityBuffer1 = pattern.OscillateIntensity != null ? pattern.OscillateIntensity[0] : 0;
            oscillateIntensityBuffer2 = pattern.OscillateIntensity != null ? pattern.OscillateIntensity[1] : 0;

            linearPositionBuffer = pattern.LinearPosition.GetValueOrDefault(0);
            rotateSpeedBuffer = pattern.RotateSpeed.GetValueOrDefault(0);
            rotateClockwiseBuffer = pattern.RotateClockwise.GetValueOrDefault(true);
            inflateAmountBuffer = pattern.InflateAmount.GetValueOrDefault(0);
            constrictAmountBuffer = pattern.ConstrictAmount.GetValueOrDefault(0);
            durationDoubleBuffer = pattern.Duration / 1000d;
        }
    }
}
