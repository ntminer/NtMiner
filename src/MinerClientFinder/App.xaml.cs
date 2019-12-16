using NTMiner.Views;
using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace NTMiner {
    public partial class App : Application {
        public App() {
            Logger.Disable();
            Write.Disable();
            VirtualRoot.SetOut(NotiCenterWindowViewModel.Instance);
            AppUtil.Init(this);
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e) {
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            if (!AppUtil.GetMutex("MinerClientFinderAppMutex")) {
                Process thatProcess = null;
                Process currentProcess = Process.GetCurrentProcess();
                Process[] Processes = Process.GetProcessesByName(currentProcess.ProcessName);
                foreach (Process process in Processes) {
                    if (process.Id != currentProcess.Id) {
                        if (typeof(App).Assembly.Location.Equals(currentProcess.MainModule.FileName, StringComparison.OrdinalIgnoreCase)) {
                            thatProcess = process;
                        }
                    }
                }
                if (thatProcess != null) {
                    AppUtil.Show(thatProcess);
                }
                else {
                    MessageBox.Show("Another MinerClientFinder is running", "alert", MessageBoxButton.OKCancel);
                }
                Environment.Exit(-1);
                return;
            }

            base.OnStartup(e);

            NotiCenterWindow.Instance.ShowWindow();
            MainWindow = new MainWindow();
            MainWindow.Show();
            VirtualRoot.StartTimer(new WpfTimer());
        }
    }
}
