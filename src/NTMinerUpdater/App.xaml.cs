using NTMiner.MinerServer;
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
                if (VirtualRoot.IsMinerStudio) {
                    return NTMinerAppType.MinerStudio;
                }
                return NTMinerAppType.MinerClient;
            }
        }

        public App() {
            VirtualRoot.SetOut(NotiCenterWindowViewModel.Instance);
            AppUtil.Init(this);
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e) {
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            if (AppUtil.GetMutex(NTKeyword.MinerUpdaterAppMutex)) {
                NotiCenterWindow.Instance.ShowWindow();
                this.MainWindow = new MainWindow();
                this.MainWindow.Show();
                VirtualRoot.StartTimer(new WpfTimer());
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
                Environment.Exit(-1);
                return;
            }
            base.OnStartup(e);
        }
    }
}
