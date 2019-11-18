using NTMiner.Core;
using NTMiner.MinerClient;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NTMiner {
    // 注意：这里的成员只应用于绑定，不应在.cs中使用，在IDE中看到的静态源代码应用计数应为0
    public static class AppStatic {
        public static readonly BitmapImage BigLogoImageSource = new BitmapImage(new Uri((VirtualRoot.IsMinerStudio ? "/NTMinerWpf;component/Styles/Images/cc128.png" : "/NTMinerWpf;component/Styles/Images/logo128.png"), UriKind.RelativeOrAbsolute));

        private static string GetUpdaterVersion() {
            string updaterVersion = string.Empty;
            if (VirtualRoot.LocalAppSettingSet.TryGetAppSetting(NTKeyword.UpdaterVersionAppSettingKey, out IAppSetting setting) && setting.Value != null) {
                updaterVersion = setting.Value.ToString();
            }
            return updaterVersion;
        }

        private static void SetUpdaterVersion(string value) {
            VirtualRoot.Execute(new SetLocalAppSettingCommand(new AppSettingData {
                Key = NTKeyword.UpdaterVersionAppSettingKey,
                Value = value
            }));
        }

        #region Upgrade
        public static void Upgrade(string fileName, Action callback) {
            try {
                OfficialServer.FileUrlService.GetNTMinerUpdaterUrlAsync((downloadFileUrl, e) => {
                    try {
                        string argument = string.Empty;
                        if (!string.IsNullOrEmpty(fileName)) {
                            argument = "ntminerFileName=" + fileName;
                        }
                        if (VirtualRoot.IsMinerStudio) {
                            argument += " --minerstudio";
                        }
                        if (string.IsNullOrEmpty(downloadFileUrl)) {
                            if (File.Exists(SpecialPath.UpdaterFileFullName)) {
                                Windows.Cmd.RunClose(SpecialPath.UpdaterFileFullName, argument);
                            }
                            callback?.Invoke();
                            return;
                        }
                        Uri uri = new Uri(downloadFileUrl);
                        string updaterVersion = GetUpdaterVersion();
                        if (string.IsNullOrEmpty(updaterVersion) || !File.Exists(SpecialPath.UpdaterFileFullName) || uri.AbsolutePath != updaterVersion) {
                            VirtualRoot.Execute(new ShowFileDownloaderCommand(downloadFileUrl, "开源矿工更新器", (window, isSuccess, message, saveFileFullName) => {
                                try {
                                    if (isSuccess) {
                                        File.Copy(saveFileFullName, SpecialPath.UpdaterFileFullName, overwrite: true);
                                        File.Delete(saveFileFullName);
                                        SetUpdaterVersion(uri.AbsolutePath);
                                        window?.Close();
                                        Windows.Cmd.RunClose(SpecialPath.UpdaterFileFullName, argument);
                                        callback?.Invoke();
                                    }
                                    else {
                                        VirtualRoot.ThisLocalError(nameof(AppStatic), message, toConsole: true);
                                        callback?.Invoke();
                                    }
                                }
                                catch {
                                    callback?.Invoke();
                                }
                            }));
                        }
                        else {
                            Windows.Cmd.RunClose(SpecialPath.UpdaterFileFullName, argument);
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

        #region IsWin10
        public static bool IsGEWin10 {
            get { return VirtualRoot.IsGEWin10; }
        }
        public static bool IsLTWin10 {
            get { return VirtualRoot.IsLTWin10; }
        }
        public static Visibility IsGEWin10Visible {
            get {
                if (VirtualRoot.IsGEWin10) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        public static Visibility IsLTWin10Visible {
            get {
                if (VirtualRoot.IsLTWin10) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        #endregion

        #region InnerProperty
        public static string Id {
            get { return VirtualRoot.Id.ToString(); }
        }
        public static string BootOn {
            get => NTMinerRoot.Instance.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static string HomeDir {
            get => MainAssemblyInfo.HomeDirFullName;
        }
        public static string TempDir {
            get { return MainAssemblyInfo.TempDirFullName; }
        }
        public static string ServerDbFileFullName {
            get {
                return SpecialPath.ServerDbFileFullName.Replace(HomeDir, NTKeyword.HomeDirParameterName);
            }
        }
        public static string LocalDbFileFullName {
            get => VirtualRoot.LocalDbFileFullName.Replace(HomeDir, NTKeyword.HomeDirParameterName);
        }

        public static string ServerJsonFileFullName {
            get { return SpecialPath.ServerJsonFileFullName.Replace(HomeDir, NTKeyword.HomeDirParameterName); }
        }

        public static string ServerVersionJsonFileFullName {
            get { return MainAssemblyInfo.ServerVersionJsonFileFullName.Replace(HomeDir, NTKeyword.HomeDirParameterName); }
        }

        public static string PackagesDirFullName {
            get { return SpecialPath.PackagesDirFullName.Replace(HomeDir, NTKeyword.HomeDirParameterName); }
        }

        public static string DaemonFileFullName {
            get { return SpecialPath.DaemonFileFullName.Replace(TempDir, NTKeyword.TempDirParameterName); }
        }

        public static string DevConsoleFileFullName {
            get { return SpecialPath.DevConsoleFileFullName.Replace(TempDir, NTKeyword.TempDirParameterName); }
        }

        public static string DownloadDirFullName {
            get {
                return SpecialPath.DownloadDirFullName.Replace(TempDir, NTKeyword.TempDirParameterName);
            }
        }

        public static string KernelsDirFullName {
            get { return SpecialPath.KernelsDirFullName.Replace(TempDir, NTKeyword.TempDirParameterName); }
        }

        public static string LogsDirFullName {
            get { return SpecialPath.LogsDirFullName.Replace(TempDir, NTKeyword.TempDirParameterName); }
        }

        public static string AppRuntime {
            get {
                if (VirtualRoot.IsMinerStudio) {
                    return "群控客户端";
                }
                else if (VirtualRoot.IsMinerClient) {
                    return "挖矿端";
                }
                return "未知";
            }
        }
        #endregion

        #region IsMinerClient
        public static bool IsMinerClient {
            get => VirtualRoot.IsMinerClient;
        }

        public static Visibility IsMinerClientVisible {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return Visibility.Visible;
                }
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
                if (WpfUtil.IsInDesignMode) {
                    return Visibility.Visible;
                }
                if (VirtualRoot.IsMinerStudio) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public static Visibility IsMinerStudioDevVisible {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return Visibility.Visible;
                }
                if (!DevMode.IsDevMode) {
                    return Visibility.Collapsed;
                }
                if (VirtualRoot.IsMinerStudio) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        #endregion

        #region IsDev
        public static bool IsDevMode {
            get {
                return WpfUtil.IsDevMode;
            }
        }

        public static bool IsNotDevMode => !WpfUtil.IsDevMode;

        public static Visibility IsDevModeVisible {
            get {
                if (WpfUtil.IsDevMode) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        #endregion

        #region IsAmd
        public static Visibility IsAmdGpuVisible {
            get {
                if (NTMinerRoot.Instance.GpuSet.GpuType == GpuType.AMD) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public static bool IsAmdGpu {
            get {
                return NTMinerRoot.Instance.GpuSet.GpuType == GpuType.AMD;
            }
        }
        #endregion

        #region IsBrand
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
        #endregion

        #region MainWindowHeight MainWindowWidth
        public static double MainWindowHeight {
            get {
                if (SystemParameters.WorkArea.Size.Height >= 620) {
                    return 620;
                }
                else if (SystemParameters.WorkArea.Size.Height >= 520) {
                    return 520;
                }
                return 480;
            }
        }

        public static double MainWindowWidth {
            get {
                if (SystemParameters.WorkArea.Size.Width >= 1090) {
                    return 1090;
                }
                else if (SystemParameters.WorkArea.Size.Width >= 1000) {
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
        #endregion

        #region EnumItems
        public static IEnumerable<EnumItem<SupportedGpu>> SupportedGpuEnumItems {
            get {
                return NTMinerRoot.SupportedGpuEnumItems;
            }
        }

        public static IEnumerable<EnumItem<GpuType>> GpuTypeEnumItems {
            get {
                return NTMinerRoot.GpuTypeEnumItems;
            }
        }

        public static IEnumerable<EnumItem<PublishStatus>> PublishStatusEnumItems {
            get {
                return NTMinerRoot.PublishStatusEnumItems;
            }
        }

        public static IEnumerable<EnumItem<MineStatus>> MineStatusEnumItems {
            get {
                return NTMinerRoot.MineStatusEnumItems;
            }
        }

        public static IEnumerable<EnumItem<ServerMessageType>> ServerMessageTypeEnumItems {
            get {
                return NTMinerRoot.ServerMessageTypeEnumItems;
            }
        }

        public static IEnumerable<EnumItem<LocalMessageType>> LocalMessageTypeEnumItems {
            get { return NTMinerRoot.LocalMessageTypeEnumItems; }
        }
        #endregion

        #region AppName CurrentVersion VersionTag VersionFullName
        public static string AppName {
            get {
                return VirtualRoot.AppName;
            }
        }

        public static string CurrentVersion {
            get {
                return MainAssemblyInfo.CurrentVersion.ToString();
            }
        }

        public static string VersionTag {
            get {
                return MainAssemblyInfo.CurrentVersionTag;
            }
        }

        public static string VersionFullName {
            get {
                return $"v{MainAssemblyInfo.CurrentVersion}({VersionTag})";
            }
        }
        #endregion

        #region Gpu
        public static Version MinAmdDriverVersion {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return new Version();
                }
                if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "MinAmdDriverVersion", out ISysDicItem dicItem)) {
                    if (Version.TryParse(dicItem.Value, out Version version)) {
                        return version;
                    }
                }
                return new Version(17, 10, 2);
            }
        }

        public static Version MinNvidiaDriverVersion {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return new Version();
                }
                if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "MinNvidiaDriverVersion", out ISysDicItem dicItem)) {
                    if (Version.TryParse(dicItem.Value, out Version version)) {
                        return version;
                    }
                }
                return new Version(399, 24);
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
                    var cudaVersion = gpuSet.Properties.FirstOrDefault(a => a.Code == NTKeyword.CudaVersionSysDicCode);
                    if (cudaVersion != null) {
                        return $"{gpuSet.DriverVersion}_{cudaVersion.Value}";
                    }
                }
                return gpuSet.DriverVersion.ToString();
            }
        }

        public static SolidColorBrush DriverVersionColor {
            get {
                var gpuSet = NTMinerRoot.Instance.GpuSet;
                switch (gpuSet.GpuType) {
                    case GpuType.NVIDIA:
                        if (gpuSet.DriverVersion < MinNvidiaDriverVersion) {
                            return WpfUtil.RedBrush;
                        }
                        break;
                    case GpuType.AMD:
                        if (gpuSet.DriverVersion < MinAmdDriverVersion) {
                            return WpfUtil.RedBrush;
                        }
                        break;
                }
                return (SolidColorBrush)Application.Current.Resources["LableColor"];
            }
        }

        public static string DriverVersionToolTip {
            get {
                var gpuSet = NTMinerRoot.Instance.GpuSet;
                bool isTooLow = false;
                switch (gpuSet.GpuType) {
                    case GpuType.NVIDIA:
                        if (gpuSet.DriverVersion < MinNvidiaDriverVersion) {
                            isTooLow = true;
                        }
                        break;
                    case GpuType.AMD:
                        if (gpuSet.DriverVersion < MinAmdDriverVersion) {
                            isTooLow = true;
                        }
                        break;
                }
                if (isTooLow) {
                    return "显卡驱动版本较低，工具箱里有驱动下载地址";
                }
                return "显卡驱动版本";
            }
        }
        #endregion

        #region Windows
        private static readonly string _windowsEdition = Windows.OS.Instance.WindowsEdition?.Replace("Windows ", "Win");
        public static string WindowsEdition {
            get {
                return _windowsEdition;
            }
        }

        public const string LowWinMessage = "Windows版本较低，建议使用Win10系统";
        public static string WindowsEditionToolTip {
            get {
                // Win7下WinDivert.sys文件签名问题
                if (VirtualRoot.IsLTWin10) {
                    return LowWinMessage;
                }
                return "操作系统";
            }
        }

        public static SolidColorBrush WindowsEditionColor {
            get {
                // Win7下WinDivert.sys文件签名问题
                if (VirtualRoot.IsLTWin10) {
                    return WpfUtil.RedBrush;
                }
                return (SolidColorBrush)Application.Current.Resources["LableColor"];
            }
        }

        public static string TotalVirtualMemoryGbText {
            get {
                return AppContext.Instance.VirtualMemorySetVm.TotalVirtualMemoryGbText;
            }
        }
        #endregion

        public static ICommand ShowServerKernelOutputKeywords { get; private set; } = new DelegateCommand(() => {

        });

        public static ICommand ShowIcons { get; private set; } = new DelegateCommand(() => {
            Views.Ucs.Icons.ShowWindow();
        });

        public static ICommand OpenDir { get; private set; } = new DelegateCommand<string>((dir) => {
            if (dir.StartsWith(NTKeyword.TempDirParameterName)) {
                dir = dir.Replace(NTKeyword.TempDirParameterName, MainAssemblyInfo.TempDirFullName);
            }
            else if (dir.StartsWith(NTKeyword.HomeDirParameterName)) {
                dir = dir.Replace(NTKeyword.HomeDirParameterName, MainAssemblyInfo.HomeDirFullName);
            }
            Process.Start(dir);
        });

        public static ICommand ViewUrl { get; private set; } = new DelegateCommand<string>(url => {
            if (string.IsNullOrEmpty(url)) {
                return;
            }
            Process.Start(url);
        });

        public static string ExportServerJsonMenuName {
            get {
                return "导出" + MainAssemblyInfo.ServerJsonFileName;
            }
        }

        public static ICommand ExportServerJson { get; private set; } = new DelegateCommand(() => {
            try {
                NTMinerRoot.ExportServerVersionJson(MainAssemblyInfo.ServerVersionJsonFileFullName);
                VirtualRoot.Out.ShowSuccess($"{MainAssemblyInfo.ServerJsonFileName}", "导出成功");
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        });

        public static string ServerJsonFileName { get; private set; } = MainAssemblyInfo.ServerJsonFileName;

        public static ICommand SetServerJsonVersion { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowDialogWindowCommand(message: $"您确定刷新{MainAssemblyInfo.ServerJsonFileName}吗？", title: "确认", onYes: () => {
                try {
                    VirtualRoot.Execute(new SetServerAppSettingCommand(new AppSettingData {
                        Key = MainAssemblyInfo.ServerJsonFileName,
                        Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")
                    }));
                    VirtualRoot.Out.ShowSuccess($"刷新成功");
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    VirtualRoot.Out.ShowError($"刷新失败");
                }
            }));
        });

        public static ICommand ShowLocalMessagesConfig { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowKernelOutputKeywordsCommand());
        });

        public static ICommand ShowMessagePathIds { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowMessagePathIdsCommand());
        });

        public static ICommand ShowUsers { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowUserPageCommand());
        });

        public static ICommand ShowOverClockDatas { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowOverClockDataPageCommand());
        });

        public static ICommand ShowNTMinerWallets { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowNTMinerWalletPageCommand());
        });

        public static ICommand ShowChartsWindow { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowChartsWindowCommand());
        });

        public static ICommand ShowProperty { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowPropertyCommand());
        });

        public static ICommand JoinQQGroup { get; private set; } = new DelegateCommand(() => {
            string url = "https://jq.qq.com/?_wv=1027&k=5ZPsuCk";
            if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "QQGroupJoinUrl", out ISysDicItem dicItem)) {
                url = dicItem.Value;
            }
            Process.Start(url);
        });

        public static ICommand RunAsAdministrator { get; private set; } = new DelegateCommand(WpfUtil.RunAsAdministrator);

        public static ICommand ShowNotificationSample { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowNotificationSampleCommand());
        });

        public static ICommand AppExit { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new CloseNTMinerCommand("手动操作"));
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
        public static ICommand ShowTagBrand { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowTagBrandCommand());
        });
        public static ICommand ManagePools { get; private set; } = new DelegateCommand<CoinViewModel>(coinVm => {
            VirtualRoot.Execute(new ShowCoinPageCommand(coinVm, NTKeyword.PoolParameterName));
        });
        public static ICommand ManageWallet { get; private set; } = new DelegateCommand<CoinViewModel>(coinVm => {
            VirtualRoot.Execute(new ShowCoinPageCommand(coinVm, NTKeyword.WalletParameterName));
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
            string url = "http://ntminer.com/";
            if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "HelpUrl", out ISysDicItem dicItem)) {
                url = dicItem.Value;
            }
            Process.Start(url);
        });
        public static ICommand ShowMinerClients { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowMinerClientsWindowCommand());
        });
        public static ICommand ShowCalcConfig { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowCalcConfigCommand());
        });
        public static ICommand ShowHomeDir { get; private set; } = new DelegateCommand(() => {
            Process.Start(MainAssemblyInfo.HomeDirFullName);
        });
        public static ICommand OpenLocalLiteDb { get; private set; } = new DelegateCommand(() => {
            OpenLiteDb(VirtualRoot.LocalDbFileFullName);
        });
        public static ICommand OpenServerLiteDb { get; private set; } = new DelegateCommand(() => {
            OpenLiteDb(SpecialPath.ServerDbFileFullName);
        });

        #region private method OpenLiteDb
        private static void OpenLiteDb(string dbFileFullName) {
            string liteDbExplorerDir = Path.Combine(SpecialPath.ToolsDirFullName, "LiteDBExplorerPortable");
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
        #endregion

        public static ICommand OpenLogfile { get; private set; } = new DelegateCommand<string>((logfileFullName) => {
            OpenTxtFile(logfileFullName);
        });

        public static string NppPackageUrl {
            get {
                if (WpfUtil.IsDevMode) {
                    return "https://minerjson.oss-cn-beijing.aliyuncs.com/npp.zip";
                }
                if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem("Tool", "npp", out ISysDicItem dicItem)) {
                    return dicItem.Value;
                }
                return "https://minerjson.oss-cn-beijing.aliyuncs.com/npp.zip";
            }
        }

        #region private method OpenTxtFile
        private static void OpenTxtFile(string fileFullName) {
            string nppDir = Path.Combine(SpecialPath.ToolsDirFullName, "Npp");
            string nppFileFullName = Path.Combine(nppDir, "notepad++.exe");
            if (!Directory.Exists(nppDir)) {
                Directory.CreateDirectory(nppDir);
            }
            if (!File.Exists(nppFileFullName)) {
                VirtualRoot.Execute(new ShowFileDownloaderCommand(NppPackageUrl, "Notepad++", (window, isSuccess, message, saveFileFullName) => {
                    if (isSuccess) {
                        ZipUtil.DecompressZipFile(saveFileFullName, nppDir);
                        File.Delete(saveFileFullName);
                        window?.Close();
                        Windows.Cmd.RunClose(nppFileFullName, fileFullName);
                    }
                }));
            }
            else {
                Windows.Cmd.RunClose(nppFileFullName, fileFullName);
            }
        }
        #endregion

        public static ICommand ShowCalc { get; private set; } = new DelegateCommand<CoinViewModel>(coinVm => {
            VirtualRoot.Execute(new ShowCalcCommand(coinVm));
        });

        public static ICommand ShowLocalIps { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowLocalIpsCommand());
        });

        public static ICommand ShowEthNoDevFee { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowEthNoDevFeeCommand());
        });

        public static ICommand OpenOfficialSite { get; private set; } = new DelegateCommand(() => {
            string url = "http://ntminer.com/";
            if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "HomePageUrl", out ISysDicItem dicItem)) {
                url = dicItem.Value;
            }
            Process.Start(url);
        });

        public static string QQGroup {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return string.Empty;
                }
                if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "QQGroup", out ISysDicItem dicItem)) {
                    return dicItem.Value;
                }
                return "863725136";
            }
        }

        public static string OfficialSiteName {
            get {
                if (WpfUtil.IsDevMode) {
                    return "NTMiner.com";
                }
                if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "HomePageUrl", out ISysDicItem dicItem) && !string.IsNullOrEmpty(dicItem.Value)) {
                    if (dicItem.Value.StartsWith("https://")) {
                        return dicItem.Value.Substring("https://".Length);
                    }
                    if (dicItem.Value.StartsWith("http://")) {
                        return dicItem.Value.Substring("http://".Length);
                    }
                }
                return "NTMiner.com";
            }
        }

        public static string AppMinerName {
            get {
                if (WpfUtil.IsDevMode) {
                    return "开源矿工";
                }
                if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "AppMinerName", out ISysDicItem dicItem)) {
                    return dicItem.Value;
                }
                return "开源矿工";
            }
        }

        public static string AppMinerDescription {
            get {
                if (WpfUtil.IsDevMode) {
                    return " - 做最好的矿工";
                }
                if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "AppMinerName", out ISysDicItem dicItem)) {
                    return " - " + dicItem.Description;
                }
                return " - 做最好的矿工";
            }
        }

        public static string AppMinerIntro {
            get {
                if (WpfUtil.IsDevMode) {
                    return "开源、开放、安全、专业、最高收益。QQ群863725136";
                }
                if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "AppMinerIntro", out ISysDicItem dicItem)) {
                    return dicItem.Value;
                }
                return "开源、开放、安全、专业、最高收益。QQ群863725136";
            }
        }

        public static ICommand BusinessModel { get; private set; } = new DelegateCommand(() => {
            string url = "https://www.loserhub.cn/posts/details/52";
            if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "BusinessModelUrl", out ISysDicItem dicItem)) {
                url = dicItem.Value;
            }
            Process.Start(url);
        });

        public static ICommand OpenGithub { get; private set; } = new DelegateCommand(() => {
            string url = "https://github.com/ntminer/ntminer";
            if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "GithubUrl", out ISysDicItem dicItem)) {
                url = dicItem.Value;
            }
            Process.Start(url);
        });

        public static ICommand OpenDiscussSite { get; private set; } = new DelegateCommand(() => {
            string url = "https://github.com/ntminer/ntminer/issues";
            if (NTMinerRoot.Instance.ServerContext.SysDicItemSet.TryGetDicItem(NTKeyword.ThisSystemSysDicCode, "DiscussUrl", out ISysDicItem dicItem)) {
                url = dicItem.Value;
            }
            Process.Start(url);
        });

        public static ICommand DownloadMinerStudio { get; private set; } = new DelegateCommand(() => {
            Process.Start($"{OfficialServer.MinerJsonBucket}MinerStudio.exe?t={DateTime.Now.Ticks}");
        });

        public static ICommand ShowQQGroupQrCode { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowQQGroupQrCodeCommand());
        });
    }
}
