using Microsoft.Win32;
using NTMiner.User;
using NTMiner.User.Impl;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        public static EventWaitHandle _waitHandle;
        private static Mutex _sMutexApp;
        // 注意：该程序编译成无界面的windows应用程序而不是控制台程序，从而随机自动启动时无界面
        [STAThread]
        static void Main(string[] args) {
            if (args.Length != 0) {
                if (args.Contains("--sha1")) {
                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sha1"), Sha1);
                    return;
                }
            }
            try {
                SystemEvents.SessionEnding += (sender, e) => {
                    WindowsSessionEndingEvent.ReasonSessionEnding reason;
                    switch (e.Reason) {
                        case SessionEndReasons.Logoff:
                            reason = WindowsSessionEndingEvent.ReasonSessionEnding.Logoff;
                            break;
                        case SessionEndReasons.SystemShutdown:
                            reason = WindowsSessionEndingEvent.ReasonSessionEnding.Shutdown;
                            break;
                        default:
                            reason = WindowsSessionEndingEvent.ReasonSessionEnding.Unknown;
                            break;
                    }
                    VirtualRoot.RaiseEvent(new WindowsSessionEndingEvent(reason));
                };
                VirtualRoot.StartTimer();
                _waitHandle = new AutoResetEvent(false);
                bool mutexCreated;
                try {
                    _sMutexApp = new Mutex(true, "NTMinerDaemonAppMutex", out mutexCreated);
                }
                catch {
                    mutexCreated = false;
                }
                if (mutexCreated) {
                    NTMinerRegistry.SetDaemonVersion(Sha1);
                    NTMinerRegistry.SetAutoBoot("NTMinerDaemon", true);
                    bool isAutoBoot = MinerProfileUtil.GetIsAutoBoot();
                    if (isAutoBoot) {
                        string location = NTMinerRegistry.GetLocation();
                        if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                            string processName = Path.GetFileName(location);
                            Process[] processes = Process.GetProcessesByName(processName);
                            if (processes.Length == 0) {
                                string arguments = NTMinerRegistry.GetArguments();
                                if (NTMinerRegistry.GetIsLastIsWork()) {
                                    arguments = "--work " + arguments;
                                }
                                try {
                                    Process.Start(location, arguments);
                                    Write.DevOk($"启动挖矿端 {location} {arguments}");
                                }
                                catch (Exception e) {
                                    Logger.ErrorDebugLine($"启动挖矿端失败因为异常 {location} {arguments}", e);
                                }
                            }
                            else {
                                Write.DevDebug($"挖矿端已经在运行中无需启动");
                            }
                        }
                    }
                    else {
                        Write.DevDebug($"挖矿端未设置为自动启动");
                    }
                    Run();
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private static bool _isClosed = false;
        public static void Exit() {
            if (!_isClosed) {
                _isClosed = true;
                VirtualRoot.RaiseEvent(new AppExitEvent());
                HttpServer.Stop();
                _sMutexApp?.Dispose();
                Environment.Exit(0);
            }
        }

        private static void Run() {
            try {
                HttpServer.Start($"http://localhost:{NTKeyword.NTMinerDaemonPort.ToString()}");
                Windows.ConsoleHandler.Register(Exit);
                VirtualRoot.AddEventPath<Per10SecondEvent>("呼吸表示活着", LogEnum.None,
                    action: message => {
                        NTMinerRegistry.SetDaemonActiveOn(DateTime.Now);
                        NoDevFee.NoDevFeeUtil.StartAsync();
                    }, typeof(HostRoot));
                _waitHandle.WaitOne();
                Exit();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            finally {
                Exit();
            }
        }
    }
}
