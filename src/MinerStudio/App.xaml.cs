using NTMiner.MinerStudio;
using NTMiner.View;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using NTMiner.Ws;
using System;
using System.Diagnostics;
using System.Windows;

namespace NTMiner {
    public partial class App : Application {
        public App() {
            HomePath.SetHomeDirFullName(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NTMiner"));
            VirtualRoot.SetOut(NotiCenterWindowViewModel.Instance);
            Logger.SetDir(HomePath.HomeLogsDirFullName);
            WpfUtil.Init();
            AppUtil.Init(this);
            InitializeComponent();
        }

        private readonly IAppViewFactory _appViewFactory = new AppViewFactory();

        protected override void OnExit(ExitEventArgs e) {
            VirtualRoot.RaiseEvent(new AppExitEvent());
            base.OnExit(e);
            NTMinerConsole.Free();
        }

        protected override void OnStartup(StartupEventArgs e) {
            // 之所以提前到这里是因为升级之前可能需要下载升级器，下载升级器时需要下载器
            VirtualRoot.AddCmdPath<ShowFileDownloaderCommand>(action: message => {
                FileDownloader.ShowWindow(message.DownloadFileUrl, message.FileTitle, message.DownloadComplete);
            }, location: this.GetType());
            VirtualRoot.AddCmdPath<UpgradeCommand>(action: message => {
                AppRoot.Upgrade(NTMinerAppType.MinerStudio, message.FileName, message.Callback);
            }, location: this.GetType());
            if (AppUtil.GetMutex(NTKeyword.MinerStudioAppMutex)) {
                this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                // 因为登录窗口会用到VirtualRoot.Out，而Out的延迟自动关闭消息会用到倒计时
                VirtualRoot.StartTimer(new WpfTimingEventProducer());
                NotiCenterWindow.ShowWindow();
                AppRoot.RemoteDesktop = MsRdpRemoteDesktop.OpenRemoteDesktop;
                LoginWindow.Login(() => {
                    IWsClient wsClient = null;
                    if (RpcRoot.IsOuterNet) {
                        wsClient = new MinerStudioWsClient();
                    }
                    MinerStudioRoot.Init(wsClient);
                    NTMinerContext.Instance.Init(() => {
                        _appViewFactory.Link();
                        UIThread.Execute(() => () => {
                            MinerStudioRoot.MinerClientsWindowVm.OnPropertyChanged(nameof(MinerStudioRoot.MinerClientsWindowVm.NetTypeText));
                            if (RpcRoot.IsOuterNet) {
                                MinerStudioRoot.MinerClientsWindowVm.QueryMinerClients();
                            }
                            else {
                                VirtualRoot.AddOnecePath<ClientSetInitedEvent>("矿工集合初始化完成后刷新矿机列表界面", LogEnum.DevConsole, action: message => {
                                    MinerStudioRoot.MinerClientsWindowVm.QueryMinerClients();
                                }, pathId: PathId.Empty, this.GetType());
                            }
                            AppRoot.NotifyIcon = ExtendedNotifyIcon.Create("群控客户端", isMinerStudio: true);
                            VirtualRoot.Execute(new ShowMinerClientsWindowCommand(isToggle: false));
                        });
                        #region 处理显示主界面命令
                        VirtualRoot.AddCmdPath<ShowMainWindowCommand>(action: message => {
                            VirtualRoot.Execute(new ShowMinerClientsWindowCommand(isToggle: true));
                        }, location: this.GetType());
                        #endregion
                        HttpServer.Start($"http://localhost:{NTKeyword.MinerStudioPort.ToString()}");
                    });
                }, btnCloseClick: () => {
                    Shutdown();
                });
            }
            else {
                try {
                    _appViewFactory.ShowMainWindow(this, NTMinerAppType.MinerStudio);
                }
                catch (Exception) {
                    DialogWindow.ShowSoftDialog(new DialogWindowViewModel(
                        message: "另一个群控客户端正在运行但唤醒失败，请重试。",
                        title: "错误",
                        icon: "Icon_Error"));
                    Process currentProcess = Process.GetCurrentProcess();
                    NTMiner.Windows.TaskKill.KillOtherProcess(currentProcess);
                }
            }
            base.OnStartup(e);
        }
    }
}
