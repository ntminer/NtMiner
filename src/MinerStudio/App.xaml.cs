using NTMiner.Views;
using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace NTMiner {
    public partial class App : Application, IDisposable {
        public App() {
            VirtualRoot.SetIsMinerStudio(true);
            VirtualRoot.GlobalDirFullName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NTMiner");
            Logging.LogDir.SetDir(System.IO.Path.Combine(VirtualRoot.GlobalDirFullName, "Logs"));
            AppHelper.Init(this);
            InitializeComponent();
        }

        private bool createdNew;
        private Mutex appMutex;
        private static string s_appPipName = "ntminercontrol";

        protected override void OnExit(ExitEventArgs e) {
            AppContext.NotifyIcon?.Dispose();
            NTMinerRoot.Instance.Exit();
            HttpServer.Stop();
            if (NTMinerRegistry.GetIsAutoCloseServices()) {
                Server.ControlCenterService.CloseServices();
            }
            base.OnExit(e);
            ConsoleManager.Hide();
        }

        protected override void OnStartup(StartupEventArgs e) {
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            try {
                appMutex = new Mutex(true, s_appPipName, out createdNew);
            }
            catch (Exception){
                createdNew = false;
            }

            if (createdNew) {
                NTMinerRoot.SetIsMinerClient(false);
                SplashWindow splashWindow = new SplashWindow();
                splashWindow.Show();
                NotiCenterWindow.Instance.Show();
                bool isInnerIp = Ip.Util.IsInnerIp(NTMinerRegistry.GetControlCenterHost());
                if (isInnerIp) {
                    NTMinerServices.NTMinerServicesUtil.RunNTMinerServices(()=> {
                        Init(splashWindow);
                    });
                }
                else {
                    Init(splashWindow);
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
                                Logger.ErrorDebugLine(ex.Message, ex);
                                Environment.Exit(0);
                            }
                        });
                    });
            }
            else {
                try {
                    WindowFactory.ShowMainWindow(this, MinerServer.NTMinerAppType.MinerStudio);
                }
                catch (Exception) {
                    DialogWindow.ShowDialog(message: "另一个NTMiner正在运行，请手动结束正在运行的NTMiner进程后再次尝试。", title: "alert", icon: "Icon_Error");
                    Process currentProcess = Process.GetCurrentProcess();
                    NTMiner.Windows.TaskKill.KillOtherProcess(currentProcess);
                }
            }
            base.OnStartup(e);
        }

        private void Init(SplashWindow splashWindow) {
            NTMinerRoot.Instance.Init(() => {
                WindowFactory.Link();
                UIThread.Execute(() => {
                    splashWindow?.Close();
                    LoginWindow loginWindow = new LoginWindow();
                    var result = loginWindow.ShowDialog();
                    if (result.HasValue && result.Value) {
                        ChartsWindow.ShowWindow();
                        AppContext.NotifyIcon = ExtendedNotifyIcon.Create("群控客户端", isMinerStudio: true);
                        #region 处理显示主界面命令
                        VirtualRoot.Window<ShowMainWindowCommand>("处理显示主界面命令", LogEnum.None,
                            action: message => {
                                Dispatcher.Invoke((ThreadStart)ChartsWindow.ShowWindow);
                            });
                        #endregion
                        HttpServer.Start($"http://localhost:{Consts.MinerStudioPort}");
                        AppContext.RemoteDesktop = MsRdpRemoteDesktop.OpenRemoteDesktop;
                    }
                });
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
