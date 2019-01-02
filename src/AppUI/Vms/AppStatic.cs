using NTMiner.Core.Impl;
using NTMiner.Notifications;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public static class AppStatic {
        public static ICommand ExportServerJson { get; private set; } = new DelegateCommand(() => {
            try {
                ServerJson.Export();
                MainWindowViewModel.Current.Manager.CreateMessage()
                    .Accent("#1751C3")
                    .Background("#333")
                    .HasBadge("Info")
                    .HasMessage("导出成功")
                    .Dismiss()
                    .WithDelay(TimeSpan.FromSeconds(2))
                    .Queue();
            }
            catch (Exception e) {
                Global.Logger.Error(e.Message, e);
            }
        });

        public static ICommand ShowLangViewItems { get; private set; } = new DelegateCommand<string>((viewId) => {
            ViewLang.ShowWindow(new ViewLangViewModel(viewId));
        });

        public static ICommand JoinQQGroup { get; private set; } = new DelegateCommand(() => {
            Process.Start("https://jq.qq.com/?_wv=1027&k=5ZPsuCk");
        });

        public static ICommand RunAsAdministrator { get; private set; } = new DelegateCommand(() => {
            AppHelper.RunAsAdministrator();
        });

        public static ICommand ShowNotificationSample { get; private set; } = new DelegateCommand(() => {
            NotificationSample.ShowWindow();
        });

        public static ICommand AppExit { get; private set; } = new DelegateCommand(() => {
            Application.Current.MainWindow.Close();
        });

        public static ICommand ShowRestartWindows { get; private set; } = new DelegateCommand(() => {
            RestartWindows.ShowDialog();
        });

        public static ICommand ShowVirtualMemory { get; private set; } = new DelegateCommand(() => {
            Views.Ucs.VirtualMemory.ShowWindow();
        });
        public static ICommand ViewEvent { get; private set; } = new DelegateCommand(() => {
            EventPage.ShowWindow();
        });
        public static ICommand ShowSysDic { get; private set; } = new DelegateCommand(() => {
            SysDicPage.ShowWindow();
        });
        public static ICommand ShowMinerProfile { get; private set; } = new DelegateCommand(() => {
            MinerProfileOption.ShowWindow();
        });
        public static ICommand ShowGroups { get; private set; } = new DelegateCommand(() => {
            GroupPage.ShowWindow();
        });
        public static ICommand ShowCoins { get; private set; } = new DelegateCommand(() => {
            CoinPage.ShowWindow();
        });
        public static ICommand ShowKernels { get; private set; } = new DelegateCommand(() => {
            KernelPage.ShowWindow(Guid.Empty);
        });
        public static ICommand ShowAbout { get; private set; } = new DelegateCommand<string>((appType) => {
            AboutPage.ShowWindow(appType);
        });
        public static ICommand ShowSpeedChart { get; private set; } = new DelegateCommand(() => {
            SpeedCharts.ShowWindow();
        });
        public static ICommand ShowOnlineUpdate { get; private set; } = new DelegateCommand(() => {
            string updaterDirFullName = Path.Combine(Global.GlobalDirFullName, "Updater");
            if (!Directory.Exists(updaterDirFullName)) {
                Directory.CreateDirectory(updaterDirFullName);
            }
            string ntMinerUpdaterFileFullName = Path.Combine(updaterDirFullName, "NTMinerUpdater.exe");
            if (!File.Exists(ntMinerUpdaterFileFullName)) {
                Server.FileUrlService.GetNTMinerUpdaterUrl(downloadFileUrl => {
                    if (string.IsNullOrEmpty(downloadFileUrl)) {
                        return;
                    }
                    FileDownloader.ShowWindow(downloadFileUrl, "开源矿工更新器", (window, isSuccess, message, saveFileFullName) => {
                        if (isSuccess) {
                            File.Copy(saveFileFullName, ntMinerUpdaterFileFullName);
                            File.Delete(saveFileFullName);
                            window?.Close();
                            Windows.Cmd.RunClose(ntMinerUpdaterFileFullName, string.Empty);
                        }
                    });
                });
            }
            else {
                Windows.Cmd.RunClose(ntMinerUpdaterFileFullName, string.Empty);
            }
        });
        public static ICommand ShowHelp { get; private set; } = new DelegateCommand(() => {
            Process.Start("https://github.com/ntminer/ntminer");
        });
        public static ICommand ManageMinerGroup { get; private set; } = new DelegateCommand(() => {
            MinerGroupPage.ShowWindow();
        });
        public static ICommand ManageMineWork { get; private set; } = new DelegateCommand(() => {
            MineWorkPage.ShowWindow();
        });
        public static ICommand ShowMinerClients { get; private set; } = new DelegateCommand(() => {
            MinerClients.ShowWindow();
        });
        public static ICommand ShowCalcConfig { get; private set; } = new DelegateCommand(() => {
            CalcConfig.ShowWindow();
        });
        public static ICommand ShowShareDir { get; private set; } = new DelegateCommand(() => {
            Process.Start(Global.GlobalDirFullName);
        });
        public static ICommand OpenLangLiteDb { get; private set; } = new DelegateCommand(() => {
            OpenLiteDb(SpecialPath.LangDbFileFullName);
        });
        public static ICommand OpenLocalLiteDb { get; private set; } = new DelegateCommand(() => {
            OpenLiteDb(SpecialPath.LocalDbFileFullName);
        });
        public static ICommand OpenServerLiteDb { get; private set; } = new DelegateCommand(() => {
            OpenLiteDb(SpecialPath.ServerDbFileFullName);
        });

        private static void OpenLiteDb(string dbFileFullName) {
            string liteDbExplorerDir = Path.Combine(Global.GlobalDirFullName, "LiteDBExplorerPortable");
            string liteDbExplorerFileFullName = Path.Combine(liteDbExplorerDir, "LiteDbExplorer.exe");
            if (!Directory.Exists(liteDbExplorerDir)) {
                Directory.CreateDirectory(liteDbExplorerDir);
            }
            if (!File.Exists(liteDbExplorerFileFullName)) {
                Server.FileUrlService.GetLiteDBExplorerUrl(downloadFileUrl => {
                    if (string.IsNullOrEmpty(downloadFileUrl)) {
                        return;
                    }
                    FileDownloader.ShowWindow(downloadFileUrl, "LiteDB数据库管理工具", (window, isSuccess, message, saveFileFullName) => {
                        if (isSuccess) {
                            ZipUtil.DecompressZipFile(saveFileFullName, liteDbExplorerDir);
                            File.Delete(saveFileFullName);
                            window?.Close();
                            Windows.Cmd.RunClose(liteDbExplorerFileFullName, dbFileFullName);
                        }
                    });
                });
            }
            else {
                Windows.Cmd.RunClose(liteDbExplorerFileFullName, dbFileFullName);
            }
        }

        public static ICommand ShowCalc { get; private set; } = new DelegateCommand<CoinViewModel>(coin => {
            Calc.ShowWindow(coin);
        });

        public static ICommand OpenOfficialSite { get; private set; } = new DelegateCommand(() => {
            Process.Start("http://ntminer.com/");
        });

        public static ICommand ShowQQGroupQrCode { get; private set; } = new DelegateCommand(() => {
            QQGroupQrCode.ShowWindow();
        });

        public static bool IsDevMode {
            get {
                if (NTMinerRoot.IsInDesignMode) {
                    return true;
                }
                return DevMode.IsDevMode;
            }
        }

        public static bool IsNotDevMode {
            get {
                return !IsDevMode;
            }
        }

        public static Visibility IsDevModeVisible {
            get {
                if (IsDevMode) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public static Visibility IsNotDevModeVisible {
            get {
                if (IsNotDevMode) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        private static bool _isMinerClient;
        public static bool IsMinerClient {
            get {
                return _isMinerClient;
            }
            set {
                _isMinerClient = value;
                if (value) {
                    IsMinerClientVisible = Visibility.Visible;
                    IsMinerClientNotVisible = Visibility.Collapsed;
                }
                else {
                    IsMinerClientVisible = Visibility.Collapsed;
                    IsMinerClientNotVisible = Visibility.Visible;
                }
            }
        }

        public static Visibility IsMinerClientVisible {
            get; private set;
        }

        public static Visibility IsMinerClientNotVisible {
            get; private set;
        }
    }
}
