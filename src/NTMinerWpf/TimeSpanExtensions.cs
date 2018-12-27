using System;
using System.Threading.Tasks;

namespace NTMiner {
    public static class TimeSpanExtensions {
        public static Task Delay(this TimeSpan timeSpan) {
            var tcs = new TaskCompletionSource<object>();
            var timer = new System.Timers.Timer(timeSpan.TotalMilliseconds) { AutoReset = false };
            timer.Elapsed += delegate { timer.Dispose(); tcs.SetResult(null); };
            timer.Start();
            return tcs.Task;
        }
    }
}
