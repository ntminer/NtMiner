using NTMiner.Core;
using NTMiner.Notifications;
using NTMiner.RemoteDesktop;
using NTMiner.View;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace NTMiner {
    public partial class App : Application {
        public App() {
            VirtualRoot.SetOut(NotiCenterWindowViewModel.Instance);
            Logger.SetDir(SpecialPath.LogsDirFullName);
            AppUtil.Init(this);
            InitializeComponent();
        }

        private readonly IAppViewFactory _appViewFactory = new AppViewFactory();

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
            VirtualRoot.AddCmdPath<ShowFileDownloaderCommand>(action: message => {
                UIThread.Execute(() => {
                    FileDownloader.ShowWindow(message.DownloadFileUrl, message.FileTitle, message.DownloadComplete);
                });
            }, location: this.GetType());
            VirtualRoot.AddCmdPath<UpgradeCommand>(action: message => {
                AppStatic.Upgrade(message.FileName, message.Callback);
            }, location: this.GetType());
            if (!string.IsNullOrEmpty(CommandLineArgs.Upgrade)) {
                VirtualRoot.Execute(new UpgradeCommand(CommandLineArgs.Upgrade, () => {
                    UIThread.Execute(() => { Environment.Exit(0); });
                }));
            }
            else {
                if (AppUtil.GetMutex("ntminerclient")) {
                    Logger.InfoDebugLine($"==================NTMiner.exe {EntryAssemblyInfo.CurrentVersion.ToString()}==================");
                    NotiCenterWindowViewModel.IsHotKeyEnabled = true;
                    SplashWindow splashWindow = null;
                    // 在另一个UI线程运行欢迎界面以确保欢迎界面的响应不被耗时的主界面初始化过程阻塞
                    // 注意：必须确保SplashWindow没有用到任何其它界面用到的依赖对象
                    SplashWindow.ShowWindowAsync(window => {
                        splashWindow = window;
                    });
                    //ConsoleWindow.Instance.Show();
                    NotiCenterWindow.Instance.ShowWindow();
                    if (!NTMiner.Windows.WMI.IsWmiEnabled) {
                        DialogWindow.ShowSoftDialog(new DialogWindowViewModel(
                            message: "开源矿工无法运行所需的组件，因为本机未开启WMI服务，开源矿工需要使用WMI服务检测windows的内存、显卡等信息，请先手动开启WMI。",
                            title: "提醒",
                            icon: "Icon_Error"));
                        Shutdown();
                        Environment.Exit(0);
                    }
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
                            AppContext.NotifyIcon = ExtendedNotifyIcon.Create("开源矿工", isMinerStudio: false);
                            if (NTMinerRoot.Instance.MinerProfile.IsNoUi && NTMinerRoot.Instance.MinerProfile.IsAutoStart) {
                                ConsoleWindow.Instance.Hide();
                                VirtualRoot.Out.ShowSuccess("已切换为无界面模式运行，可在选项页调整设置", header: "开源矿工");
                            }
                            else {
                                // 预热视图模型
                                AppContext.Instance.VmsCtor();
                                _appViewFactory.ShowMainWindow(isToggle: false);
                            }
                            StartStopMineButtonViewModel.Instance.AutoStart();
                            splashWindow?.Dispatcher.Invoke((Action)delegate () {
                                splashWindow?.Close();
                            });
                            VirtualRoot.StartTimer(new WpfTimer());
                        });
                        Task.Factory.StartNew(() => {
                            if (NTMinerRoot.Instance.MinerProfile.IsAutoDisableWindowsFirewall) {
                                Firewall.DisableFirewall();
                            }
                            if (!Firewall.IsMinerClientRuleExists()) {
                                Firewall.AddMinerClientRule();
                            }
                            try {
                                HttpServer.Start($"http://localhost:{NTKeyword.MinerClientPort.ToString()}");
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
                        DialogWindow.ShowSoftDialog(new DialogWindowViewModel(
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
                    VirtualRoot.RaiseEvent(new ShareChangedEvent(Guid.Empty, coinShare));
                    if ((NTMinerRoot.Instance.LockedMineContext is IDualMineContext dualMineContext) && dualMineContext.DualCoin != null) {
                        coinShare = NTMinerRoot.Instance.CoinShareSet.GetOrCreate(dualMineContext.DualCoin.GetId());
                        VirtualRoot.RaiseEvent(new ShareChangedEvent(Guid.Empty, coinShare));
                    }
                    AppContext.Instance.GpuSpeedVms.Refresh();
                }
            });
        }

        private void Link() {
            VirtualRoot.AddEventPath<StartingMineFailedEvent>("开始挖矿失败", LogEnum.DevConsole,
                action: message => {
                    AppContext.Instance.MinerProfileVm.IsMining = false;
                    VirtualRoot.Out.ShowError(message.Message);
                }, location: this.GetType());
            #region 处理显示主界面命令
            VirtualRoot.AddCmdPath<ShowMainWindowCommand>(action: message => {
                ShowMainWindow(message.IsToggle);
            }, location: this.GetType());
            #endregion
            #region 周期确保守护进程在运行
            VirtualRoot.AddEventPath<Per1MinuteEvent>("周期确保守护进程在运行", LogEnum.DevConsole,
                action: message => {
                    Daemon.DaemonUtil.RunNTMinerDaemon();
                }, location: this.GetType());
            #endregion
            #region 开始和停止挖矿后
            VirtualRoot.AddEventPath<MineStartedEvent>("启动1080ti小药丸、启动DevConsole? 更新挖矿按钮状态", LogEnum.DevConsole,
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
                }, location: this.GetType());
            VirtualRoot.AddEventPath<MineStopedEvent>("停止挖矿后停止1080ti小药丸 挖矿停止后更新界面挖矿状态", LogEnum.DevConsole,
                action: message => {
                    AppContext.Instance.MinerProfileVm.IsMining = false;
                    StartStopMineButtonViewModel.Instance.BtnStopText = "尚未开始";
                    OhGodAnETHlargementPill.OhGodAnETHlargementPillUtil.Stop();
                }, location: this.GetType());
            #endregion
            #region 处理禁用win10系统更新
            VirtualRoot.AddCmdPath<BlockWAUCommand>(action: message => {
                NTMiner.Windows.WindowsUtil.BlockWAU();
            }, location: this.GetType());
            #endregion
            #region 优化windows
            VirtualRoot.AddCmdPath<Win10OptimizeCommand>(action: message => {
                NTMiner.Windows.WindowsUtil.Win10Optimize();
            }, location: this.GetType());
            #endregion
            #region 处理开启A卡计算模式
            VirtualRoot.AddCmdPath<SwitchRadeonGpuCommand>(action: message => {
                if (NTMinerRoot.Instance.GpuSet.GpuType == GpuType.AMD) {
                    SwitchRadeonGpuMode(message.On);
                }
            }, location: this.GetType());
            #endregion
            #region 处理A卡驱动签名
            VirtualRoot.AddCmdPath<AtikmdagPatcherCommand>(action: message => {
                if (NTMinerRoot.Instance.GpuSet.GpuType == GpuType.AMD) {
                    AtikmdagPatcher.AtikmdagPatcherUtil.Run();
                }
            }, location: this.GetType());
            #endregion
            #region 启用或禁用windows远程桌面
            VirtualRoot.AddCmdPath<EnableWindowsRemoteDesktopCommand>(action: message => {
                if (NTMinerRegistry.GetIsRemoteDesktopEnabled()) {
                    return;
                }
                string msg = "确定启用Windows远程桌面吗？";
                DialogWindow.ShowSoftDialog(new DialogWindowViewModel(
                    message: msg,
                    title: "确认",
                    onYes: () => {
                        Rdp.SetRdpEnabled(true);
                        Firewall.AddRdpRule();
                    }));
            }, location: this.GetType());
            #endregion
            #region 启用或禁用windows开机自动登录
            VirtualRoot.AddCmdPath<EnableOrDisableWindowsAutoLoginCommand>(action: message => {
                if (NTMiner.Windows.OS.Instance.IsAutoAdminLogon) {
                    return;
                }
                NTMiner.Windows.Cmd.RunClose("control", "userpasswords2");
            }, location: this.GetType());
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
                    VirtualRoot.Out.ShowError(e.Message, autoHideSeconds: 4);
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
    }
}
