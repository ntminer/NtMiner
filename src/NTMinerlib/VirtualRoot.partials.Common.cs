using NTMiner.User;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace NTMiner {
    public static partial class VirtualRoot {
        public static readonly bool IsGEWin10 = Environment.OSVersion.Version >= new Version(6, 2);
        public static readonly bool IsLTWin10 = Environment.OSVersion.Version < new Version(6, 2);
        public static RpcUser RpcUser { get; private set; } = RpcUser.Empty;

        public static void SetRpcUser(RpcUser rpcUser) {
            RpcUser = rpcUser;
        }

        public static Task Delay(this TimeSpan timeSpan) {
            var tcs = new TaskCompletionSource<object>();
            var timer = new Timer(timeSpan.TotalMilliseconds) { AutoReset = false };
            timer.Elapsed += (sender, e) => {
                timer.Stop();
                timer.Dispose();
                tcs.SetResult(null);
            };
            timer.Start();
            return tcs.Task;
        }

        public static Task MillisecondsDelay(this int n) {
            var tcs = new TaskCompletionSource<object>();
            var timer = new Timer(n) { AutoReset = false };
            timer.Elapsed += (sender, e) => {
                timer.Stop();
                timer.Dispose();
                tcs.SetResult(null);
            };
            timer.Start();
            return tcs.Task;
        }

        public static Task SecondsDelay(this int n) {
            var tcs = new TaskCompletionSource<object>();
            AddViaTimesLimitPath<Per1SecondEvent>("倒计时", LogEnum.None, message => {
                n--;
                if (n == 0) {
                    tcs.SetResult(null);
                }
            }, viaTimesLimit: n, typeof(VirtualRoot));
            return tcs.Task;
        }

        public static void SetInterval(TimeSpan per, Action perCallback, Action stopCallback, TimeSpan timeout, Func<bool> requestStop) {
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
