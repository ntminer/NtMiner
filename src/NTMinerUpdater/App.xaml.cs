using NTMiner.MinerServer;
using NTMiner.Views;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace NTMiner {
    public partial class App : Application, IDisposable {
        private static class NativeMethods {
            [DllImport("User32.dll")]
            public static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

            [DllImport("User32.dll")]
            public static extern bool SetForegroundWindow(IntPtr hWnd);
        }

        public static readonly NTMinerAppType AppType;
        public static readonly bool IsInDesignMode = (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;

        static App() {
            AppType = Environment.CommandLine.IndexOf("--minerstudio", StringComparison.OrdinalIgnoreCase) != -1 ? NTMinerAppType.MinerStudio : NTMinerAppType.MinerClient;
            // 读取注册表中的Location的时候会根据VirtualRoot.IsMinerStudio而变化所以需要赋值
            if (AppType == NTMinerAppType.MinerStudio) {
                VirtualRoot.IsMinerStudio = true;
            }
        }

        private Mutex mutexApp;

        public App() {
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) => {
                var exception = e.ExceptionObject as Exception;
                if (exception != null) {
                    Handle(exception);
                }
            };

            DispatcherUnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) => {
                Handle(e.Exception);
                e.Handled = true;
            };

            UIThread.InitializeWithDispatcher();
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
            InitializeComponent();
        }

        private void Handle(Exception e) {
            if (e == null) {
                return;
            }
            Logger.ErrorDebugLine(e);
        }

        protected override void OnStartup(StartupEventArgs e) {
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            bool mutexCreated;
            try {
                mutexApp = new Mutex(true, "NTMinerUpdaterAppMutex", out mutexCreated);
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
                    MessageBox.Show("Another Updater is running", "alert", MessageBoxButton.OKCancel);
                }
                Environment.Exit(-1);
                return;
            }

            base.OnStartup(e);

            NotiCenterWindow.Instance.Show();
            this.MainWindow = new MainWindow();
            this.MainWindow.Show();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (disposing) {
                if (mutexApp != null) {
                    mutexApp.Dispose();
                }
            }
        }

        private const int SW_SHOWNOMAL = 1;
        private static void Show(Process instance) {
            NativeMethods.ShowWindowAsync(instance.MainWindowHandle, SW_SHOWNOMAL);
            NativeMethods.SetForegroundWindow(instance.MainWindowHandle);
        }
    }
}
