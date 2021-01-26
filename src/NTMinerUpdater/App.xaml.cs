using NTMiner.Views;
using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.Windows;

namespace NTMiner {
    public class Program {
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public static void Main() {
            if (AppUtil.IsDotNetVersionEG45) {
                NTMiner.App app = new NTMiner.App();
                app.InitializeComponent();
                app.Run();
            }
            else {
                Process.Start("https://ntminer.com/getDotNet.html");
            }
            // 这个机制在MinerClient程序起作用但在MinerStudio程序中会发生类型初始化错误不起作用，具体原因未知
        }
    }

    public partial class App : Application {
        public static NTMinerAppType AppType {
            get {
                if (AppStatic.IsMinerStudio) {
                    return NTMinerAppType.MinerStudio;
                }
                return NTMinerAppType.MinerClient;
            }
        }

        public App() {
            VirtualRoot.SetOut(NotiCenterWindowViewModel.Instance);
            WpfUtil.Init();
            AppUtil.Init(this);
            InitializeComponent();
        }

        protected override void OnExit(ExitEventArgs e) {
            VirtualRoot.RaiseEvent(new AppExitEvent());
            RpcRoot.RpcUser?.Logout();
            base.OnExit(e);
            NTMinerConsole.Free();
        }

        protected override void OnStartup(StartupEventArgs e) {
            if (AppUtil.GetMutex(NTKeyword.MinerUpdaterAppMutex)) {
                NotiCenterWindow.ShowWindow();
                this.MainWindow = new MainWindow();
                this.MainWindow.Show();
                VirtualRoot.StartTimer(new WpfTimingEventProducer());
                NTMinerConsole.SetIsMainUiOk(true);
            }
            else {
                Process thatProcess = null;
                Process currentProcess = Process.GetCurrentProcess();
                Process[] Processes = Process.GetProcessesByName(currentProcess.ProcessName);
                foreach (Process process in Processes) {
                    if (process.Id != currentProcess.Id) {
                        // 因为挖矿端和群控端的升级器是同一份程序所以区分一下
                        if (typeof(App).Assembly.Location.Equals(currentProcess.MainModule.FileName, StringComparison.OrdinalIgnoreCase)) {
                            thatProcess = process;
                        }
                    }
                }
                if (thatProcess != null) {
                    AppUtil.Show(thatProcess);
                }
                else {
                    MessageBox.Show("另一个升级器已在运行", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                }
                Environment.Exit(0);
                return;
            }
            base.OnStartup(e);
        }
    }
}
