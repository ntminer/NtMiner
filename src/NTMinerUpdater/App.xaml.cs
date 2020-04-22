using NTMiner.Views;
using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace NTMiner {
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
            HomePath.SetHomeDirFullName(AppDomain.CurrentDomain.BaseDirectory);
            VirtualRoot.SetOut(NotiCenterWindowViewModel.Instance);
            WpfUtil.Init();
            AppUtil.Init(this);
            InitializeComponent();
        }

        protected override void OnExit(ExitEventArgs e) {
            VirtualRoot.RaiseEvent(new AppExitEvent());
            base.OnExit(e);
            NTMinerConsole.Free();
        }

        protected override void OnStartup(StartupEventArgs e) {
            if (AppUtil.GetMutex(NTKeyword.MinerUpdaterAppMutex)) {
                NotiCenterWindow.ShowWindow();
                this.MainWindow = new MainWindow();
                this.MainWindow.Show();
                VirtualRoot.StartTimer(new WpfTimingEventProducer());
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
