using System;
using System.Diagnostics;
using System.Windows;

namespace NTMiner.View {
    public abstract class AbstractAppViewFactory : IAppViewFactory {
        private static readonly object _locker = new object();
        private static Window _mainWindow = null;

        public AbstractAppViewFactory() {
            VirtualRoot.AddCmdPath<CloseNTMinerCommand>(action: message => {
                // 不能推迟这个日志记录的时机，因为推迟会有windows异常日志
                VirtualRoot.ThisLocalInfo(nameof(AbstractAppViewFactory), $"退出{VirtualRoot.AppName}。原因：{message.Reason}");
                UIThread.Execute(() => {
                    try {
                        Application.Current.Shutdown();
                    }
                    catch (Exception ex) {
                        Logger.ErrorDebugLine(ex);
                        Environment.Exit(0);
                    }
                });
            }, location: typeof(AbstractAppViewFactory));
        }

        public void ShowMainWindow(bool isToggle, out Window mainWindow) {
            if (_mainWindow == null) {
                lock (_locker) {
                    if (_mainWindow == null) {
                        _mainWindow = CreateMainWindow();
                        NTMinerContext.RefreshArgsAssembly.Invoke("主界面创建后");
                        _mainWindow.Show();
                    }
                }
            }
            else {
                AppRoot.Enable();
                bool needActive = _mainWindow.WindowState != WindowState.Minimized;
                _mainWindow.ShowWindow(isToggle);
                if (needActive) {
                    _mainWindow.Activate();
                }
            }
            mainWindow = _mainWindow;
        }

        public abstract void BuildPaths();
        public abstract Window CreateMainWindow();

        public void ShowMainWindow(Application app, NTMinerAppType appType) {
            try {
                switch (appType) {
                    case NTMinerAppType.MinerClient:
                        RpcRoot.Client.MinerClientService.ShowMainWindowAsync((isSuccess, exception) => {
                            if (!isSuccess) {
                                RestartNTMiner();
                            }
                            UIThread.Execute(() => app.Shutdown());
                        });
                        break;
                    case NTMinerAppType.MinerStudio:
                        RpcRoot.Client.MinerStudioService.ShowMainWindowAsync((isSuccess, exception) => {
                            if (!isSuccess) {
                                RestartNTMiner();
                            }
                            UIThread.Execute(() => app.Shutdown());
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
