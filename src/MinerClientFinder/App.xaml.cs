using NTMiner.Views;
using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.Windows;

namespace NTMiner {
    public partial class App : Application {
        public App() {
            Logger.Disable();
            NTMinerConsole.Disable();
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
            if (AppUtil.GetMutex(NTKeyword.MinerClientFinderAppMutex)) {
                NotiCenterWindow.ShowWindow();
                MainWindow = new MainWindow();
                MainWindow.Show();
                VirtualRoot.StartTimer(new WpfTimingEventProducer());
            }
            else {
                Process thatProcess = null;
                Process currentProcess = Process.GetCurrentProcess();
                Process[] Processes = Process.GetProcessesByName(currentProcess.ProcessName);
                foreach (Process process in Processes) {
                    if (process.Id != currentProcess.Id) {
                        thatProcess = process;
                        break;
                    }
                }
                if (thatProcess != null) {
                    AppUtil.Show(thatProcess);
                }
                else {
                    MessageBox.Show("另一个矿机雷达已在运行", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                }
                Environment.Exit(0);
                return;
            }

            base.OnStartup(e);
        }
    }
}
