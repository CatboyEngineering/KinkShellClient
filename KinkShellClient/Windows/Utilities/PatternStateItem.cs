using CatboyEngineering.KinkShellClient.Models.Toy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShellClient.Windows.Utilities
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
        public int durationIntBuffer = 0;
        public int delayIntBuffer = 0;

        public PatternStateItem(Pattern pattern)
        {
            TrackingID = Guid.NewGuid();
            Pattern = pattern;

            NewPatternType = pattern.PatternType;
            NewIntensity = pattern.Intensity;
            NewDuration = pattern.Duration; 
            NewDelay = pattern.Delay;

            patternIntBuffer = ((int)pattern.PatternType);
            intensityDoubleBuffer = pattern.Intensity;
            durationIntBuffer = pattern.Duration;
            delayIntBuffer = pattern.Delay;
        }
    }
}
