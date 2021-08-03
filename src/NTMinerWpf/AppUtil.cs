using Microsoft.Win32;
using NTMiner.Views;
using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace NTMiner {
    public static class AppUtil {
        private static bool IsDotNetVersionGE45 {
            get {
                const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
                using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey)) {
                    if (ndpKey != null) {
                        var obj = ndpKey.GetValue("Release");
                        if (obj != null) {
                            return (int)obj >= 378389;
                        }
                    }
                    return false;
                }
            }
        }

        public static void Run<TApp>(bool withSplashWindow = false) where TApp : Application, IApp, new() {
            if (withSplashWindow) {
                SplashScreen splashScreen = new SplashScreen("splashwindow.png");
                splashScreen.Show(true);
            }
            if (IsDotNetVersionGE45) {
                TApp app = new TApp();
                app.InitializeComponent();
                app.Run();
            }
            else {
                Process.Start("http://dl.ntminer.top/getDotNet.html");
            }
            // 这个机制在MinerClient程序起作用但在MinerStudio程序中会发生类型初始化错误不起作用，具体原因未知
        }

        private static class SafeNativeMethods {
            [DllImport(DllName.User32Dll)]
            public static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

            [DllImport(DllName.User32Dll)]
            public static extern bool SetForegroundWindow(IntPtr hWnd);
        }

        // 因为每个App都配备有个NotiCenterWindow，所以热键的逻辑放在NotiCenterWindow中完成
        public static bool IsHotKeyEnabled { get; set; }

        private static Application _app;
        #region Init
        public static void Init(Application app) {
            _app = app;
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) => {
                if (e.ExceptionObject is Exception exception) {
                    Handle(exception);
                }
            };

            app.DispatcherUnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) => {
                Handle(e.Exception);
                e.Handled = true;
            };

            UIThread.InitializeWithDispatcher(app.Dispatcher);
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(60000));
            app.Exit += (sender, e) => {
                _mutexApp?.Dispose();
            };
            app.SessionEnding += (sender, e) => {
                SessionEndReasons reason;
                switch (e.ReasonSessionEnding) {
                    case ReasonSessionEnding.Logoff:
                        reason = SessionEndReasons.Logoff;
                        break;
                    case ReasonSessionEnding.Shutdown:
                        reason = SessionEndReasons.SystemShutdown;
                        break;
                    default:
                        reason = SessionEndReasons.Logoff;
                        break;
                }
                SessionEndingEventArgs args = new SessionEndingEventArgs(reason) {
                    Cancel = e.Cancel
                };
                VirtualRoot.SessionEndingEventHandler?.Invoke(sender, args);
            };
        }
        #endregion

        [System.Diagnostics.CodeAnalysis.SuppressMessage("代码质量", "IDE0052:删除未读的私有成员", Justification = "这个成员是供下层操作系统访问的")]
        private static Mutex _mutexApp;
        public static bool GetMutex(string name) {
            bool result;
            try {
                _mutexApp = new Mutex(true, name, out result);
            }
            catch {
                result = false;
            }
            return result;
        }

        private const int SW_SHOWNOMAL = 1;
        public static void Show(Process instance) {
            SafeNativeMethods.ShowWindowAsync(instance.MainWindowHandle, SW_SHOWNOMAL);
            SafeNativeMethods.SetForegroundWindow(instance.MainWindowHandle);
        }

        public static T GetResource<T>(string key) {
            if (_app == null) {
                return (T)Application.Current.Resources[key];
            }
            return (T)_app.Resources[key];
        }

        public static object GetResource(string key) {
            if (_app == null) {
                return Application.Current.Resources[key];
            }
            return _app.Resources[key];
        }

        #region private methods
        private static void Handle(Exception e) {
            if (e == null) {
                return;
            }
            if (e is ValidationException) {
                DialogWindow.ShowSoftDialog(new DialogWindowViewModel(title: "验证失败", message: e.Message, icon: "Icon_Error"));
            }
            else {
                Logger.ErrorDebugLine(e);
            }
        }
        #endregion
    }
}
