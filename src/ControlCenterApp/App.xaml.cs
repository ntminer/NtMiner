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
                NTMinerRoot.Current.Init(() => {
                    NTMinerRoot.KernelDownloader = new KernelDownloader();
                    Execute.OnUIThread(() => {
                        bool? result = true;
                        if (string.IsNullOrEmpty(Server.LoginName) || string.IsNullOrEmpty(Server.PasswordSha1)) {
                            LoginWindow loginWindow = new LoginWindow();
                            splashWindow.Hide();
                            result = loginWindow.ShowDialog();
                        }
                        if (result.HasValue && result.Value) {
                            Logger.InfoDebugLine("new MainWindow");
                            ControlCenterWindow window = new ControlCenterWindow();
                            IMainWindow mainWindow = window;
                            this.MainWindow = window;
                            this.MainWindow.Show();
                            this.MainWindow.Activate();
                            Logger.InfoDebugLine("MainWindow showed");
                            notifyIcon = new ExtendedNotifyIcon("pack://application:,,,/ControlCenterApp;component/logo.ico");
                            notifyIcon.Init();
                            #region 处理显示主界面命令
                            VirtualRoot.Access<ShowMainWindowCommand>(
                                Guid.Parse("01f3c467-f494-42b8-bcb5-848050df59f3"),
                                "处理显示主界面命令",
                                LogEnum.None,
                                action: message => {
                                    Execute.OnUIThread(() => {
                                        Dispatcher.Invoke((ThreadStart)mainWindow.ShowThisWindow);
                                    });
                                });
                            #endregion
                        }
                        splashWindow?.Close();
                    });
                });
            }
            else {
                try {
                    AppHelper.ShowMainWindow(this, _appPipName);
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
