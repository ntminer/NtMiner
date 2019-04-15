using NTMiner.Core;
using NTMiner.OverClock;
using NTMiner.Views;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace NTMiner {
    public partial class App : Application, IDisposable {
        public App() {
            Logging.LogDir.SetDir(System.IO.Path.Combine(VirtualRoot.GlobalDirFullName, "Logs"));
            AppHelper.Init(this);
            InitializeComponent();
        }

        private bool createdNew;
        private Mutex appMutex;
        private static string s_appPipName = "ntminerclient";
        protected override void OnExit(ExitEventArgs e) {
            AppHelper.NotifyIcon?.Dispose();
            NTMinerRoot.Current.Exit();
            HttpServer.Stop();
            base.OnExit(e);
            ConsoleManager.Hide();
        }

        protected override void OnStartup(StartupEventArgs e) {
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            if (!string.IsNullOrEmpty(CommandLineArgs.Upgrade)) {
                Vms.AppStatic.Upgrade(CommandLineArgs.Upgrade, () => {
                    Environment.Exit(0);
                });
            }
            else {
                try {
                    appMutex = new Mutex(true, s_appPipName, out createdNew);
                }
                catch (Exception) {
                    createdNew = false;
                }
                if (createdNew) {
                    Vms.AppStatic.IsMinerClient = true;
                    SplashWindow splashWindow = new SplashWindow();
                    splashWindow.Show();
                    if (!NTMiner.Windows.WMI.IsWmiEnabled) {
                        DialogWindow.ShowDialog(message: "开源矿工无法运行所需的组件，因为本机未开机WMI服务，开源矿工需要使用WMI服务检测windows的内存、显卡等信息，请先手动开启WMI。", title: "提醒", icon: "Icon_Error");
                        Shutdown();
                        Environment.Exit(0);
                    }
                    NTMinerRoot.KernelBrandId = Brand.BrandUtil.KernelBrandId;
                    NotiCenterWindow.Instance.Show();
                    NTMinerRoot.AppName = "开源矿工挖矿客户端";
                    NTMinerRoot.Current.Init(() => {
                        NTMinerRoot.KernelDownloader = new KernelDownloader();
                        UIThread.Execute(() => {
                            MainWindow window = new MainWindow();
                            IMainWindow mainWindow = window;
                            this.MainWindow = window;
                            this.MainWindow.Show();
                            System.Drawing.Icon icon = new System.Drawing.Icon(GetResourceStream(new Uri("pack://application:,,,/NTMiner;component/logo.ico")).Stream);
                            AppHelper.NotifyIcon = ExtendedNotifyIcon.Create(icon, "挖矿端", isMinerStudio: false);
                            #region 处理显示主界面命令
                            VirtualRoot.Window<ShowMainWindowCommand>("处理显示主界面命令", LogEnum.None,
                                action: message => {
                                    Dispatcher.Invoke((ThreadStart)mainWindow.ShowThisWindow);
                                });
                            #endregion
                            splashWindow?.Close();
                            Task.Factory.StartNew(() => {
                                try {
                                    HttpServer.Start($"http://localhost:{WebApiConst.MinerClientPort}");
                                    NTMinerRoot.Current.Start();
                                }
                                catch (Exception ex) {
                                    Logger.ErrorDebugLine(ex.Message, ex);
                                }
                            });
                        });
                    });
                    VirtualRoot.Window<CloseNTMinerCommand>("处理关闭NTMiner客户端命令", LogEnum.UserConsole,
                        action: message => {
                            UIThread.Execute(() => {
                                if (MainWindow != null) {
                                    MainWindow.Close();
                                }
                                Shutdown();
                                Environment.Exit(0);
                            });
                        });
                    #region 周期确保守护进程在运行
                    Daemon.DaemonUtil.RunNTMinerDaemon();
                    VirtualRoot.On<Per20SecondEvent>("周期确保守护进程在运行", LogEnum.None,
                        action: message => {
                            Daemon.DaemonUtil.RunNTMinerDaemon();
                        });
                    #endregion
                    VirtualRoot.On<MineStartedEvent>("开始挖矿后启动1080ti小药丸、挖矿开始后如果需要启动DevConsole则启动DevConsole", LogEnum.DevConsole,
                        action: message => {
                            // 启动DevConsole
                            if (NTMinerRoot.IsUseDevConsole) {
                                var mineContext = message.MineContext;
                                string poolIp = mineContext.MainCoinPool.GetIp();
                                string consoleTitle = mineContext.MainCoinPool.Server;
                                Daemon.DaemonUtil.RunDevConsoleAsync(poolIp, consoleTitle);
                            }
                            OhGodAnETHlargementPill.OhGodAnETHlargementPillUtil.Start();
                        });
                    VirtualRoot.On<MineStopedEvent>("停止挖矿后停止1080ti小药丸", LogEnum.DevConsole,
                        action: message => {
                            OhGodAnETHlargementPill.OhGodAnETHlargementPillUtil.Stop();
                        });
                    NTMinerOverClockUtil.ExtractResource();
                }
                else {
                    try {
                        AppHelper.ShowMainWindow(this, MinerServer.NTMinerAppType.MinerClient);
                    }
                    catch (Exception) {
                        DialogWindow.ShowDialog(message: "另一个NTMiner正在运行，请手动结束正在运行的NTMiner进程后再次尝试。", title: "提醒", icon: "Icon_Error");
                        Process currentProcess = Process.GetCurrentProcess();
                        NTMiner.Windows.TaskKill.KillOtherProcess(currentProcess);
                    }
                }
            }
            base.OnStartup(e);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (disposing) {
                if (appMutex != null) {
                    appMutex.Dispose();
                }
            }
        }
    }
}
