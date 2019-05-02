using NTMiner.User;
using NTMiner.User.Impl;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace NTMiner {
    public class HostRoot : IHostRoot {
        public static readonly IHostRoot Instance = new HostRoot();
        private static string s_sha1 = null;
        public static string Sha1 {
            get {
                if (s_sha1 == null) {
                    s_sha1 = HashUtil.Sha1(File.ReadAllBytes(Process.GetCurrentProcess().MainModule.FileName));
                }
                return s_sha1;
            }
        }

        public DateTime StartedOn { get; private set; } = DateTime.Now;

        public IUserSet UserSet { get; private set; }

        public void RefreshUserSet() {
            _userSet.Refresh();
        }

        private readonly UserSet _userSet;
        private HostRoot() {
            _userSet = new UserSet();
            this.UserSet = _userSet;
        }

        public static void StartTimer() {
            NTMinerRegistry.SetDaemonActiveOn(DateTime.Now);
            var timer = new System.Timers.Timer(10 * 1000);
            timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => {
                NTMinerRegistry.SetDaemonActiveOn(DateTime.Now);
            };
            timer.Start();
        }

        public static EventWaitHandle WaitHandle = new AutoResetEvent(false);
        private static Mutex _sMutexApp;
        // 注意：该程序编译成无界面的windows应用程序而不是控制台程序，从而随机自动启动时无界面
        [STAThread]
        static void Main(string[] args) {
            try {
                bool mutexCreated;
                try {
                    _sMutexApp = new Mutex(true, "NTMinerDaemonAppMutex", out mutexCreated);
                }
                catch {
                    mutexCreated = false;
                }
                if (mutexCreated) {
                    StartTimer();
                    NTMinerRegistry.SetDaemonVersion(Sha1);
                    NTMinerRegistry.SetAutoBoot("NTMinerDaemon", true);
                    bool isAutoBoot = NTMinerRegistry.GetIsAutoBoot();
                    if (isAutoBoot) {
                        string location = NTMinerRegistry.GetLocation();
                        if (!string.IsNullOrEmpty(location)) {
                            string arguments = NTMinerRegistry.GetArguments();
                            try {
                                Process.Start(location, arguments);
                            }
                            catch (Exception e) {
                                Logger.ErrorDebugLine(e.Message, e);
                            }
                        }
                    }
                    Run();
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }

        private static bool _isClosed = false;
        private static void Close() {
            if (!_isClosed) {
                _isClosed = true;
                HttpServer.Stop();
                _sMutexApp?.Dispose();
            }
        }

        public static void Exit() {
            Close();
            Environment.Exit(0);
        }

        private static void Run() {
            try {
                HttpServer.Start($"http://localhost:{WebApiConst.NTMinerDaemonPort}");
                Windows.ConsoleHandler.Register(Close);
                WaitHandle.WaitOne();
                Close();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
            finally {
                Close();
            }
        }
    }
}
