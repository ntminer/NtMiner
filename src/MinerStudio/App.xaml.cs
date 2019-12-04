using NTMiner.RemoteDesktop;
using NTMiner.View;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace NTMiner {
    public partial class App : Application, IDisposable {
        public App() {
            MainAssemblyInfo.SetHomeDirFullName(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NTMiner"));
            VirtualRoot.SetOut(NotiCenterWindowViewModel.Instance);
            Logger.SetDir(SpecialPath.LogsDirFullName);
            Write.UIThreadId = Dispatcher.Thread.ManagedThreadId;
            AppUtil.Init(this);
            InitializeComponent();
        }

        private readonly IAppViewFactory _appViewFactory = new AppViewFactory();

        private bool createdNew;
        private Mutex appMutex;
        private static readonly string s_appPipName = "ntminercontrol";

        protected override void OnExit(ExitEventArgs e) {
            AppContext.NotifyIcon?.Dispose();
            NTMinerRoot.Instance.Exit();
            HttpServer.Stop();
            if (createdNew) {
                Server.ControlCenterService.CloseServices();
            }
            base.OnExit(e);
            NTMinerConsole.Free();
        }

        protected override void OnStartup(StartupEventArgs e) {
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            VirtualRoot.AddCmdPath<ShowFileDownloaderCommand>(action: message => {
                UIThread.Execute(() => {
                    FileDownloader.ShowWindow(message.DownloadFileUrl, message.FileTitle, message.DownloadComplete);
                });
            }, location: this.GetType());
            VirtualRoot.AddCmdPath<UpgradeCommand>(action: message => {
                AppStatic.Upgrade(message.FileName, message.Callback);
            }, location: this.GetType());
            try {
                appMutex = new Mutex(true, s_appPipName, out createdNew);
            }
            catch (Exception) {
                createdNew = false;
            }

            if (createdNew) {
                this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                NotiCenterWindow.ShowWindow();
                LoginWindow.Login(() => {
                    bool isInnerIp = Net.Util.IsInnerIp(NTMinerRegistry.GetControlCenterHost());
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
                    _appViewFactory.ShowMainWindow(this, MinerServer.NTMinerAppType.MinerStudio);
                }
                catch (Exception) {
                    DialogWindow.ShowSoftDialog(new DialogWindowViewModel(
                        message: "另一个NTMiner正在运行，请手动结束正在运行的NTMiner进程后再次尝试。",
                        title: "alert",
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
                UIThread.Execute(() => {
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
