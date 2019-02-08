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
    public partial class App : Application {
        public static readonly bool IsInDesignMode = (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;

        private Mutex mutexApp;

        public App() {
            Logger.InfoDebugLine("App.ctor start");
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

            Execute.InitializeWithDispatcher();
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
            Logger.InfoDebugLine("App.InitializeComponent start");
            InitializeComponent();
            Logger.InfoDebugLine("App.InitializeComponent end");
            Logger.InfoDebugLine("App.ctor end");
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
                DropOld.DropOldUpdater();
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
                    MessageBox.Show("another Updater is running", "alert", MessageBoxButton.OKCancel);
                }
                Environment.Exit(-1);
                return;
            }

            base.OnStartup(e);

            this.MainWindow = new MainWindow();
            this.MainWindow.Show();
        }

        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int SW_SHOWNOMAL = 1;
        private static void Show(Process instance) {
            ShowWindowAsync(instance.MainWindowHandle, SW_SHOWNOMAL);
            SetForegroundWindow(instance.MainWindowHandle);
        }
    }
}
