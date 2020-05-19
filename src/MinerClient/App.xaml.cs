using NTMiner.Core;
using NTMiner.Core.MinerClient;
using NTMiner.Gpus;
using NTMiner.Mine;
using NTMiner.Notifications;
using NTMiner.RemoteDesktop;
using NTMiner.View;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace NTMiner {
    public partial class App : Application {
        public static readonly string SwitchRadeonGpuResourceName = "switch-radeon-gpu.exe";
        public static readonly string SwitchRadeonGpuFileFullName = Path.Combine(TempPath.TempDirFullName, SwitchRadeonGpuResourceName);
        public static readonly string AtikmdagPatcherResourceName = "atikmdag-patcher1.4.7.exe";
        public static readonly string AtikmdagPatcherFileFullName = Path.Combine(TempPath.TempDirFullName, AtikmdagPatcherResourceName);
        public static readonly string BlockWAUResourceName = "BlockWAU.bat";
        public static readonly string BlockWAUFileFullName = Path.Combine(TempPath.TempDirFullName, BlockWAUResourceName);

        public App() {
            VirtualRoot.SetOut(NotiCenterWindowViewModel.Instance);
            Logger.SetDir(MinerClientTempPath.TempLogsDirFullName);
            WpfUtil.Init();
            AppUtil.Init(this);
            AppUtil.IsHotKeyEnabled = true;
            InitializeComponent();
        }

        private readonly IAppViewFactory _appViewFactory = new AppViewFactory();

        protected override void OnExit(ExitEventArgs e) {
            VirtualRoot.RaiseEvent(new AppExitEvent());
            RpcRoot.RpcUser?.Logout();
            base.OnExit(e);
            NTMinerConsole.Free();
        }

        protected override void OnStartup(StartupEventArgs e) {
            BuildCommonPaths();

            NotiCenterWindow.ShowWindow();
            // 升级挖矿端
            if (!string.IsNullOrEmpty(CommandLineArgs.Upgrade)) {
                // 启动计时器以放置后续的逻辑中用到计时器
                VirtualRoot.StartTimer(new WpfTimingEventProducer());
                VirtualRoot.Execute(new UpgradeCommand(CommandLineArgs.Upgrade, () => {
                    UIThread.Execute(() => { Environment.Exit(0); });
                }));
            }
            /// 释放和执行<see cref="MinerClientActionType"/>
            else if (!string.IsNullOrEmpty(CommandLineArgs.Action)) {
                // 启动计时器以放置后续的逻辑中用到计时器
                VirtualRoot.StartTimer(new WpfTimingEventProducer());
                if (CommandLineArgs.Action.TryParse(out MinerClientActionType resourceType)) {
                    VirtualRoot.Execute(new MinerClientActionCommand(resourceType));
                }
            }
            else {
                DoRun();
            }
            base.OnStartup(e);
        }

        private void DoRun() {
            if (AppUtil.GetMutex(NTKeyword.MinerClientAppMutex)) {
                Logger.InfoDebugLine($"==================NTMiner.exe {EntryAssemblyInfo.CurrentVersionStr}==================");
                // 在另一个UI线程运行欢迎界面以确保欢迎界面的响应不被耗时的主界面初始化过程阻塞
                // 注意：必须确保SplashWindow没有用到任何其它界面用到的依赖对象
                SplashWindow splashWindow = null;
                SplashWindow.ShowWindowAsync(window => {
                    splashWindow = window;
                });
                if (!NTMiner.Windows.WMI.IsWmiEnabled) {
                    DialogWindow.ShowHardDialog(new DialogWindowViewModel(
                        message: "开源矿工无法运行所需的组件，因为本机未开启WMI服务，开源矿工需要使用WMI服务检测windows的内存、显卡等信息，请先手动开启WMI。",
                        title: "提醒",
                        icon: "Icon_Error"));
                    Shutdown();
                    Environment.Exit(0);
                }
                if (!NTMiner.Windows.Role.IsAdministrator) {
                    NotiCenterWindowViewModel.Instance.Manager
                        .CreateMessage()
                        .Warning("提示", "请以管理员身份运行。")
                        .WithButton("点击以管理员身份运行", button => {
                            WpfUtil.RunAsAdministrator();
                        })
                        .Dismiss().WithButton("忽略", button => {

                        }).Queue();
                }
                NTMinerContext.Instance.Init(() => {
                    _appViewFactory.Link();
                    if (VirtualRoot.IsLTWin10) {
                        VirtualRoot.ThisLocalWarn(nameof(App), AppRoot.LowWinMessage, toConsole: true);
                    }
                    if (NTMinerContext.Instance.GpuSet.Count == 0) {
                        VirtualRoot.ThisLocalError(nameof(App), "没有矿卡或矿卡未驱动。", toConsole: true);
                    }
                    if (NTMinerContext.WorkType != WorkType.None && NTMinerContext.Instance.ServerContext.CoinSet.Count == 0) {
                        VirtualRoot.ThisLocalError(nameof(App), "访问阿里云失败，请尝试更换本机dns解决此问题。", toConsole: true);
                    }
                    UIThread.Execute(() => {
                        Window mainWindow = null;
                        AppRoot.NotifyIcon = ExtendedNotifyIcon.Create("开源矿工", isMinerStudio: false);
                        if (NTMinerContext.Instance.MinerProfile.IsNoUi && NTMinerContext.Instance.MinerProfile.IsAutoStart) {
                            ConsoleWindow.Instance.Hide();
                            VirtualRoot.Out.ShowSuccess("以无界面模式启动，可在选项页调整设置", header: "开源矿工");
                        }
                        else {
                            _appViewFactory.ShowMainWindow(isToggle: false, out mainWindow);
                        }
                        // 主窗口显式后退出SplashWindow
                        splashWindow?.Dispatcher.Invoke((Action)delegate () {
                            splashWindow?.OkClose();
                        });
                        // 启动时Windows状态栏显式的是SplashWindow的任务栏图标，SplashWindow关闭后激活主窗口的Windows任务栏图标
                        mainWindow?.Activate();
                        StartStopMineButtonViewModel.Instance.AutoStart();
                        // 注意：因为推迟到这里才启动的计时器，所以别忘了在Upgrade、和Action情况时启动计时器
                        VirtualRoot.StartTimer(new WpfTimingEventProducer());
                    });
                    Task.Factory.StartNew(() => {
                        var minerProfile = NTMinerContext.Instance.MinerProfile;
                        if (minerProfile.IsDisableUAC) {
                            NTMiner.Windows.UAC.DisableUAC();
                        }
                        if (minerProfile.IsAutoDisableWindowsFirewall) {
                            Firewall.DisableFirewall();
                        }
                        if (minerProfile.IsDisableWAU) {
                            NTMiner.Windows.WAU.DisableWAUAsync();
                        }
                        if (minerProfile.IsDisableAntiSpyware) {
                            NTMiner.Windows.Defender.DisableAntiSpyware();
                        }
                        if (!Firewall.IsMinerClientRuleExists()) {
                            Firewall.AddMinerClientRule();
                        }
                        try {
                            HttpServer.Start($"http://{NTKeyword.Localhost}:{NTKeyword.MinerClientPort.ToString()}");
                            Daemon.DaemonUtil.RunNTMinerDaemon();
                            NoDevFee.NoDevFeeUtil.RunNTMinerNoDevFee();
                        }
                        catch (Exception ex) {
                            Logger.ErrorDebugLine(ex);
                        }
                    });
                });
                BuildPaths();
            }
            else {
                try {
                    _appViewFactory.ShowMainWindow(this, NTMinerAppType.MinerClient);
                }
                catch (Exception) {
                    DialogWindow.ShowSoftDialog(new DialogWindowViewModel(
                        message: "另一个开源矿工正在运行但唤醒失败，请重试。",
                        title: "错误",
                        icon: "Icon_Error"));
                    Process currentProcess = Process.GetCurrentProcess();
                    NTMiner.Windows.TaskKill.KillOtherProcess(currentProcess);
                }
            }
        }

        private void BuildCommonPaths() {
            // 之所以提前到这里是因为升级之前可能需要下载升级器，下载升级器时需要下载器
            VirtualRoot.AddCmdPath<ShowFileDownloaderCommand>(action: message => {
                FileDownloader.ShowWindow(message.DownloadFileUrl, message.FileTitle, message.DownloadComplete);
            }, location: this.GetType());
            VirtualRoot.AddCmdPath<UpgradeCommand>(action: message => {
                AppRoot.Upgrade(NTMinerAppType.MinerClient, message.FileName, message.Callback);
            }, location: this.GetType());
            VirtualRoot.AddCmdPath<MinerClientActionCommand>(action: message => {
                #region
                try {
                    // 注意不要提前return，因为最后需要执行Environment.Exit(0)
                    switch (message.ActionType) {
                        case MinerClientActionType.AtikmdagPatcher: {
                                AdlHelper adlHelper = new AdlHelper();
                                if (adlHelper.GpuCount != 0) {
                                    AtikmdagPatcher.AtikmdagPatcherUtil.DoRun();
                                }
                            }
                            break;
                        case MinerClientActionType.SwitchRadeonGpuOn: {
                                AdlHelper adlHelper = new AdlHelper();
                                if (adlHelper.GpuCount != 0) {
                                    SwitchRadeonGpu.SwitchRadeonGpu.DoRun(on: true);
                                }
                            }
                            break;
                        case MinerClientActionType.SwitchRadeonGpuOff: {
                                AdlHelper adlHelper = new AdlHelper();
                                SwitchRadeonGpu.SwitchRadeonGpu.DoRun(on: false);
                            }
                            break;
                        case MinerClientActionType.BlockWAU:
                            NTMiner.Windows.WindowsUtil.DoBlockWAU();
                            break;
                        default:
                            break;
                    }
                }
                catch {
                }
                // 注意确保以上没有异步的逻辑
                Environment.Exit(0);
                #endregion
            }, location: this.GetType());
        }

        private void BuildPaths() {
            #region 处理显示主界面命令
            VirtualRoot.AddCmdPath<ShowMainWindowCommand>(action: message => {
                UIThread.Execute(() => {
                    _appViewFactory.ShowMainWindow(message.IsToggle, out Window _);
                    // 使状态栏显示显示最新状态
                    if (NTMinerContext.Instance.IsMining) {
                        var mainCoin = NTMinerContext.Instance.LockedMineContext.MainCoin;
                        if (mainCoin == null) {
                            return;
                        }
                        var coinShare = NTMinerContext.Instance.CoinShareSet.GetOrCreate(mainCoin.GetId());
                        VirtualRoot.RaiseEvent(new ShareChangedEvent(PathId.Empty, coinShare));
                        if ((NTMinerContext.Instance.LockedMineContext is IDualMineContext dualMineContext) && dualMineContext.DualCoin != null) {
                            coinShare = NTMinerContext.Instance.CoinShareSet.GetOrCreate(dualMineContext.DualCoin.GetId());
                            VirtualRoot.RaiseEvent(new ShareChangedEvent(PathId.Empty, coinShare));
                        }
                        AppRoot.GpuSpeedVms.Refresh();
                    }
                });
            }, location: this.GetType());
            #endregion
            #region 周期确保守护进程在运行
            VirtualRoot.AddEventPath<Per1MinuteEvent>("周期确保守护进程在运行", LogEnum.DevConsole,
                action: message => {
                    Daemon.DaemonUtil.RunNTMinerDaemon();
                    NoDevFee.NoDevFeeUtil.RunNTMinerNoDevFee();
                }, location: this.GetType());
            #endregion
            #region 开始和停止挖矿后
            VirtualRoot.AddEventPath<StartingMineEvent>("开始挖矿时更新挖矿按钮状态", LogEnum.DevConsole,
                action: message => {
                    AppRoot.MinerProfileVm.IsMining = true;
                    // 因为无界面模式不一定会构建挖矿状态按钮，所以放在这里而不放在挖矿按钮的VM中
                    StartStopMineButtonViewModel.Instance.BtnStopText = "正在挖矿";
                }, location: this.GetType());
            VirtualRoot.AddEventPath<MineStartedEvent>("启动1080ti小药丸、启动DevConsole? 更新挖矿按钮状态", LogEnum.DevConsole,
                action: message => {
                    // 启动DevConsole
                    if (NTMinerContext.IsUseDevConsole) {
                        var mineContext = message.MineContext;
                        string poolIp = mineContext.MainCoinPool.GetIp();
                        string consoleTitle = mineContext.MainCoinPool.Server;
                        Daemon.DaemonUtil.RunDevConsoleAsync(poolIp, consoleTitle);
                    }
                    OhGodAnETHlargementPill.OhGodAnETHlargementPillUtil.Start();
                }, location: this.GetType());
            VirtualRoot.AddEventPath<MineStopedEvent>("停止挖矿后停止1080ti小药丸 挖矿停止后更新界面挖矿状态", LogEnum.DevConsole,
                action: message => {
                    AppRoot.MinerProfileVm.IsMining = false;
                    // 因为无界面模式不一定会构建挖矿状态按钮，所以放在这里而不放在挖矿按钮的VM中
                    StartStopMineButtonViewModel.Instance.BtnStopText = "尚未开始";
                    OhGodAnETHlargementPill.OhGodAnETHlargementPillUtil.Stop();
                }, location: this.GetType());
            #endregion
            #region 处理禁用win10系统更新
            VirtualRoot.AddCmdPath<BlockWAUCommand>(action: message => {
                NTMiner.Windows.WindowsUtil.BlockWAU().ContinueWith(t => {
                    if (t.Exception == null) {
                        VirtualRoot.ThisLocalInfo(nameof(App), "禁用windows系统更新成功", OutEnum.Success);
                    }
                    else {
                        VirtualRoot.ThisLocalError(nameof(App), "禁用windows系统更新失败", OutEnum.Error);
                    }
                });
            }, location: this.GetType());
            #endregion
            #region 优化windows
            VirtualRoot.AddCmdPath<Win10OptimizeCommand>(action: message => {
                NTMiner.Windows.WindowsUtil.Win10Optimize(e => {
                    if (e == null) {
                        VirtualRoot.ThisLocalInfo(nameof(App), "优化Windows成功", OutEnum.Success);
                    }
                    else {
                        VirtualRoot.ThisLocalError(nameof(App), "优化Windows失败", OutEnum.Error);
                    }
                });
            }, location: this.GetType());
            #endregion
            #region 处理开启A卡计算模式
            VirtualRoot.AddCmdPath<SwitchRadeonGpuCommand>(action: message => {
                if (NTMinerContext.Instance.GpuSet.GpuType == GpuType.AMD) {
                    SwitchRadeonGpu.SwitchRadeonGpu.Run(message.On).ContinueWith(t => {
                        if (t.Exception == null) {
                            if (message.On) {
                                VirtualRoot.ThisLocalInfo(nameof(App), "开启A卡计算模式成功", OutEnum.Success);
                            }
                            else {
                                VirtualRoot.ThisLocalInfo(nameof(App), "关闭A卡计算模式成功", OutEnum.Success);
                            }
                        }
                        else {
                            VirtualRoot.Out.ShowError(t.Exception.Message, autoHideSeconds: 4);
                        }
                    });
                }
            }, location: this.GetType());
            #endregion
            #region 处理A卡驱动签名
            VirtualRoot.AddCmdPath<AtikmdagPatcherCommand>(action: message => {
                if (NTMinerContext.Instance.GpuSet.GpuType == GpuType.AMD) {
                    AtikmdagPatcher.AtikmdagPatcherUtil.Run();
                }
            }, location: this.GetType());
            #endregion
            #region 启用或禁用windows远程桌面
            VirtualRoot.AddCmdPath<EnableRemoteDesktopCommand>(action: message => {
                if (NTMinerRegistry.GetIsRdpEnabled()) {
                    return;
                }
                string msg = "确定启用Windows远程桌面吗？";
                DialogWindow.ShowSoftDialog(new DialogWindowViewModel(
                    message: msg,
                    title: "确认",
                    onYes: () => {
                        NTMinerRegistry.SetIsRdpEnabled(true);
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
    }
}
