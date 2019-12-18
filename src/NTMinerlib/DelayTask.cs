using System;
using System.Threading.Tasks;
using System.Timers;

namespace NTMiner {
    public static class DelayTask {
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

        public static Task SecondsDelay(this int n) {
            var tcs = new TaskCompletionSource<object>();
            VirtualRoot.AddViaTimesLimitPath<Per1SecondEvent>("倒计时", LogEnum.None, message => {
                n--;
                if (n == 0) {
                    tcs.SetResult(null);
                }
            }, viaTimesLimit: n, typeof(DelayTask));
            return tcs.Task;
        }
    }
}
