using NTMiner.Views;
using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace NTMiner {
    public partial class App : Application {
        private static class SafeNativeMethods {
            [DllImport(DllName.User32Dll)]
            public static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

            [DllImport(DllName.User32Dll)]
            public static extern bool SetForegroundWindow(IntPtr hWnd);
        }

        private Mutex _mutexApp;

        public App() {
            Logger.Disable();
            Write.Disable();
            VirtualRoot.SetOut(NotiCenterWindowViewModel.Instance);
            AppUtil.Init(this);
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e) {
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            bool mutexCreated;
            try {
                _mutexApp = new Mutex(true, "MinerClientFinderAppMutex", out mutexCreated);
            }
            catch {
                mutexCreated = false;
            }
            if (mutexCreated == false) {
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
                    Show(thatProcess);
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

        public void Dispose() {
            CleanUp(true);
            GC.SuppressFinalize(this);
        }

        private void CleanUp(bool disposing) {
            if (disposing) {
                if (_mutexApp != null) {
                    _mutexApp.Dispose();
                }
            }
        }

        private const int SW_SHOWNOMAL = 1;
        private static void Show(Process instance) {
            SafeNativeMethods.ShowWindowAsync(instance.MainWindowHandle, SW_SHOWNOMAL);
            SafeNativeMethods.SetForegroundWindow(instance.MainWindowHandle);
        }
    }
}
