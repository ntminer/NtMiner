using NTMiner.MinerServer;
using NTMiner.Views;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace NTMiner {
    public static class AppHelper {
        public static ExtendedNotifyIcon NotifyIcon;
        public static Action<RemoteDesktopInput> RemoteDesktop;

        public static void RunAsAdministrator() {
            ProcessStartInfo startInfo = new ProcessStartInfo {
                FileName = VirtualRoot.AppFileFullName,
                Arguments = string.Join(" ", CommandLineArgs.Args),
                Verb = "runas"
            };
            Process.Start(startInfo);
            Application.Current.Shutdown();
        }

        #region Init
        public static void Init(Application app) {
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) => {
                if (e.ExceptionObject is Exception exception) {
                    Handle(exception);
                }
            };

            app.DispatcherUnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) => {
                Handle(e.Exception);
                e.Handled = true;
            };

            UIThread.InitializeWithDispatcher();
            UIThread.StartTimer();
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
        }
        #endregion

        #region ShowMainWindow
        public static void ShowMainWindow(Application app, NTMinerAppType appType) {
            try {
                switch (appType) {
                    case NTMinerAppType.MinerClient:
                        Client.MinerClientService.ShowMainWindowAsync(Consts.MinerClientPort, (isSuccess, exception) => {
                            if (!isSuccess) {
                                RestartNTMiner();
                            }
                            UIThread.Execute(() => {
                                app.Shutdown();
                            });
                        });
                        break;
                    case NTMinerAppType.MinerStudio:
                        Client.MinerStudioService.ShowMainWindowAsync(Consts.MinerStudioPort, (isSuccess, exception) => {
                            if (!isSuccess) {
                                RestartNTMiner();
                            }
                            UIThread.Execute(() => {
                                app.Shutdown();
                            });
                        });
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex) {
                RestartNTMiner();
                Logger.ErrorDebugLine(ex.Message, ex);
            }
        }

        public static void ShowWindow(Window window, bool isToggle) {
            if (!isToggle) {
                window.Show();
            }
            window.ShowInTaskbar = true;
            if (window.WindowState == WindowState.Minimized) {
                window.WindowState = WindowState.Normal;
            }
            else {
                if (isToggle) {
                    window.WindowState = WindowState.Minimized;
                }
                else {
                    var oldState = window.WindowState;
                    window.WindowState = WindowState.Minimized;
                    window.WindowState = oldState;
                }
            }
            if (!isToggle) {
                window.Activate();
            }
        }

        private static void RestartNTMiner() {
            Process thisProcess = Process.GetCurrentProcess();
            Windows.TaskKill.KillOtherProcess(thisProcess);
            Windows.Cmd.RunClose(VirtualRoot.AppFileFullName, string.Join(" ", CommandLineArgs.Args));
        }
        #endregion

        #region private methods
        private static void Handle(Exception e) {
            if (e == null) {
                return;
            }
            if (e is ValidationException) {
                DialogWindow.ShowDialog(title: "验证失败", message: e.Message, icon: "Icon_Error");
            }
            else {
                Logger.ErrorDebugLine(e);
            }
        }
        #endregion
    }
}
