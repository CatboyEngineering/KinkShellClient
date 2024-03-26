using CatboyEngineering.KinkShell.Models.Toy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.KinkShell.Toy
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
                    Intensity = 0.25,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.5,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.75,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.85,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 1,
                    Duration = 200,
                    Delay = 1000,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.25,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.5,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.75,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.85,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 1,
                    Duration = 200,
                    Delay = 1000,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 1,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.85,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.75,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.5,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.25,
                    Duration = 200,
                    Delay = 1000,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 1,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.85,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.75,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.50,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.25,
                    Duration = 200,
                    Delay = 1000,
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
                    Intensity = 0.75,
                    Duration = 1000,
                    Delay = 500,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.75,
                    Duration = 1000,
                    Delay = 500,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 1,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.50,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 1,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.50,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 1,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.50,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 1,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.50,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.75,
                    Duration = 1000,
                    Delay = 500,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.75,
                    Duration = 1000,
                    Delay = 500,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 1,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.50,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 1,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.50,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 1,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.50,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 1,
                    Duration = 200,
                    Delay = 100,
                },
                new() {
                    PatternType = PatternType.VIBRATE,
                    Intensity = 0.50,
                    Duration = 200,
                    Delay = 100,
                }
            }
        };
    }
}
