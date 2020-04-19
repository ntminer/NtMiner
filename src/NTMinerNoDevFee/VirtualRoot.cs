using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace NTMiner {
    public static partial class VirtualRoot {
        private static string s_sha1 = null;
        public static string Sha1 {
            get {
                if (s_sha1 == null) {
                    s_sha1 = HashUtil.Sha1(File.ReadAllBytes(Process.GetCurrentProcess().MainModule.FileName));
                }
                return s_sha1;
            }
        }

        public static DateTime StartedOn { get; private set; } = DateTime.Now;

        public static EventWaitHandle _waitHandle;
        private static Mutex _sMutexApp;
        // 注意：该程序编译成无界面的windows应用程序而不是控制台程序，从而随机自动启动时无界面
        [STAThread]
        static void Main(string[] args) {
            HomePath.SetHomeDirFullName(AppDomain.CurrentDomain.BaseDirectory);
            if (args.Length != 0) {
                if (args.Contains("--sha1")) {
                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sha1"), Sha1);
                    return;
                }
            }
            try {
                if (DevMode.IsDevMode) {
                    NTMinerConsole.GetOrAlloc();
                }
                SystemEvents.SessionEnding += SessionEndingEventHandler;
                StartTimer();
                _waitHandle = new AutoResetEvent(false);
                bool mutexCreated;
                try {
                    _sMutexApp = new Mutex(true, "NTMinerNoDevFeeAppMutex", out mutexCreated);
                }
                catch {
                    mutexCreated = false;
                }
                if (mutexCreated) {
                    if (!DevMode.IsDevMode) {
                        Write.Disable();
                    }
                    NTMinerRegistry.SetNoDevFeeVersion(Sha1);
                    NTMinerRegistry.SetAutoBoot("NTMinerNoDevFee", true);
                    Run();
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private static void Run() {
            try {
                Windows.ConsoleHandler.Register(Exit);
                AddEventPath<Per10SecondEvent>("呼吸表示活着", LogEnum.None,
                    action: message => {
                        NTMinerRegistry.SetNoDevFeeActiveOn(DateTime.Now);
                        NoDevFee.NoDevFeeUtil.StartAsync();
                    }, typeof(VirtualRoot));
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

        private static bool _isClosed = false;
        public static void Exit() {
            if (!_isClosed) {
                _isClosed = true;
                RaiseEvent(new AppExitEvent());
                _sMutexApp?.Dispose();
                NTMinerConsole.Free();
                Environment.Exit(0);
            }
        }
    }
}
