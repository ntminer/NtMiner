using LiteDB;
using Microsoft.Win32;
using NTMiner.Core;
using NTMiner.Core.Impl;
using NTMiner.Serialization;
using NTMiner.Ws;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace NTMiner {
    public partial class VirtualRoot {
        public static IJsonSerializer JsonSerializer { get; private set; } = new NTJsonSerializer();
        public static IBinarySerializer BinarySerializer { get; private set; } = new BinarySerializer(JsonSerializer);
        public static IOperationResultSet OperationResultSet { get; private set; } = new OperationResultSet();
        public static IDaemonOperation DaemonOperation = new DaemonOperation();

        private static string _sha1 = null;
        private static string Sha1 {
            get {
                if (_sha1 == null) {
                    _sha1 = HashUtil.Sha1(File.ReadAllBytes(Process.GetCurrentProcess().MainModule.FileName));
                }
                return _sha1;
            }
        }

        public static DaemonWsClient DaemonWsClient;
        public static DateTime StartedOn { get; private set; } = DateTime.Now;

        static VirtualRoot() {
        }

        public static EventWaitHandle _waitHandle;
        private static Mutex _sMutexApp;
        // 注意：该程序编译成无界面的windows应用程序而不是控制台程序，从而随机自动启动时无界面
        [STAThread]
        static void Main(string[] args) {
            SetOut(new ConsoleOut());
            if (args.Length != 0) {
                if (args.Contains("--sha1", StringComparer.OrdinalIgnoreCase)) {
                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sha1"), Sha1);
                    return;
                }
            }
            try {
                SystemEvents.SessionEnding += SessionEndingEventHandler;
                StartTimer();
                _waitHandle = new AutoResetEvent(false);
                bool mutexCreated;
                try {
                    _sMutexApp = new Mutex(true, "NTMinerDaemonAppMutex", out mutexCreated);
                }
                catch {
                    mutexCreated = false;
                }
                if (mutexCreated) {
                    if (!DevMode.IsDevMode) {
                        Write.Disable();
                    }
                    NTMinerRegistry.SetDaemonVersion(Sha1);
                    NTMinerRegistry.SetAutoBoot("NTMinerDaemon", true);
                    #region 是否自动启动挖矿端
                    bool isAutoBoot = MinerProfileUtil.GetIsAutoBoot();
                    if (isAutoBoot) {
                        string location = NTMinerRegistry.GetLocation(NTMinerAppType.MinerClient);
                        if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                            string processName = Path.GetFileName(location);
                            Process[] processes = Process.GetProcessesByName(processName);
                            if (processes.Length == 0) {
                                string arguments = NTMinerRegistry.GetMinerClientArguments(NTMinerAppType.MinerClient);
                                try {
                                    Process.Start(location, arguments);
                                    Write.DevOk(() => $"启动挖矿端 {location} {arguments}");
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
                    #endregion
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
                HttpServer.Start($"http://{NTKeyword.Localhost}:{NTKeyword.NTMinerDaemonPort.ToString()}");
                DaemonWsClient = new DaemonWsClient();
                AddEventPath<Per2MinuteEvent>("每2分钟通过Ws通道上报一次算力", LogEnum.DevConsole, action: message => {
                    if (!DaemonWsClient.IsOpen) {
                        return;
                    }
                    RpcRoot.Client.MinerClientService.WsGetSpeedAsync((data, ex) => {
                        if (!DaemonWsClient.IsOpen) {
                            return;
                        }
                        DaemonWsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.Speed) {
                            Data = data
                        });
                    });
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

        public static LiteDatabase CreateLocalDb() {
            return new LiteDatabase($"filename={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NTKeyword.LocalDbFileName)}");
        }

        private static bool _isClosed = false;
        public static void Exit() {
            if (!_isClosed) {
                _isClosed = true;
                RaiseEvent(new AppExitEvent());
                RpcRoot.RpcUser?.Logout();
                NTMinerConsole.Free();
                _sMutexApp?.Dispose();
                Environment.Exit(0);
            }
        }
    }
}
