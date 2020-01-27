using NTMiner.Core;
using NTMiner.RemoteDesktop;
using NTMiner.View;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace NTMiner {
    public partial class App : Application {
        public App() {
            EntryAssemblyInfo.SetHomeDirFullName(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NTMiner"));
            VirtualRoot.SetOut(NotiCenterWindowViewModel.Instance);
            Logger.SetDir(SpecialPath.HomeLogsDirFullName);
            AppUtil.Init(this);
            InitializeComponent();
        }

        private readonly IAppViewFactory _appViewFactory = new AppViewFactory();

        private bool createdNew;
        protected override void OnExit(ExitEventArgs e) {
            VirtualRoot.RaiseEvent(new AppExitEvent());
            if (createdNew) {
                RpcRoot.Server.ControlCenterService.CloseServices();
            }
            base.OnExit(e);
            NTMinerConsole.Free();
        }

        protected override void OnStartup(StartupEventArgs e) {
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            // 之所以提前到这里是因为升级之前可能需要下载升级器，下载升级器时需要下载器
            VirtualRoot.AddCmdPath<ShowFileDownloaderCommand>(action: message => {
                FileDownloader.ShowWindow(message.DownloadFileUrl, message.FileTitle, message.DownloadComplete);
            }, location: this.GetType());
            VirtualRoot.AddCmdPath<UpgradeCommand>(action: message => {
                AppStatic.Upgrade(message.FileName, message.Callback);
            }, location: this.GetType());
            createdNew = AppUtil.GetMutex(NTKeyword.MinerStudioAppMutex);
            if (createdNew) {
                this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                // 因为登录窗口会用到VirtualRoot.Out，而Out的延迟自动关闭消息会用到倒计时
                VirtualRoot.StartTimer(new WpfTimer());
                NotiCenterWindow.Instance.ShowWindow();
                LoginWindow.Login(() => {
                    bool isInnerIp = Net.IpUtil.IsInnerIp(NTMinerRegistry.GetControlCenterHost());
                    if (isInnerIp) {
                        NTMinerServices.NTMinerServicesUtil.RunNTMinerServices(() => {
                            Init();
                        });
                    }
                    else {
                        Init();
                    }
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

        private void Init() {
            NTMinerRoot.Instance.Init(() => {
                _appViewFactory.Link();
                UIThread.Execute(() => () => {
                    VirtualRoot.Execute(new ShowMinerClientsWindowCommand());
                    AppContext.NotifyIcon = ExtendedNotifyIcon.Create("群控客户端", isMinerStudio: true);
                });
                #region 处理显示主界面命令
                VirtualRoot.AddCmdPath<ShowMainWindowCommand>(action: message => {
                    VirtualRoot.Execute(new ShowMinerClientsWindowCommand());
                }, location: this.GetType());
                #endregion
                HttpServer.Start($"http://localhost:{NTKeyword.MinerStudioPort.ToString()}");
                Rdp.RemoteDesktop = MsRdpRemoteDesktop.OpenRemoteDesktop;
            });
        }
    }
}
