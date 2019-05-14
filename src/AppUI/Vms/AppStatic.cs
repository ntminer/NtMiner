using NTMiner.Core;
using NTMiner.MinerServer;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NTMiner.Vms {
    // 注意：这里的成员只应用于绑定，不应在.cs中使用，在IDE中看到的静态源代码应用计数应为0
    public static class AppStatic {
        public static readonly BitmapImage BigLogoImageSource = new BitmapImage(new Uri((VirtualRoot.IsMinerStudio ? "/NTMinerWpf;component/Styles/Images/cc128.png" : "/NTMinerWpf;component/Styles/Images/logo128.png"), UriKind.RelativeOrAbsolute));
        
        public static bool IsMinerClient {
            get => NTMinerRoot.IsMinerClient;
        }

        public static string AppName {
            get {
                Assembly mainAssembly = Assembly.GetEntryAssembly();
                return ((AssemblyTitleAttribute)mainAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), inherit: false).First()).Title;
            }
        }

        public static bool IsDebugMode {
            get {
                return Design.IsDebugMode;
            }
        }

        public static bool IsNotDebugMode => !Design.IsDebugMode;

        public static Visibility IsDebugModeVisible {
            get {
                if (Design.IsDebugMode) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public static Visibility IsDevModeVisible {
            get {
                if (DevMode.IsDevMode) {
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
                return SupportedGpu.AMD.GetEnumItems();
            }
        }

        public static IEnumerable<EnumItem<GpuType>> GpuTypeEnumItems {
            get {
                return GpuType.AMD.GetEnumItems();
            }
        }

        public static IEnumerable<EnumItem<PublishStatus>> PublishStatusEnumItems {
            get {
                return PublishStatus.Published.GetEnumItems();
            }
        }

        public static IEnumerable<EnumItem<MineStatus>> MineStatusEnumItems {
            get {
                return MineStatus.All.GetEnumItems();
            }
        }

        public static string GetIconFileFullName(ICoin coin) {
            if (coin == null || string.IsNullOrEmpty(coin.Icon)) {
                return string.Empty;
            }
            string iconFileFullName = Path.Combine(SpecialPath.CoinIconsDirFullName, coin.Icon);
            return iconFileFullName;
        }

        public static void Upgrade(string ntminerFileName, Action callback) {
            try {
                string updaterDirFullName = Path.Combine(VirtualRoot.GlobalDirFullName, "Updater");
                if (!Directory.Exists(updaterDirFullName)) {
                    Directory.CreateDirectory(updaterDirFullName);
                }
                OfficialServer.FileUrlService.GetNTMinerUpdaterUrlAsync((downloadFileUrl, e) => {
                    try {
                        string ntMinerUpdaterFileFullName = Path.Combine(updaterDirFullName, "NTMinerUpdater.exe");
                        string argument = string.Empty;
                        if (!string.IsNullOrEmpty(ntminerFileName)) {
                            argument = "ntminerFileName=" + ntminerFileName;
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

        public static string QQGroup {
            get {
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
                        return $"{gpuSet.DriverVersion} / {cudaVersion.Value}";
                    }
                }
                return gpuSet.DriverVersion;
            }
        }

        public static ICommand ConfigControlCenterHost { get; private set; } = new DelegateCommand(ControlCenterHostConfig.ShowWindow);

        public static ICommand ExportServerJson { get; private set; } = new DelegateCommand(() => {
            try {
                NTMinerRoot.ExportServerVersionJson(AssemblyInfo.ServerVersionJsonFileFullName);
                NotiCenterWindowViewModel.Instance.Manager.ShowSuccessMessage($"{AssemblyInfo.ServerJsonFileName}", "导出成功");
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        });

        public static string ServerJsonFileName { get; private set; } = AssemblyInfo.ServerJsonFileName;

        public static ICommand SetServerJsonVersion { get; private set; } = new DelegateCommand(() => {
            try {
                DialogWindow.ShowDialog(message: $"您确定刷新{AssemblyInfo.ServerJsonFileName}吗？", title: "确认", onYes: () => {
                    try {
                        VirtualRoot.Execute(new ChangeServerAppSettingCommand(new AppSettingData {
                            Key = ServerJsonFileName,
                            Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")
                        }));
                        NotiCenterWindowViewModel.Instance.Manager.ShowSuccessMessage($"刷新成功");
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        NotiCenterWindowViewModel.Instance.Manager.ShowErrorMessage($"刷新失败");
                    }
                }, icon: IconConst.IconConfirm);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        });

        public static ICommand ShowUsers { get; private set; } = new DelegateCommand(UserPage.ShowWindow);

        public static ICommand ShowOverClockDatas { get; private set; } = new DelegateCommand(OverClockDataPage.ShowWindow);

        public static ICommand ShowChartsWindow { get; private set; } = new DelegateCommand(ChartsWindow.ShowWindow);

        public static ICommand ShowInnerProperty { get; private set; } = new DelegateCommand(InnerProperty.ShowWindow);

        public static ICommand JoinQQGroup { get; private set; } = new DelegateCommand(() => {
            Process.Start("https://jq.qq.com/?_wv=1027&k=5ZPsuCk");
        });

        public static ICommand RunAsAdministrator { get; private set; } = new DelegateCommand(AppHelper.RunAsAdministrator);

        public static ICommand ShowNotificationSample { get; private set; } = new DelegateCommand(NotificationSample.ShowWindow);

        public static ICommand AppExit { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new CloseNTMinerCommand());
        });

        public static ICommand ShowRestartWindows { get; private set; } = new DelegateCommand(RestartWindows.ShowDialog);

        public static ICommand ShowVirtualMemory { get; private set; } = new DelegateCommand(Views.Ucs.VirtualMemory.ShowWindow);

        public static ICommand ShowSysDic { get; private set; } = new DelegateCommand(SysDicPage.ShowWindow);
        public static ICommand ShowGroups { get; private set; } = new DelegateCommand(GroupPage.ShowWindow);
        public static ICommand ShowCoins { get; private set; } = new DelegateCommand<CoinViewModel>((currentCoin) => {
            CoinPage.ShowWindow(currentCoin, "coin");
        });
        public static ICommand ManageColumnsShows { get; private set; } = new DelegateCommand(ColumnsShowPage.ShowWindow);
        public static ICommand ManagePools { get; private set; } = new DelegateCommand<CoinViewModel>(coinVm => {
            CoinPage.ShowWindow(coinVm, "pool");
        });
        public static ICommand ManageWallet { get; private set; } = new DelegateCommand<CoinViewModel>(coinVm => {
            CoinPage.ShowWindow(coinVm, "wallet");
        });
        public static ICommand ShowKernelInputs { get; private set; } = new DelegateCommand(KernelInputPage.ShowWindow);
        public static ICommand ShowKernelOutputs { get; private set; } = new DelegateCommand<KernelOutputViewModel>(KernelOutputPage.ShowWindow);
        public static ICommand ShowKernels { get; private set; } = new DelegateCommand(() => {
            KernelsWindow.ShowWindow(Guid.Empty);
        });
        public static ICommand ShowAbout { get; private set; } = new DelegateCommand<string>(AboutPage.ShowWindow);
        public static ICommand ShowSpeedChart { get; private set; } = new DelegateCommand(() => {
            SpeedCharts.ShowWindow();
        });
        public static ICommand ShowNTMinerUpdaterConfig { get; private set; } = new DelegateCommand(NTMinerUpdaterConfig.ShowWindow);
        public static ICommand ShowOnlineUpdate { get; private set; } = new DelegateCommand(() => {
            Upgrade(string.Empty, null);
        });
        public static ICommand ShowHelp { get; private set; } = new DelegateCommand(() => {
            Process.Start("https://github.com/ntminer/ntminer");
        });
        public static ICommand ShowMinerClients { get; private set; } = new DelegateCommand(() => {
            MinerClientsWindow.ShowWindow();
        });
        public static ICommand ShowCalcConfig { get; private set; } = new DelegateCommand(CalcConfig.ShowWindow);
        public static ICommand ShowGlobalDir { get; private set; } = new DelegateCommand(() => {
            Process.Start(VirtualRoot.GlobalDirFullName);
        });
        public static ICommand OpenLocalLiteDb { get; private set; } = new DelegateCommand(() => {
            OpenLiteDb(SpecialPath.LocalDbFileFullName);
        });
        public static ICommand OpenServerLiteDb { get; private set; } = new DelegateCommand(() => {
            OpenLiteDb(SpecialPath.ServerDbFileFullName);
        });

        private static void OpenLiteDb(string dbFileFullName) {
            string liteDbExplorerDir = Path.Combine(VirtualRoot.GlobalDirFullName, "LiteDBExplorerPortable");
            string liteDbExplorerFileFullName = Path.Combine(liteDbExplorerDir, "LiteDbExplorer.exe");
            if (!Directory.Exists(liteDbExplorerDir)) {
                Directory.CreateDirectory(liteDbExplorerDir);
            }
            if (!File.Exists(liteDbExplorerFileFullName)) {
                OfficialServer.FileUrlService.GetLiteDbExplorerUrlAsync((downloadFileUrl, e) => {
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

        public static ICommand ShowCalc { get; private set; } = new DelegateCommand<CoinViewModel>(Calc.ShowWindow);

        public static ICommand OpenOfficialSite { get; private set; } = new DelegateCommand(() => {
            Process.Start("https://github.com/ntminer/ntminer");
        });

        public static ICommand OpenDiscussSite { get; private set; } = new DelegateCommand(() => {
            Process.Start("https://github.com/ntminer/ntminer/issues");
        });

        public static ICommand DownloadMinerStudio { get; private set; } = new DelegateCommand(() => {
            Process.Start(AssemblyInfo.MinerJsonBucket + "MinerStudio.exe?t=" + DateTime.Now.Ticks);
        });

        public static ICommand ShowQQGroupQrCode { get; private set; } = new DelegateCommand(QQGroupQrCode.ShowWindow);
    }
}
