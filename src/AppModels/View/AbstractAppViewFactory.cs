using NTMiner.MinerServer;
using System;
using System.Diagnostics;
using System.Windows;

namespace NTMiner.View {
    public abstract class AbstractAppViewFactory : IAppViewFactory {
        private static readonly object _locker = new object();
        private static Window _mainWindow = null;
        public void ShowMainWindow(bool isToggle) {
            UIThread.Execute(() => {
                if (_mainWindow == null) {
                    lock (_locker) {
                        if (_mainWindow == null) {
                            _mainWindow = CreateMainWindow();
                            Application.Current.MainWindow = _mainWindow;
                            _mainWindow.Show();
                            AppContext.Enable();
                            NTMinerRoot.IsUiVisible = true;
                            NTMinerRoot.MainWindowRendedOn = DateTime.Now;
                            VirtualRoot.Happened(new MainWindowShowedEvent());
                            _mainWindow.Closed += (object sender, EventArgs e) => {
                                NTMinerRoot.IsUiVisible = false;
                                _mainWindow = null;
                            };
                        }
                    }
                }
                else {
                    bool needActive = _mainWindow.WindowState != WindowState.Minimized;
                    _mainWindow.ShowWindow(isToggle);
                    if (needActive) {
                        _mainWindow.Activate();
                    }
                }
            });
        }

        public abstract void Link();
        public abstract Window CreateMainWindow();
        public abstract Window CreateSplashWindow();

        public void ShowMainWindow(Application app, NTMinerAppType appType) {
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
            catch (Exception e) {
                RestartNTMiner();
                Logger.ErrorDebugLine(e);
            }
        }

        private void RestartNTMiner() {
            Process thisProcess = Process.GetCurrentProcess();
            Windows.TaskKill.KillOtherProcess(thisProcess);
            Windows.Cmd.RunClose(VirtualRoot.AppFileFullName, string.Join(" ", CommandLineArgs.Args));
        }
    }
}
