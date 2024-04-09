using CatboyEngineering.KinkShellClient.Models.Toy;
using System.Collections.Generic;

namespace CatboyEngineering.KinkShellClient.Toy
{
    public static class DefaultPatterns
    {
        public static readonly StoredShellCommand Pulse = new()
        {
            Name = "Pulse",
            Instructions = new List<Pattern>
            {
                new() {
                    PatternType = PatternType.VIBRATE,
                    VibrateIntensity = new double[]{ 0.4, 0 },
                    Duration = 300
                },
                new() {
                    PatternType = PatternType.NONE,
                    Duration = 500
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    VibrateIntensity = new double[]{ 0.4, 0 },
                    Duration = 300
                },
                new() {
                    PatternType = PatternType.NONE,
                    Duration = 100
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    VibrateIntensity = new double[]{ 0.4, 0 },
                    Duration = 300
                },
                new() {
                    PatternType = PatternType.NONE,
                    Duration = 500
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    VibrateIntensity = new double[]{ 0.5, 0 },
                    Duration = 300
                }
            }
        };
    }
}
