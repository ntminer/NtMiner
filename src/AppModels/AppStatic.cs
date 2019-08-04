using NTMiner.Core;
using NTMiner.MinerServer;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NTMiner {
    // 注意：这里的成员只应用于绑定，不应在.cs中使用，在IDE中看到的静态源代码应用计数应为0
    public static class AppStatic {
        public static readonly BitmapImage BigLogoImageSource = new BitmapImage(new Uri((VirtualRoot.IsMinerStudio ? "/NTMinerWpf;component/Styles/Images/cc128.png" : "/NTMinerWpf;component/Styles/Images/logo128.png"), UriKind.RelativeOrAbsolute));

        #region Upgrade
        public static void Upgrade(string fileName, Action callback) {
            try {
                string updaterDirFullName = Path.Combine(AssemblyInfo.LocalDirFullName, "Updater");
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
                                NTMiner.Windows.Cmd.RunClose(ntMinerUpdaterFileFullName, argument);
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
                            VirtualRoot.Execute(new ShowFileDownloaderCommand(downloadFileUrl, "开源矿工更新器", (window, isSuccess, message, saveFileFullName) => {
                                try {
                                    if (isSuccess) {
                                        File.Copy(saveFileFullName, ntMinerUpdaterFileFullName, overwrite: true);
                                        File.Delete(saveFileFullName);
                                        VirtualRoot.Execute(new ChangeLocalAppSettingCommand(new AppSettingData {
                                            Key = "UpdaterVersion",
                                            Value = uri.AbsolutePath
                                        }));
                                        window?.Close();
                                        NTMiner.Windows.Cmd.RunClose(ntMinerUpdaterFileFullName, argument);
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
                            }));
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

        public static bool IsMinerClient {
            get => VirtualRoot.IsMinerClient;
        }

        public static Visibility IsMinerClientVisible {
            get {
                if (VirtualRoot.IsMinerClient) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public static bool IsMinerStudio {
            get => VirtualRoot.IsMinerStudio;
        }

        public static Visibility IsMinerStudioVisible {
            get {
                if (VirtualRoot.IsMinerStudio) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public static string AppName {
            get {
                Assembly mainAssembly = Assembly.GetEntryAssembly();
                return ((AssemblyTitleAttribute)mainAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), inherit: false).First()).Title;
            }
        }

        public static bool IsDebugMode {
            get {
                return Design.IsDevMode;
            }
        }

        public static bool IsNotDebugMode => !Design.IsDevMode;

        public static Visibility IsDebugModeVisible {
            get {
                if (Design.IsDevMode) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public static Visibility IsDevModeVisible {
            get {
                if (Design.IsDevMode) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public static Visibility IsAmdGpuVisible {
            get {
                if (NTMinerRoot.Instance.GpuSet.GpuType == GpuType.AMD) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public static bool IsPoolBrand {
            get {
                return NTMinerRoot.IsPoolBrand;
            }
        }

        public static Visibility IsPoolBrandVisible {
            get {
                return NTMinerRoot.IsPoolBrand ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public static Visibility IsPoolBrandCollapsed {
            get { return NTMinerRoot.IsPoolBrand ? Visibility.Collapsed : Visibility.Visible; }
        }

        public static bool IsKernelBrand {
            get {
                return NTMinerRoot.IsKernelBrand;
            }
        }

        public static Visibility IsKernelBrandVisible {
            get {
                return NTMinerRoot.IsKernelBrand ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public static Visibility IsKernelBrandCollapsed {
            get { return NTMinerRoot.IsKernelBrand ? Visibility.Collapsed : Visibility.Visible; }
        }

        public static bool IsBrandSpecified {
            get { return NTMinerRoot.IsBrandSpecified; }
        }

        public static Visibility IsBrandSpecifiedVisible {
            get {
                return NTMinerRoot.IsBrandSpecified ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public static Visibility IsBrandSpecifiedCollapsed {
            get { return NTMinerRoot.IsBrandSpecified ? Visibility.Collapsed : Visibility.Visible; }
        }

        public static double MainWindowHeight {
            get {
                if (SystemParameters.WorkArea.Size.Height >= 600) {
                    return 600;
                }
                else if (SystemParameters.WorkArea.Size.Height >= 520) {
                    return 520;
                }
                return 480;
            }
        }

        public static double MainWindowWidth {
            get {
                if (SystemParameters.WorkArea.Size.Width >= 1000) {
                    return 1000;
                }
                else if (SystemParameters.WorkArea.Size.Width >= 860) {
                    return 860;
                }
                else if (SystemParameters.WorkArea.Size.Width >= 800) {
                    return 800;
                }
                return 640;
            }
        }

        public static IEnumerable<EnumItem<SupportedGpu>> SupportedGpuEnumItems {
            get {
                return EnumSet.SupportedGpuEnumItems;
            }
        }

        public static IEnumerable<EnumItem<GpuType>> GpuTypeEnumItems {
            get {
                return EnumSet.GpuTypeEnumItems;
            }
        }

        public static IEnumerable<EnumItem<PublishStatus>> PublishStatusEnumItems {
            get {
                return EnumSet.PublishStatusEnumItems;
            }
        }

        public static IEnumerable<EnumItem<MineStatus>> MineStatusEnumItems {
            get {
                return EnumSet.MineStatusEnumItems;
            }
        }

        public static string CurrentVersion {
            get {
                return NTMinerRoot.CurrentVersion.ToString();
            }
        }

        public static string VersionTag {
            get {
                return NTMinerRoot.CurrentVersionTag;
            }
        }

        public static string VersionFullName {
            get {
                return $"v{NTMinerRoot.CurrentVersion}({VersionTag})";
            }
        }

        public static string QQGroup {
            get {
                if (Design.IsInDesignMode) {
                    return string.Empty;
                }
                if (NTMinerRoot.Instance.SysDicItemSet.TryGetDicItem("ThisSystem", "QQGroup", out ISysDicItem dicItem)) {
                    return dicItem.Value;
                }
                return "863725136";
            }
        }

        private static readonly string _windowsEdition = Windows.OS.Instance.WindowsEdition?.Replace("Windows ", "Win");
        public static string WindowsEdition {
            get {
                return _windowsEdition;
            }
        }

        public static string TotalVirtualMemoryGbText {
            get {
                return AppContext.Instance.VirtualMemorySetVm.TotalVirtualMemoryGbText;
            }
        }

        public static string GpuSetInfo {
            get {
                return NTMinerRoot.Instance.GpuSetInfo;
            }
        }

        public static string DriverVersion {
            get {
                var gpuSet = NTMinerRoot.Instance.GpuSet;
                if (gpuSet.GpuType == GpuType.NVIDIA) {
                    var cudaVersion = gpuSet.Properties.FirstOrDefault(a => a.Code == "CudaVersion");
                    if (cudaVersion != null) {
                        return $"{gpuSet.DriverVersion}_{cudaVersion.Value}";
                    }
                }
                return gpuSet.DriverVersion;
            }
        }

        public static ICommand OpenDir { get; private set; } = new DelegateCommand<string>((dir) => {
            dir = dir.Replace(Consts.LocalDirParameterName, AssemblyInfo.LocalDirFullName);
            Process.Start(dir);
        });

        public static ICommand ViewUrl { get; private set; } = new DelegateCommand<string>(url => {
            Process.Start(url);
        });

        public static ICommand ConfigControlCenterHost { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowControlCenterHostConfigCommand());
        });

        public static ICommand ExportServerJson { get; private set; } = new DelegateCommand(() => {
            try {
                NTMinerRoot.ExportServerVersionJson(AssemblyInfo.ServerVersionJsonFileFullName);
                NotiCenterWindowViewModel.Instance.Manager.ShowSuccessMessage($"{AssemblyInfo.ServerJsonFileName}", "导出成功");
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        });

        public static string ServerJsonFileName { get; private set; } = AssemblyInfo.ServerJsonFileName;

        public static ICommand SetServerJsonVersion { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowDialogWindowCommand(message: $"您确定刷新{AssemblyInfo.ServerJsonFileName}吗？", title: "确认", onYes: () => {
                try {
                    VirtualRoot.Execute(new ChangeServerAppSettingCommand(new AppSettingData {
                        Key = AssemblyInfo.ServerJsonFileName,
                        Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")
                    }));
                    NotiCenterWindowViewModel.Instance.Manager.ShowSuccessMessage($"刷新成功");
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    NotiCenterWindowViewModel.Instance.Manager.ShowErrorMessage($"刷新失败");
                }
            }, icon: IconConst.IconConfirm));
        });

        public static ICommand ShowUsers { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowUserPageCommand());
        });

        public static ICommand ShowOverClockDatas { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowOverClockDataPageCommand());
        });

        public static ICommand ShowChartsWindow { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowChartsWindowCommand());
        });

        public static ICommand ShowProperty { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowPropertyCommand());
        });

        public static ICommand JoinQQGroup { get; private set; } = new DelegateCommand(() => {
            Process.Start("https://jq.qq.com/?_wv=1027&k=5ZPsuCk");
        });

        public static ICommand RunAsAdministrator { get; private set; } = new DelegateCommand(Wpf.Util.RunAsAdministrator);

        public static ICommand ShowNotificationSample { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowNotificationSampleCommand());
        });

        public static ICommand AppExit { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new CloseNTMinerCommand());
        });

        public static ICommand ShowRestartWindows { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowRestartWindowsCommand());
        });

        public static ICommand ShowVirtualMemory { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowVirtualMemoryCommand());
        });

        public static ICommand ShowSysDic { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowSysDicPageCommand());
        });
        public static ICommand ShowGroups { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowGroupPageCommand());
        });
        public static ICommand ShowCoins { get; private set; } = new DelegateCommand<CoinViewModel>((currentCoin) => {
            VirtualRoot.Execute(new ShowCoinPageCommand(currentCoin, "coin"));
        });
        public static ICommand ManagePools { get; private set; } = new DelegateCommand<CoinViewModel>(coinVm => {
            VirtualRoot.Execute(new ShowCoinPageCommand(coinVm, Consts.PoolParameterName));
        });
        public static ICommand ManageWallet { get; private set; } = new DelegateCommand<CoinViewModel>(coinVm => {
            VirtualRoot.Execute(new ShowCoinPageCommand(coinVm, Consts.WalletParameterName));
        });
        public static ICommand ShowKernelInputs { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowKernelInputPageCommand());
        });
        public static ICommand ShowFileWriters { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowFileWriterPageCommand());
        });
        public static ICommand ShowFragmentWriters { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowFragmentWriterPageCommand());
        });
        public static ICommand ShowKernelOutputs { get; private set; } = new DelegateCommand<KernelOutputViewModel>((selectedKernelOutputVm) => {
            VirtualRoot.Execute(new ShowKernelOutputPageCommand(selectedKernelOutputVm));
        });
        public static ICommand ShowKernels { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowKernelsWindowCommand());
        });
        public static ICommand ShowAbout { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowAboutPageCommand());
        });
        public static ICommand ShowSpeedChart { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowSpeedChartsCommand());
        });
        public static ICommand ShowNTMinerUpdaterConfig { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowNTMinerUpdaterConfigCommand());
        });
        public static ICommand ShowOnlineUpdate { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new UpgradeCommand(string.Empty, null));
        });
        public static ICommand ShowHelp { get; private set; } = new DelegateCommand(() => {
            Process.Start("http://ntminer.com/");
        });
        public static ICommand ShowMinerClients { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowMinerClientsWindowCommand());
        });
        public static ICommand ShowCalcConfig { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowCalcConfigCommand());
        });
        public static ICommand ShowGlobalDir { get; private set; } = new DelegateCommand(() => {
            Process.Start(AssemblyInfo.LocalDirFullName);
        });
        public static ICommand OpenLocalLiteDb { get; private set; } = new DelegateCommand(() => {
            OpenLiteDb(SpecialPath.LocalDbFileFullName);
        });
        public static ICommand OpenServerLiteDb { get; private set; } = new DelegateCommand(() => {
            OpenLiteDb(SpecialPath.ServerDbFileFullName);
        });

        private static void OpenLiteDb(string dbFileFullName) {
            string liteDbExplorerDir = Path.Combine(AssemblyInfo.LocalDirFullName, "LiteDBExplorerPortable");
            string liteDbExplorerFileFullName = Path.Combine(liteDbExplorerDir, "LiteDbExplorer.exe");
            if (!Directory.Exists(liteDbExplorerDir)) {
                Directory.CreateDirectory(liteDbExplorerDir);
            }
            if (!File.Exists(liteDbExplorerFileFullName)) {
                OfficialServer.FileUrlService.GetLiteDbExplorerUrlAsync((downloadFileUrl, e) => {
                    if (string.IsNullOrEmpty(downloadFileUrl)) {
                        return;
                    }
                    VirtualRoot.Execute(new ShowFileDownloaderCommand(downloadFileUrl, "LiteDB数据库管理工具", (window, isSuccess, message, saveFileFullName) => {
                        if (isSuccess) {
                            ZipUtil.DecompressZipFile(saveFileFullName, liteDbExplorerDir);
                            File.Delete(saveFileFullName);
                            window?.Close();
                            Windows.Cmd.RunClose(liteDbExplorerFileFullName, dbFileFullName);
                        }
                    }));
                });
            }
            else {
                Windows.Cmd.RunClose(liteDbExplorerFileFullName, dbFileFullName);
            }
        }

        public static ICommand ShowCalc { get; private set; } = new DelegateCommand<CoinViewModel>(coinVm => {
            VirtualRoot.Execute(new ShowCalcCommand(coinVm));
        });

        public static ICommand ShowEthNoDevFee { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowEthNoDevFeeCommand());
        });

        public static ICommand OpenOfficialSite { get; private set; } = new DelegateCommand(() => {
            Process.Start("http://ntminer.com/");
        });

        public static ICommand BusinessModel { get; private set; } = new DelegateCommand(() => {
            Process.Start("https://www.loserhub.cn/posts/details/52");
        });

        public static ICommand OpenGithub { get; private set; } = new DelegateCommand(() => {
            Process.Start("https://github.com/ntminer-project/ntminer");
        });

        public static ICommand OpenDiscussSite { get; private set; } = new DelegateCommand(() => {
            Process.Start("https://github.com/ntminer-project/ntminer/issues");
        });

        public static ICommand DownloadMinerStudio { get; private set; } = new DelegateCommand(() => {
            Process.Start(AssemblyInfo.MinerJsonBucket + "MinerStudio.exe?t=" + DateTime.Now.Ticks);
        });

        public static ICommand ShowQQGroupQrCode { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowQQGroupQrCodeCommand());
        });
    }
}
