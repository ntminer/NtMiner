using NTMiner.View;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace NTMiner {
    public partial class App : Application, IDisposable {
        public App() {
            if (!Debugger.IsAttached && !Design.IsInDesignMode) {
                Write.Init();
            }
            VirtualRoot.SetIsMinerStudio(true);
            AssemblyInfo.LocalDirFullName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NTMiner");
            Logging.LogDir.SetDir(System.IO.Path.Combine(AssemblyInfo.LocalDirFullName, "Logs"));
            AppUtil.Init(this);
            InitializeComponent();
        }

        private readonly IAppViewFactory _appViewFactory = new AppViewFactory();

        private bool createdNew;
        private Mutex appMutex;
        private static string s_appPipName = "ntminercontrol";

        protected override void OnExit(ExitEventArgs e) {
            AppContext.NotifyIcon?.Dispose();
            NTMinerRoot.Instance.Exit();
            HttpServer.Stop();
            if (createdNew) {
                Server.ControlCenterService.CloseServices();
            }
            base.OnExit(e);
            NTMinerConsole.Hide();
        }

        protected override void OnStartup(StartupEventArgs e) {
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            VirtualRoot.Window<ShowFileDownloaderCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        FileDownloader.ShowWindow(message.DownloadFileUrl, message.FileTitle, message.DownloadComplete);
                    });
                });
            VirtualRoot.Window<UpgradeCommand>(LogEnum.DevConsole,
                action: message => {
                    AppStatic.Upgrade(message.FileName, message.Callback);
                });
            try {
                appMutex = new Mutex(true, s_appPipName, out createdNew);
            }
            catch (Exception){
                createdNew = false;
            }

            if (createdNew) {
                this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                VirtualRoot.SetIsMinerClient(false);
                NotiCenterWindow.Instance.Show();
                LoginWindow loginWindow = new LoginWindow();
                var result = loginWindow.ShowDialog();
                if (result.HasValue && result.Value) {
                    bool isInnerIp = Ip.Util.IsInnerIp(NTMinerRegistry.GetControlCenterHost());
                    if (isInnerIp) {
                        NTMinerServices.NTMinerServicesUtil.RunNTMinerServices(() => {
                            Init();
                        });
                    }
                    else {
                        Init();
                    }
                }
                VirtualRoot.Window<CloseNTMinerCommand>("处理关闭群控客户端命令", LogEnum.UserConsole,
                    action: message => {
                        UIThread.Execute(() => {
                            try {
                                if (MainWindow != null) {
                                    MainWindow.Close();
                                }
                                Shutdown();
                            }
                            catch (Exception ex) {
                                Logger.ErrorDebugLine(ex);
                                Environment.Exit(0);
                            }
                        });
                    });
            }
            else {
                try {
                    _appViewFactory.ShowMainWindow(this, MinerServer.NTMinerAppType.MinerStudio);
                }
                catch (Exception) {
                    DialogWindow.ShowDialog(message: "另一个NTMiner正在运行，请手动结束正在运行的NTMiner进程后再次尝试。", title: "alert", icon: "Icon_Error");
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
                    VirtualRoot.Execute(new ShowChartsWindowCommand());
                    AppContext.NotifyIcon = ExtendedNotifyIcon.Create("群控客户端", isMinerStudio: true);
                });
                #region 处理显示主界面命令
                VirtualRoot.Window<ShowMainWindowCommand>("处理显示主界面命令", LogEnum.DevConsole,
                    action: message => {
                        VirtualRoot.Execute(new ShowChartsWindowCommand());
                    });
                #endregion
                HttpServer.Start($"http://localhost:{Consts.MinerStudioPort}");
                AppContext.RemoteDesktop = MsRdpRemoteDesktop.OpenRemoteDesktop;
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
