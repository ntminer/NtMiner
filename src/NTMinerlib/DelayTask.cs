using System;
using System.Threading.Tasks;
using System.Timers;

namespace NTMiner {
    public static class DelayTask {
        public static Task Delay(this TimeSpan timeSpan) {
            var tcs = new TaskCompletionSource<object>();
#pragma warning disable IDE0067 // 丢失范围之前释放对象
            var timer = new Timer(timeSpan.TotalMilliseconds) { AutoReset = false };
#pragma warning restore IDE0067 // 丢失范围之前释放对象
            timer.Elapsed += (sender, e) => {
                timer.Dispose();
                tcs.SetResult(null);
            };
            timer.Start();
            return tcs.Task;
        }

        public static Task MillisecondsDelay(this int n) {
            var tcs = new TaskCompletionSource<object>();
#pragma warning disable IDE0067 // 丢失范围之前释放对象
            var timer = new Timer(n) { AutoReset = false };
#pragma warning restore IDE0067 // 丢失范围之前释放对象
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
