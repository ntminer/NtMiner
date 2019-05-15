using NTMiner.MinerServer;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace NTMiner {
    public static class WindowFactory {
        #region WindowFactory
        private static readonly object _locker = new object();
        private static Window _instance = null;
        public static void ShowMainWindow(bool isToggle) {
            UIThread.Execute(() => {
                if (_instance == null) {
                    lock (_locker) {
                        if (_instance == null) {
                            _instance = new MainWindow();
                            Application.Current.MainWindow = _instance;
                            _instance.Show();
                            AppContext.Enable();
                            NTMinerRoot.IsUiVisible = true;
                            NTMinerRoot.MainWindowRendedOn = DateTime.Now;
                            VirtualRoot.Happened(new MainWindowShowedEvent());
                            _instance.Closed += (object sender, EventArgs e) => {
                                NTMinerRoot.IsUiVisible = false;
                                _instance = null;
                            };
                        }
                    }
                }
                else {
                    _instance.ShowWindow(isToggle);
                }
            });
        }

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

        private static void RestartNTMiner() {
            Process thisProcess = Process.GetCurrentProcess();
            Windows.TaskKill.KillOtherProcess(thisProcess);
            Windows.Cmd.RunClose(VirtualRoot.AppFileFullName, string.Join(" ", CommandLineArgs.Args));
        }

        public static Window CreateSplashWindow() {
            return new SplashWindow();
        }
        #endregion

        public static void Link() {
            VirtualRoot.Window<ShowKernelDownloaderCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelsWindow.ShowWindow(message.KernelId, message.DownloadComplete);
                    });
                });
            VirtualRoot.Window<EnvironmentVariableEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        EnvironmentVariableEdit.ShowWindow(message.CoinKernelVm, message.EnvironmentVariable);
                    });
                });
            VirtualRoot.Window<InputSegmentEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        InputSegmentEdit.ShowWindow(message.CoinKernelVm, message.Segment);
                    });
                });
            VirtualRoot.Window<CoinKernelEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        CoinKernelEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<CoinEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        CoinEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<ColumnsShowEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        ColumnsShowEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<ShowContainerWindowCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        ContainerWindow window = ContainerWindow.GetWindow(message.Vm);
                        window?.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowSpeedChartsCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        SpeedCharts.ShowWindow(message.GpuSpeedVm);
                    });
                });
            VirtualRoot.Window<GroupEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        GroupEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<KernelInputEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelInputEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<KernelOutputFilterEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelOutputFilterEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<KernelOutputTranslaterEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelOutputTranslaterEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<KernelOutputEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelOutputEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<ShowPackagesWindowCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        PackagesWindow.ShowWindow();
                    });
                });
            VirtualRoot.Window<KernelEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        KernelEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<ShowLogColorCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        LogColor.ShowWindow();
                    });
                });
            VirtualRoot.Window<ShowMinerClientSettingCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        MinerClientSetting.ShowWindow(message.Vm);
                    });
                });
            VirtualRoot.Window<ShowMinerNamesSeterCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        MinerNamesSeter.ShowWindow(message.Vm);
                    });
                });
            VirtualRoot.Window<ShowGpuProfilesPageCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        GpuProfilesPage.ShowWindow(message.MinerClientsWindowVm);
                    });
                });
            VirtualRoot.Window<ShowMinerClientAddCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        MinerClientAdd.ShowWindow();
                    });
                });
            VirtualRoot.Window<MinerGroupEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        MinerGroupEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<MineWorkEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        MineWorkEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<OverClockDataEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        OverClockDataEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<PackageEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        PackageEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<PoolKernelEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        PoolKernelEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<PoolEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        PoolEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<ShowControlCenterHostConfigCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        ControlCenterHostConfig.ShowWindow();
                    });
                });
            VirtualRoot.Window<SysDicItemEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        SysDicItemEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<SysDicEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        SysDicEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<UserEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        UserEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<WalletEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        WalletEdit.ShowWindow(message.FormType, message.Source);
                    });
                });
            VirtualRoot.Window<UpgradeCommand>(LogEnum.DevConsole,
                action: message => {
                    Upgrade(message.FileName, message.Callback);
                });
        }

        #region Upgrade
        private static void Upgrade(string fileName, Action callback) {
            try {
                string updaterDirFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "Updater");
                if (!Directory.Exists(updaterDirFullName)) {
                    Directory.CreateDirectory(updaterDirFullName);
                }
                OfficialServer.FileUrlService.GetNTMinerUpdaterUrlAsync((downloadFileUrl, e) => {
                    try {
                        string ntMinerUpdaterFileFullName = Path.Combine(updaterDirFullName, "NTMinerUpdater.exe");
                        string argument = string.Empty;
                        if (!string.IsNullOrEmpty(fileName)) {
                            argument = "ntminerFileName=" + fileName;
                        }
                        if (VirtualRoot.IsMinerStudio) {
                            argument += " --minerstudio";
                        }
                        if (string.IsNullOrEmpty(downloadFileUrl)) {
                            if (File.Exists(ntMinerUpdaterFileFullName)) {
                                Windows.Cmd.RunClose(ntMinerUpdaterFileFullName, argument);
                            }
                            callback?.Invoke();
                            return;
                        }
                        Uri uri = new Uri(downloadFileUrl);
                        string updaterVersion = string.Empty;
                        if (NTMinerRoot.Instance.LocalAppSettingSet.TryGetAppSetting("UpdaterVersion", out IAppSetting appSetting) && appSetting.Value != null) {
                            updaterVersion = appSetting.Value.ToString();
                        }
                        if (string.IsNullOrEmpty(updaterVersion) || !File.Exists(ntMinerUpdaterFileFullName) || uri.AbsolutePath != updaterVersion) {
                            FileDownloader.ShowWindow(downloadFileUrl, "开源矿工更新器", (window, isSuccess, message, saveFileFullName) => {
                                try {
                                    if (isSuccess) {
                                        File.Copy(saveFileFullName, ntMinerUpdaterFileFullName, overwrite: true);
                                        File.Delete(saveFileFullName);
                                        VirtualRoot.Execute(new ChangeLocalAppSettingCommand(new AppSettingData {
                                            Key = "UpdaterVersion",
                                            Value = uri.AbsolutePath
                                        }));
                                        window?.Close();
                                        Windows.Cmd.RunClose(ntMinerUpdaterFileFullName, argument);
                                        callback?.Invoke();
                                    }
                                    else {
                                        NotiCenterWindowViewModel.Instance.Manager.ShowErrorMessage(message);
                                        callback?.Invoke();
                                    }
                                }
                                catch {
                                    callback?.Invoke();
                                }
                            });
                        }
                        else {
                            Windows.Cmd.RunClose(ntMinerUpdaterFileFullName, argument);
                            callback?.Invoke();
                        }
                    }
                    catch {
                        callback?.Invoke();
                    }
                });
            }
            catch {
                callback?.Invoke();
            }
        }
        #endregion
    }
}
