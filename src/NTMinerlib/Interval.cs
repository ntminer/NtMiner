using System;
using System.Timers;

namespace NTMiner {
    public static class Interval {
        public static void Start(TimeSpan per, Action perCallback, Action stopCallback, TimeSpan timeout, Func<bool> requestStop) {
            var timer = new Timer(per.TotalMilliseconds) { AutoReset = true };
            double milliseconds = 0;
            timer.Elapsed += (sender, e) => {
                milliseconds += per.TotalMilliseconds;
                perCallback?.Invoke();
                if (milliseconds >= timeout.TotalMilliseconds || (requestStop != null && requestStop.Invoke())) {
                    timer.Stop();
                    timer.Dispose();
                    stopCallback?.Invoke();
                }
            };
            timer.Start();
        }
    }
}
