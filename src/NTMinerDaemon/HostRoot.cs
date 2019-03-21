using NTMiner.Serialization;
using NTMiner.User;
using NTMiner.User.Impl;
using System;
using System.Diagnostics;
using System.Threading;

namespace NTMiner {
    public class HostRoot : IHostRoot {
        public static readonly IHostRoot Current = new HostRoot();
        public static readonly IObjectSerializer JsonSerializer = new ObjectJsonSerializer();

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

        public static EventWaitHandle WaitHandle = new AutoResetEvent(false);
        public static ExtendedNotifyIcon NotifyIcon;
        private static Mutex s_mutexApp;
        static void Main(string[] args) {
            try {
                Console.Title = "NTMinerDaemon";
                bool mutexCreated;
                try {
                    s_mutexApp = new Mutex(true, "NTMinerDaemonAppMutex", out mutexCreated);
                }
                catch {
                    mutexCreated = false;
                }
                if (mutexCreated) {
                    NTMinerRegistry.SetAutoBoot("NTMinerDaemon", true);
                    Type thisType = typeof(HostRoot);
                    NotifyIcon = ExtendedNotifyIcon.Create(new System.Drawing.Icon(thisType.Assembly.GetManifestResourceStream(thisType, "logo.ico")), "挖矿端守护进程", isShowNotifyIcon: DevMode.IsDebugMode);
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
                s_mutexApp?.Dispose();
                NotifyIcon?.Dispose();
            }
        }

        public static void Exit() {
            Close();
            Environment.Exit(0);
        }

        private static void Run() {
            try {
                HttpServer.Start($"http://localhost:{WebApiConst.NTMinerDaemonPort}");
                Windows.ConsoleHandler.Register(() => {
                    Close();
                });
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
