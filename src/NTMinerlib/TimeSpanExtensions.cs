using System;
using System.Threading.Tasks;
using System.Timers;

namespace NTMiner {
    public static class TimeSpanExtensions {
        public static Task Delay(this TimeSpan timeSpan) {
            var tcs = new TaskCompletionSource<object>();
            var timer = new Timer(timeSpan.TotalMilliseconds) { AutoReset = false };
            timer.Elapsed += (sender, e)=> {
                timer.Dispose();
                tcs.SetResult(null);
            };
            timer.Start();
            return tcs.Task;
        }

        public static Task Delay(this TimeSpan timeSpan, Action<int> perSecondCallback) {
            var tcs = new TaskCompletionSource<object>();
            var timer = new Timer(1000);
            int totalSeconds = (int)timeSpan.TotalSeconds;
            timer.Elapsed += (sender, e) => {
                perSecondCallback?.Invoke(totalSeconds);
                totalSeconds--;
                if (totalSeconds <= 0) {
                    timer.Dispose();
                    tcs.SetResult(null);
                }
            };
            timer.Start();
            return tcs.Task;
        }
    }
}
