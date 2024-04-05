using CatboyEngineering.KinkShellClient.Models.Toy;
using System;

namespace CatboyEngineering.KinkShellClient.Windows.States.Models
{
    public class PatternStateItem
    {
        public Guid TrackingID { get; set; }
        public Pattern Pattern { get; set; }
        public PatternType NewPatternType { get; set; }
        public double NewIntensity { get; set; }
        public int NewDuration { get; set; }
        public int NewDelay { get; set; }

        public int patternIntBuffer = 0;
        public double intensityDoubleBuffer = 0;
        public double durationDoubleBuffer = 0;
        public double delayDoubleBuffer = 0;

        public PatternStateItem(Pattern pattern)
        {
            TrackingID = Guid.NewGuid();
            Pattern = pattern;

            NewPatternType = pattern.PatternType;
            NewIntensity = pattern.Intensity;
            NewDuration = pattern.Duration;
            NewDelay = pattern.Delay;

            patternIntBuffer = (int)pattern.PatternType;
            intensityDoubleBuffer = pattern.Intensity;
            durationDoubleBuffer = pattern.Duration / 1000d;
            delayDoubleBuffer = pattern.Delay / 1000d;
        }
    }
}
