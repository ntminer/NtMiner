using NTMiner.Views;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace NTMiner {
    public partial class App : Application {
        public App() {
            VirtualRoot.IsControlCenter = true;
            AppHelper.Init(this);
            Logger.InfoDebugLine("App.InitializeComponent start");
            InitializeComponent();
            Logger.InfoDebugLine("App.InitializeComponent end");
        }

        private bool createdNew;
        private Mutex appMutex;
        private static string _appPipName = "ntminercontrol";
        ExtendedNotifyIcon notifyIcon;

        protected override void OnExit(ExitEventArgs e) {
            notifyIcon?.Dispose();
            NTMinerRoot.Current.Exit();
            HttpServer.Stop();
            base.OnExit(e);
        }

        protected override void OnStartup(StartupEventArgs e) {
            Logger.InfoDebugLine("App.OnStartup start");
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            try {
                appMutex = new Mutex(true, _appPipName, out createdNew);
            }
            catch (Exception){
                createdNew = false;
            }
            if (createdNew) {
                Vms.AppStatic.IsMinerClient = false;
                Logger.InfoDebugLine("new SplashWindow");
                SplashWindow splashWindow = new SplashWindow();
                splashWindow.Show();
                NTMinerRoot.AppName = "开源矿工中控客户端";
                NTMinerRoot.Current.Init(() => {
                    NTMinerRoot.KernelDownloader = new KernelDownloader();
                    UIThread.Execute(() => {
                        bool? result = true;
                        if (string.IsNullOrEmpty(Server.LoginName) || string.IsNullOrEmpty(Server.PasswordSha1)) {
                            LoginWindow loginWindow = new LoginWindow();
                            splashWindow.Hide();
                            result = loginWindow.ShowDialog();
                        }
                        if (result.HasValue && result.Value) {
                            Logger.InfoDebugLine("new MainWindow");
                            ChartsWindow window = new ChartsWindow();
                            IMainWindow mainWindow = window;
                            this.MainWindow = window;
                            this.MainWindow.Show();
                            this.MainWindow.Activate();
                            Logger.InfoDebugLine("MainWindow showed");
                            notifyIcon = new ExtendedNotifyIcon("pack://application:,,,/ControlCenterApp;component/logo.ico");
                            notifyIcon.Init();
                            #region 处理显示主界面命令
                            VirtualRoot.Accept<ShowMainWindowCommand>(
                                "处理显示主界面命令",
                                LogEnum.None,
                                action: message => {
                                    UIThread.Execute(() => {
                                        Dispatcher.Invoke((ThreadStart)mainWindow.ShowThisWindow);
                                    });
                                });
                            #endregion
                        }
                        HttpServer.Start("http://localhost:3338");
                        splashWindow?.Close();
                        AppHelper.RemoteDesktop = MsRdpRemoteDesktop.OpenRemoteDesktop;
                    });
                });
            }
            else {
                try {
                    AppHelper.ShowMainWindow(this, 3338);
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
            Logger.InfoDebugLine("App.OnStartup end");
        }
    }
}
