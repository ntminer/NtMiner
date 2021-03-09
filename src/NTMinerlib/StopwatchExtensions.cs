using System.Diagnostics;

namespace NTMiner {
    public static class StopwatchExtensions {
        public static double GetElapsedSeconds(this Stopwatch stopwatch) {
            return stopwatch.ElapsedMilliseconds / 1000.0;
        }
    }
}
