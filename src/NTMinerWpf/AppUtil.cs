using NTMiner.Views;
using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace NTMiner {
    public static class AppUtil {
        private static class SafeNativeMethods {
            [DllImport(DllName.User32Dll)]
            public static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

            [DllImport(DllName.User32Dll)]
            public static extern bool SetForegroundWindow(IntPtr hWnd);
        }

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
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
            app.SessionEnding += (sender, e)=> {
                WindowsSessionEndingEvent.ReasonSessionEnding reason;
                switch (e.ReasonSessionEnding) {
                    case ReasonSessionEnding.Logoff:
                        reason = WindowsSessionEndingEvent.ReasonSessionEnding.Logoff;
                        break;
                    case ReasonSessionEnding.Shutdown:
                        reason = WindowsSessionEndingEvent.ReasonSessionEnding.Shutdown;
                        break;
                    default:
                        reason = WindowsSessionEndingEvent.ReasonSessionEnding.Unknown;
                        break;
                }
                VirtualRoot.RaiseEvent(new WindowsSessionEndingEvent(reason));
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
