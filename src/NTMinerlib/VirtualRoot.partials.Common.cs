using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace NTMiner {
    public static partial class VirtualRoot {
        // 因为多个受保护区域中可能会互相访问，用一把锁可以避免死锁。不用多把锁是因为没有精力去检查每一个受保护区域确保它们不会互相访问导致死锁。
        private static readonly object _locker = new object();
        private static readonly Process _appProcess = Process.GetCurrentProcess();
        public static string AppFileFullName {
            get { return _appProcess.MainModule.FileName; }
        }

        // 这是任务管理器看到的内存，不等于AppProcess.WorkingSet64 - AppProcess.PrivateMemorySize64
        // 因为耗时，所以延迟启动
        private static PerformanceCounter _memoryCounter;
        private static PerformanceCounter _threadCounter;
        private static PerformanceCounter _handleCounter;
        private static readonly object _lockForPerformanceCounter = new object();
        private static void PerformanceCounterInitOnece() {
            string processInstanceName = GetProcessInstanceName();
            if (_memoryCounter == null || _memoryCounter.InstanceName != processInstanceName) {
                lock (_lockForPerformanceCounter) {
                    if (_memoryCounter == null || _memoryCounter.InstanceName != processInstanceName) {
                        _memoryCounter?.Dispose();
                        _threadCounter?.Dispose();
                        _handleCounter?.Dispose();
                        _memoryCounter = new PerformanceCounter("Process", "Working Set - Private", processInstanceName, readOnly: true);
                        _threadCounter = new PerformanceCounter("Process", "Thread Count", processInstanceName, readOnly: true);
                        _handleCounter = new PerformanceCounter("Process", "Handle Count", processInstanceName, readOnly: true);
                    }
                }
            }
        }

        private static int[] _processIds = new int[0];
        private static string _processInstanceName = _appProcess.ProcessName;
        private static string GetProcessInstanceName() {
            int[] processIds = Process.GetProcessesByName(_appProcess.ProcessName).Select(a => a.Id).ToArray();
            bool isChanged = _processIds.Length != processIds.Length || _processIds.Any(a => !processIds.Contains(a));
            if (isChanged) {
                _processIds = processIds;
                string processInstanceName = _appProcess.ProcessName;
                PerformanceCounterCategory cat = new PerformanceCounterCategory("Process");
                // 同一个程序如果运行多次，对应的多个进程名称是通过后缀#1、#2、#3...区分的
                string[] instanceNames = cat.GetInstanceNames().Where(a => a.StartsWith(_appProcess.ProcessName)).ToArray();
                foreach (string instanceName in instanceNames) {
                    using (PerformanceCounter cnt = new PerformanceCounter("Process", "ID Process", instanceName, true)) {
                        int val = (int)cnt.RawValue;
                        if (val == _appProcess.Id) {
                            processInstanceName = instanceName;
                            break;
                        }
                    }
                }
                _processInstanceName = processInstanceName;
            }
            return _processInstanceName;
        }

        public static double ProcessMemoryMb {
            get {
                PerformanceCounterInitOnece();
                return _memoryCounter.RawValue / NTKeyword.DoubleM;
            }
        }
        public static long ThreadCount {
            get {
                PerformanceCounterInitOnece();
                return _threadCounter.RawValue;
            }
        }
        public static long HandleCount {
            get {
                PerformanceCounterInitOnece();
                return _handleCounter.RawValue;
            }
        }

        /// <summary>
        /// 是否是Win10或更新版本的windows
        /// </summary>
        public static readonly bool IsGEWin10 = Environment.OSVersion.Version >= new Version(6, 2);
        /// <summary>
        /// 是否是比Win10更旧版本的windows
        /// </summary>
        public static readonly bool IsLTWin10 = !IsGEWin10;

        public static readonly SessionEndingEventHandler SessionEndingEventHandler = (sender, e) => {
            OsSessionEndingEvent.ReasonSessionEnding reason;
            switch (e.Reason) {
                case SessionEndReasons.Logoff:
                    reason = OsSessionEndingEvent.ReasonSessionEnding.Logoff;
                    break;
                case SessionEndReasons.SystemShutdown:
                    reason = OsSessionEndingEvent.ReasonSessionEnding.Shutdown;
                    break;
                default:
                    reason = OsSessionEndingEvent.ReasonSessionEnding.Unknown;
                    break;
            }
            RaiseEvent(new OsSessionEndingEvent(reason));
        };

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
            BuildViaTimesLimitPath<Per1SecondEvent>("倒计时", LogEnum.None, viaTimesLimit: n, AnonymousMessagePath.Location, PathPriority.Normal, message => {
                n--;
                if (n == 0) {
                    tcs.SetResult(null);
                }
            });
            return tcs.Task;
        }

        /// <summary>
        /// 如果是在比如Wpf的界面线程中调用该方法，注意用UIThread回调
        /// </summary>
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

        // 登录名中的保留字
        private static readonly HashSet<string> _reservedLoginNameWords = new HashSet<string> {
            "ntminer",
            "bitzero"
        };
        public static bool IsValidLoginName(string loginName, out string message) {
            message = string.Empty;
            if (string.IsNullOrEmpty(loginName)) {
                message = "登录名不能为空";
                return false;
            }
            foreach (var word in _reservedLoginNameWords) {
                if (loginName.IndexOf(word, StringComparison.OrdinalIgnoreCase) != -1) {
                    message = "登录名中不能包含保留字";
                    return false;
                }
            }
            if (loginName.IndexOf(' ') != -1) {
                message = "登录名中不能包含空格";
                return false;
            }
            if (loginName.IndexOf('@') != -1) {
                message = "登录名中不能包含@符号";
                return false;
            }
            if (loginName.Length == 11 && loginName.All(a => char.IsDigit(a))) {
                message = "登录名不能是11位的纯数字，提示：请添加非数字字符以和手机号码区分";
                return false;
            }
            return true;
        }
    }
}
