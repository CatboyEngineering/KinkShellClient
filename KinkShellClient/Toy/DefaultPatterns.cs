using CatboyEngineering.KinkShellClient.Models.Toy;
using System.Collections.Generic;

namespace CatboyEngineering.KinkShellClient.Toy
{
    public static class DefaultPatterns
    {
        public static readonly StoredShellCommand Ripple = new()
        {
            Name = "Ripple",
            Instructions = new List<Pattern>
            {
                new() {
                    PatternType = PatternType.VIBRATE,
                    VibrateIntensity = new double[]{ 0.75, 0.75 },
                    Duration = 200
                }
            }
        };

        public static readonly StoredShellCommand Shockwave = new()
        {
            Name = "Shockwave",
            Instructions = new List<Pattern>
            {
                new() {
                    PatternType = PatternType.VIBRATE,
                    VibrateIntensity = new double[]{ 0.75, 0.75 },
                    Duration = 1000
                }
            }
        };
    }
}
