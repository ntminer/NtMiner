using NTMiner.Views;
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
            VirtualRoot.IsMinerStudio = true;
            VirtualRoot.GlobalDirFullName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NTMiner");
            Logging.LogDir.SetDir(System.IO.Path.Combine(VirtualRoot.GlobalDirFullName, "Logs"));
            AppHelper.Init(this);
            InitializeComponent();
        }

        private bool createdNew;
        private Mutex appMutex;
        private static string s_appPipName = "ntminercontrol";

        protected override void OnExit(ExitEventArgs e) {
            AppHelper.NotifyIcon?.Dispose();
            NTMinerRoot.Current.Exit();
            HttpServer.Stop();
            if (NTMinerRegistry.GetIsAutoCloseServices()) {
                Server.ControlCenterService.CloseServices();
            }
            base.OnExit(e);
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
                Vms.AppStatic.IsMinerClient = false;
                SplashWindow splashWindow = new SplashWindow();
                splashWindow.Show();
                NotiCenterWindow.Instance.Show();
                NTMinerRoot.AppName = "开源矿工群控客户端";
                NTMinerServices.NTMinerServicesUtil.RunNTMinerServices();
                NTMinerRoot.Current.Init(() => {
                    NTMinerRoot.KernelDownloader = new KernelDownloader();
                    UIThread.Execute(() => {
                        splashWindow?.Close();
                        bool? result = true;
                        if (Ip.Util.IsInnerIp(NTMinerRegistry.GetControlCenterHost())) {
                            SingleUser.LoginName = "innerip";
                            SingleUser.SetPasswordSha1("123");
                            result = true;
                        }
                        else if (string.IsNullOrEmpty(SingleUser.LoginName) || string.IsNullOrEmpty(SingleUser.PasswordSha1)) {
                            LoginWindow loginWindow = new LoginWindow();
                            result = loginWindow.ShowDialog();
                        }
                        if (result.HasValue && result.Value) {
                            ChartsWindow.ShowWindow();
                            System.Drawing.Icon icon = new System.Drawing.Icon(GetResourceStream(new Uri("pack://application:,,,/MinerStudio;component/logo.ico")).Stream);
                            AppHelper.NotifyIcon = ExtendedNotifyIcon.Create(icon, "群控客户端", isMinerStudio: true);
                            #region 处理显示主界面命令
                            VirtualRoot.Window<ShowMainWindowCommand>("处理显示主界面命令", LogEnum.None,
                                action: message => {
                                    Dispatcher.Invoke((ThreadStart)ChartsWindow.ShowWindow);
                                });
                            #endregion
                            HttpServer.Start($"http://localhost:{WebApiConst.MinerStudioPort}");
                            AppHelper.RemoteDesktop = MsRdpRemoteDesktop.OpenRemoteDesktop;
                        }
                    });
                });
                VirtualRoot.Window<CloseNTMinerCommand>("处理关闭群控客户端命令", LogEnum.UserConsole,
                    action: message => {
                        UIThread.Execute(() => {
                            if (MainWindow != null) {
                                MainWindow.Close();
                            }
                            Shutdown();
                            Environment.Exit(0);
                        });
                    });
            }
            else {
                try {
                    AppHelper.ShowMainWindow(this, MinerServer.NTMinerAppType.MinerStudio);
                }
                catch (Exception) {
                    DialogWindow.ShowDialog(message: "另一个NTMiner正在运行，请手动结束正在运行的NTMiner进程后再次尝试。", title: "alert", icon: "Icon_Error");
                    Process currentProcess = Process.GetCurrentProcess();
                    Process[] processes = Process.GetProcessesByName(currentProcess.ProcessName);
                    foreach (var process in processes) {
                        if (process.Id != currentProcess.Id) {
                            NTMiner.Windows.TaskKill.Kill(process.Id);
                        }
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
