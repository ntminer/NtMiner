using NTMiner.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace NTMiner {
    public partial class App : Application {
        public App() {
            Logging.LogDir.SetDir(Path.Combine(ClientId.GlobalDirFullName, "Logs"));
            AppHelper.Init(this);
            InitializeComponent();
        }

        private bool createdNew;
        private Mutex appMutex;
        private static string _appPipName = "ntminerclient";
        ExtendedNotifyIcon notifyIcon;

        protected override void OnExit(ExitEventArgs e) {
            notifyIcon?.Dispose();
            NTMinerRoot.Current.Exit();
            base.OnExit(e);
        }

        protected override void OnStartup(StartupEventArgs e) {
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            if (!string.IsNullOrEmpty(CommandLineArgs.Upgrade)) {
                Vms.AppStatic.Upgrade(CommandLineArgs.Upgrade, ()=> {
                    Environment.Exit(0);
                });
            }
            else {
                try {
                    appMutex = new Mutex(true, _appPipName, out createdNew);
                }
                catch (Exception) {
                    createdNew = false;
                }
                if (createdNew) {
                    Vms.AppStatic.IsMinerClient = true;
                    SplashWindow splashWindow = new SplashWindow();
                    splashWindow.Show();
                    NTMinerRoot.Current.Init(OnNTMinerRootInited);
                }
                else {
                    try {
                        if (CommandLineArgs.IsWorkEdit) {
                            string arguments = NTMinerRegistry.GetArguments();
                            if (arguments == $"--controlcenter workid={CommandLineArgs.WorkId}") {
                                AppHelper.ShowMainWindow(this, _appPipName);
                            }
                            else {
                                NTMinerClientDaemon.Instance.RestartNTMinerAsync(VirtualRoot.Localhost, 3337, CommandLineArgs.WorkId, null);
                                this.Shutdown();
                            }
                        }
                        else {
                            AppHelper.ShowMainWindow(this, _appPipName);
                        }
                    }
                    catch (Exception) {
                        DialogWindow.ShowDialog(message: "另一个NTMiner正在运行，请手动结束正在运行的NTMiner进程后再次尝试。", title: "alert", icon: "Icon_Error");
                        Process currentProcess = Process.GetCurrentProcess();
                        Process[] processes = Process.GetProcessesByName(currentProcess.ProcessName);
                        foreach (var process in processes) {
                            if (process.Id != currentProcess.Id) {
                                NTMiner.Windows.TaskKill.Kill(process.Id);
                            }
                        }
                    }
                }
            }
            base.OnStartup(e);
        }

        private void OnNTMinerRootInited() {
            OhGodAnETHlargementPill.OhGodAnETHlargementPillUtil.Access();
            NTMinerRoot.KernelDownloader = new KernelDownloader();
            Execute.OnUIThread(() => {
                Window splashWindow = MainWindow;
                MainWindow window = new MainWindow();
                IMainWindow mainWindow = window;
                this.MainWindow = window;
                this.MainWindow.Show();
                this.MainWindow.Activate();
                notifyIcon = new ExtendedNotifyIcon("pack://application:,,,/NTMiner;component/logo.ico");
                notifyIcon.Init();
                #region 处理显示主界面命令
                VirtualRoot.Access<ShowMainWindowCommand>(
                    Guid.Parse("01f3c467-f494-42b8-bcb5-848050df59f3"),
                    "处理显示主界面命令",
                    LogEnum.None,
                    action: message => {
                        Execute.OnUIThread(() => {
                            Dispatcher.Invoke((ThreadStart)mainWindow.ShowThisWindow);
                        });
                    });
                #endregion
                #region 处理重启NTMiner命令
                VirtualRoot.Access<RestartNTMinerCommand>(
                    Guid.Parse("d1712c1f-507c-496f-9da2-870cbd9fc57f"),
                    "处理重启NTMiner命令",
                    LogEnum.None,
                    action: message => {
                        List<string> args = CommandLineArgs.Args;
                        if (message.IsWorkEdit) {
                            if (CommandLineArgs.IsWorkEdit && CommandLineArgs.WorkId == message.MineWorkId) {
                                Execute.OnUIThread(() => {
                                    Dispatcher.Invoke((ThreadStart)mainWindow.ShowThisWindow);
                                });
                                return;
                            }
                            if (!CommandLineArgs.IsControlCenter) {
                                args.Add("--controlCenter");
                            }
                        }
                        if (message.MineWorkId != Guid.Empty) {
                            if (!CommandLineArgs.IsWorker) {
                                args.Add("workid=" + message.MineWorkId.ToString());
                            }
                            else {
                                for (int i = 0; i < args.Count; i++) {
                                    if (args[i].StartsWith("workid=", StringComparison.OrdinalIgnoreCase)) {
                                        args[i] = "workid=" + message.MineWorkId.ToString();
                                        break;
                                    }
                                }
                            }
                        }
                        else {
                            if (CommandLineArgs.IsWorker) {
                                int workIdIndex = -1;
                                for (int i = 0; i < args.Count; i++) {
                                    if (args[i].ToLower().Contains("workid=")) {
                                        workIdIndex = i;
                                        break;
                                    }
                                }
                                if (workIdIndex != -1) {
                                    args.RemoveAt(workIdIndex);
                                }
                            }
                        }
                        NTMiner.Windows.Cmd.RunClose(ClientId.AppFileFullName, string.Join(" ", args));
                        Current.MainWindow.Close();
                    });
                #endregion
                try {
                    NTMinerRoot.Current.Start();
                }
                catch (Exception ex) {
                    Logger.ErrorDebugLine(ex.Message, ex);
                }
                splashWindow?.Close();
                if (NTMinerRoot.Current.MinerProfile.IsAutoStart || CommandLineArgs.IsAutoStart) {
                    Logger.InfoDebugLine("自动开始挖矿倒计时");
                    Views.Ucs.AutoStartCountdown.ShowDialog();
                }
            });
        }
    }
}
