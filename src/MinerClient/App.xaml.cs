using NTMiner.Core;
using NTMiner.Notifications;
using NTMiner.RemoteDesktop;
using NTMiner.View;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace NTMiner {
    public partial class App : Application, IDisposable {
        public App() {
            VirtualRoot.SetOut(NotiCenterWindowViewModel.Instance);
            LogDir.SetDir(SpecialPath.LogsDirFullName);
            AppUtil.Init(this);
            InitializeComponent();
        }

        private readonly IAppViewFactory _appViewFactory = new AppViewFactory();

        private bool createdNew;
        private Mutex appMutex;
        private static readonly string s_appPipName = "ntminerclient";
        protected override void OnExit(ExitEventArgs e) {
            AppContext.NotifyIcon?.Dispose();
            NTMinerRoot.Instance.Exit();
            HttpServer.Stop();
            base.OnExit(e);
            NTMinerConsole.Free();
        }

        protected override void OnStartup(StartupEventArgs e) {
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            // 通过群控升级挖矿端的时候升级器可能不存在所以需要下载，下载的时候需要用到下载器所以下载器需要提前注册
            VirtualRoot.BuildCmdPath<ShowFileDownloaderCommand>(action: message => {
                UIThread.Execute(() => {
                    FileDownloader.ShowWindow(message.DownloadFileUrl, message.FileTitle, message.DownloadComplete);
                });
            });
            VirtualRoot.BuildCmdPath<UpgradeCommand>(action: message => {
                AppStatic.Upgrade(message.FileName, message.Callback);
            });
            if (!string.IsNullOrEmpty(CommandLineArgs.Upgrade)) {
                VirtualRoot.Execute(new UpgradeCommand(CommandLineArgs.Upgrade, () => {
                    UIThread.Execute(() => { Environment.Exit(0); });
                }));
            }
            else {
                try {
                    appMutex = new Mutex(true, s_appPipName, out createdNew);
                }
                catch (Exception) {
                    createdNew = false;
                }
                if (createdNew) {
                    Logger.InfoDebugLine($"==================NTMiner.exe {MainAssemblyInfo.CurrentVersion.ToString()}==================");
                    if (!NTMiner.Windows.WMI.IsWmiEnabled) {
                        DialogWindow.ShowDialog(new DialogWindowViewModel(
                            message: "开源矿工无法运行所需的组件，因为本机未开启WMI服务，开源矿工需要使用WMI服务检测windows的内存、显卡等信息，请先手动开启WMI。",
                            title: "提醒",
                            icon: "Icon_Error"));
                        Shutdown();
                        Environment.Exit(0);
                    }

                    NotiCenterWindowViewModel.IsHotKeyEnabled = true;
                    SplashApp.NTMinerSplash.Run();
                    //ConsoleWindow.Instance.Show();
                    NotiCenterWindow.ShowWindow();
                    if (!NTMiner.Windows.Role.IsAdministrator) {
                        NotiCenterWindowViewModel.Instance.Manager
                            .CreateMessage()
                            .Warning("请以管理员身份运行。")
                            .WithButton("点击以管理员身份运行", button => {
                                WpfUtil.RunAsAdministrator();
                            })
                            .Dismiss().WithButton("忽略", button => {

                            }).Queue();
                    }
                    VirtualRoot.BuildEventPath<StartingMineFailedEvent>("开始挖矿失败", LogEnum.DevConsole,
                        action: message => {
                            AppContext.Instance.MinerProfileVm.IsMining = false;
                            VirtualRoot.Out.ShowError(message.Message);
                        });
                    NTMinerRoot.Instance.Init(() => {
                        _appViewFactory.Link();
                        if (VirtualRoot.IsLTWin10) {
                            VirtualRoot.ThisLocalWarn(nameof(App), AppStatic.LowWinMessage, toConsole: true);
                        }
                        if (NTMinerRoot.Instance.GpuSet.Count == 0) {
                            VirtualRoot.ThisLocalError(nameof(App), "没有矿卡或矿卡未驱动。", toConsole: true);
                        }
                        if (NTMinerRoot.Instance.ServerContext.CoinSet.Count == 0) {
                            VirtualRoot.ThisLocalError(nameof(App), "访问阿里云失败，请尝试更换本机dns解决此问题。", toConsole: true);
                        }
                        UIThread.Execute(() => {
                            if (NTMinerRoot.Instance.MinerProfile.IsNoUi && NTMinerRoot.Instance.MinerProfile.IsAutoStart) {
                                ConsoleWindow.Instance.Hide();
                                VirtualRoot.Out.ShowSuccess("已切换为无界面模式运行，可在选项页调整设置", "开源矿工");
                            }
                            else {
                                _appViewFactory.ShowMainWindow(isToggle: false);
                            }
                            SplashApp.NTMinerSplash.Kill();
                            StartStopMineButtonViewModel.Instance.AutoStart();
                            AppContext.NotifyIcon = ExtendedNotifyIcon.Create("开源矿工", isMinerStudio: false);
                            ConsoleWindow.Instance.HideSplash();
                        });
                        #region 处理显示主界面命令
                        VirtualRoot.BuildCmdPath<ShowMainWindowCommand>(action: message => {
                            ShowMainWindow(message.IsToggle);
                        });
                        #endregion
                        Task.Factory.StartNew(() => {
                            if (NTMinerRoot.Instance.MinerProfile.IsAutoDisableWindowsFirewall) {
                                Firewall.DisableFirewall();
                            }
                            if (!Firewall.IsMinerClientRuleExists()) {
                                Firewall.AddMinerClientRule();
                            }
                            try {
                                HttpServer.Start($"http://localhost:{NTKeyword.MinerClientPort}");
                                Daemon.DaemonUtil.RunNTMinerDaemon();
                            }
                            catch (Exception ex) {
                                Logger.ErrorDebugLine(ex);
                            }
                        });
                    });
                    Link();
                }
                else {
                    try {
                        _appViewFactory.ShowMainWindow(this, MinerServer.NTMinerAppType.MinerClient);
                    }
                    catch (Exception) {
                        DialogWindow.ShowDialog(new DialogWindowViewModel(
                            message: "另一个NTMiner正在运行，请手动结束正在运行的NTMiner进程后再次尝试。",
                            title: "提醒",
                            icon: "Icon_Error"));
                        Process currentProcess = Process.GetCurrentProcess();
                        NTMiner.Windows.TaskKill.KillOtherProcess(currentProcess);
                    }
                }
            }
            base.OnStartup(e);
        }

        private void ShowMainWindow(bool isToggle) {
            UIThread.Execute(() => {
                _appViewFactory.ShowMainWindow(isToggle);
                // 使状态栏显示显示最新状态
                if (NTMinerRoot.Instance.IsMining) {
                    var mainCoin = NTMinerRoot.Instance.LockedMineContext.MainCoin;
                    if (mainCoin == null) {
                        return;
                    }
                    var coinShare = NTMinerRoot.Instance.CoinShareSet.GetOrCreate(mainCoin.GetId());
                    VirtualRoot.RaiseEvent(new ShareChangedEvent(coinShare));
                    if ((NTMinerRoot.Instance.LockedMineContext is IDualMineContext dualMineContext) && dualMineContext.DualCoin != null) {
                        coinShare = NTMinerRoot.Instance.CoinShareSet.GetOrCreate(dualMineContext.DualCoin.GetId());
                        VirtualRoot.RaiseEvent(new ShareChangedEvent(coinShare));
                    }
                    AppContext.Instance.GpuSpeedVms.Refresh();
                }
            });
        }

        private void Link() {
            VirtualRoot.BuildCmdPath<CloseNTMinerCommand>(action: message => {
                // 不能推迟这个日志记录的时机，因为推迟会有windows异常日志
                VirtualRoot.ThisLocalInfo(nameof(NTMinerRoot), $"退出{VirtualRoot.AppName}");
                UIThread.Execute(() => {
                    try {
                        Shutdown();
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                        Environment.Exit(0);
                    }
                });
            });
            #region 周期确保守护进程在运行
            VirtualRoot.BuildEventPath<Per1MinuteEvent>("周期确保守护进程在运行", LogEnum.DevConsole,
                action: message => {
                    Daemon.DaemonUtil.RunNTMinerDaemon();
                });
            #endregion
            #region 开始和停止挖矿后
            VirtualRoot.BuildEventPath<MineStartedEvent>("启动1080ti小药丸、启动DevConsole? 更新挖矿按钮状态", LogEnum.DevConsole,
                action: message => {
                    AppContext.Instance.MinerProfileVm.IsMining = true;
                    StartStopMineButtonViewModel.Instance.BtnStopText = "正在挖矿";
                    // 启动DevConsole
                    if (NTMinerRoot.IsUseDevConsole) {
                        var mineContext = message.MineContext;
                        string poolIp = mineContext.MainCoinPool.GetIp();
                        string consoleTitle = mineContext.MainCoinPool.Server;
                        Daemon.DaemonUtil.RunDevConsoleAsync(poolIp, consoleTitle);
                    }
                    OhGodAnETHlargementPill.OhGodAnETHlargementPillUtil.Start();
                });
            VirtualRoot.BuildEventPath<MineStopedEvent>("停止挖矿后停止1080ti小药丸 挖矿停止后更新界面挖矿状态", LogEnum.DevConsole,
                action: message => {
                    AppContext.Instance.MinerProfileVm.IsMining = false;
                    StartStopMineButtonViewModel.Instance.BtnStopText = "尚未开始";
                    OhGodAnETHlargementPill.OhGodAnETHlargementPillUtil.Stop();
                });
            #endregion
            #region 处理禁用win10系统更新
            VirtualRoot.BuildCmdPath<BlockWAUCommand>(action: message => {
                NTMiner.Windows.WindowsUtil.BlockWAU();
            });
            #endregion
            #region 优化windows
            VirtualRoot.BuildCmdPath<Win10OptimizeCommand>(action: message => {
                NTMiner.Windows.WindowsUtil.Win10Optimize();
            });
            #endregion
            #region 处理开启A卡计算模式
            VirtualRoot.BuildCmdPath<SwitchRadeonGpuCommand>(action: message => {
                if (NTMinerRoot.Instance.GpuSet.GpuType == GpuType.AMD) {
                    SwitchRadeonGpuMode(message.On);
                }
            });
            #endregion
            #region 处理A卡驱动签名
            VirtualRoot.BuildCmdPath<AtikmdagPatcherCommand>(action: message => {
                if (NTMinerRoot.Instance.GpuSet.GpuType == GpuType.AMD) {
                    AtikmdagPatcher.AtikmdagPatcherUtil.Run();
                }
            });
            #endregion
            #region 启用或禁用windows远程桌面
            VirtualRoot.BuildCmdPath<EnableWindowsRemoteDesktopCommand>(action: message => {
                if (NTMinerRegistry.GetIsRemoteDesktopEnabled()) {
                    return;
                }
                string msg = "确定启用Windows远程桌面吗？";
                DialogWindow.ShowDialog(new DialogWindowViewModel(
                    message: msg,
                    title: "确认",
                    onYes: () => {
                        Rdp.SetRdpEnabled(true);
                        Firewall.AddRdpRule();
                    }));
            });
            #endregion
            #region 启用或禁用windows开机自动登录
            VirtualRoot.BuildCmdPath<EnableOrDisableWindowsAutoLoginCommand>(action: message => {
                if (NTMiner.Windows.OS.Instance.IsAutoAdminLogon) {
                    return;
                }
                NTMiner.Windows.Cmd.RunClose("control", "userpasswords2");
            });
            #endregion
        }

        private static void SwitchRadeonGpuMode(bool on) {
            SwitchRadeonGpu.SwitchRadeonGpu.Run(on, (isSuccess, e) => {
                if (isSuccess) {
                    if (on) {
                        VirtualRoot.ThisLocalInfo(nameof(App), "开启A卡计算模式成功", OutEnum.Success);
                    }
                    else {
                        VirtualRoot.ThisLocalInfo(nameof(App), "关闭A卡计算模式成功", OutEnum.Success);
                    }
                }
                else if (e != null) {
                    VirtualRoot.Out.ShowError(e.Message, delaySeconds: 4);
                }
                else {
                    if (on) {
                        VirtualRoot.ThisLocalError(nameof(App), "开启A卡计算模式失败", OutEnum.Warn);
                    }
                    else {
                        VirtualRoot.ThisLocalError(nameof(App), "关闭A卡计算模式失败", OutEnum.Warn);
                    }
                }
            });
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
