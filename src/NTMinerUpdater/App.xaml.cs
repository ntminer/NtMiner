using NTMiner.MinerServer;
using NTMiner.Views;
using NTMiner.Vms;
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
        private static class SafeNativeMethods {
            [DllImport(DllName.User32Dll)]
            public static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

            [DllImport(DllName.User32Dll)]
            public static extern bool SetForegroundWindow(IntPtr hWnd);
        }

        public static NTMinerAppType AppType {
            get {
                if (VirtualRoot.IsMinerStudio) {
                    return NTMinerAppType.MinerStudio;
                }
                return NTMinerAppType.MinerClient;
            }
        }
        public static readonly bool IsInDesignMode = (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;

        private Mutex mutexApp;

        public App() {
            VirtualRoot.SetOut(NotiCenterWindowViewModel.Instance);
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) => {
                if (e.ExceptionObject is Exception exception) {
                    Handle(exception);
                }
            };

            DispatcherUnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) => {
                Handle(e.Exception);
                e.Handled = true;
            };

            Write.UIThreadId = Dispatcher.Thread.ManagedThreadId;
            UIThread.InitializeWithDispatcher();
            VirtualRoot.StartTimer(new WpfTimer());
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

            NotiCenterWindow.Instance.ShowWindow();
            this.MainWindow = new MainWindow();
            this.MainWindow.Show();
        }

        public void Dispose() {
            CleanUp(true);
            GC.SuppressFinalize(this);
        }

        private void CleanUp(bool disposing) {
            if (disposing) {
                if (mutexApp != null) {
                    mutexApp.Dispose();
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
