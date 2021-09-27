using NTMiner.Core;
using NTMiner.Core.MinerServer;
using NTMiner.Gpus;
using NTMiner.MinerStudio;
using NTMiner.User;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NTMiner {
    /// <summary>
    /// 注意：这里的成员只应用于绑定且不能在SplashWindow中绑定，因为SplashWindow是在一个单独的UI线程运行的而AppStatic是为给主UI线程用的;
    /// 不应在.cs中使用，在IDE中看到的静态源代码引用计数应全为0，因为这里的数据都是用于展示的而不是为了业务逻辑的，比如ExportServerJsonFileFullName
    /// 这个属性的取值可能很长所以其中的HomePath前缀部分会被{家目录}变量取代，所以不能当作是个路径了。
    /// </summary>
    public static class AppStatic {
        private static readonly Lazy<BitmapImage> _bigLogoImageSource = new Lazy<BitmapImage>(() => {
            return new BitmapImage(new Uri((ClientAppType.IsMinerStudio ? "/NTMinerWpf;component/Styles/Images/cc128.png" : "/NTMinerWpf;component/Styles/Images/logo128.png"), UriKind.RelativeOrAbsolute));
        });

        public static BitmapImage BigLogoImageSource {
            get {
                return _bigLogoImageSource.Value;
            }
        }

        public static List<SpeedUnitViewModel> SpeedUnitVms { get; private set; } = new List<SpeedUnitViewModel> {
            SpeedUnitViewModel.HPerSecond,
            SpeedUnitViewModel.KhPerSecond,
            SpeedUnitViewModel.MhPerSecond,
            SpeedUnitViewModel.GhPerSecond,
            SpeedUnitViewModel.ThPerSecond
        };

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
            get { return NTMinerContext.Id.ToString(); }
        }
        public static string BootOn {
            get => NTMinerContext.Instance.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 为了让IDE显式的引用计数为0该文件里其它地方直接使用<see cref="EntryAssemblyInfo.HomeDirFullName"/>
        /// </summary>
        public static string HomeDir {
            get => HomePath.HomeDirFullName;
        }
        /// <summary>
        /// 为了让IDE显式的引用计数为0该文件里其它地方直接使用<see cref="EntryAssemblyInfo.TempDirFullName"/>
        /// </summary>
        public static string TempDir {
            get { return TempPath.TempDirFullName; }
        }
        public static string ServerDbFileFullName {
            get {
                return HomePath.ServerDbFileFullName.Replace(HomePath.HomeDirFullName, NTKeyword.HomeDirParameterName);
            }
        }
        public static string LocalDbFileFullName {
            get => HomePath.LocalDbFileFullName.Replace(HomePath.HomeDirFullName, NTKeyword.HomeDirParameterName);
        }

        public static string ServerJsonFileFullName {
            get { return HomePath.ServerJsonFileFullName.Replace(HomePath.HomeDirFullName, NTKeyword.HomeDirParameterName); }
        }

        public static string ExportServerJsonFileFullName {
            get { return HomePath.ExportServerJsonFileFullName.Replace(HomePath.HomeDirFullName, NTKeyword.HomeDirParameterName); }
        }

        public static string PackagesDirFullName {
            get { return HomePath.PackagesDirFullName.Replace(HomePath.HomeDirFullName, NTKeyword.HomeDirParameterName); }
        }

        public static string DaemonFileFullName {
            get { return MinerClientTempPath.DaemonFileFullName.Replace(TempPath.TempDirFullName, NTKeyword.TempDirParameterName); }
        }

        public static string DevConsoleFileFullName {
            get { return MinerClientTempPath.DevConsoleFileFullName.Replace(TempPath.TempDirFullName, NTKeyword.TempDirParameterName); }
        }

        public static string DownloadDirFullName {
            get {
                return MinerClientTempPath.DownloadDirFullName.Replace(TempPath.TempDirFullName, NTKeyword.TempDirParameterName);
            }
        }

        public static string KernelsDirFullName {
            get { return MinerClientTempPath.KernelsDirFullName.Replace(TempPath.TempDirFullName, NTKeyword.TempDirParameterName); }
        }

        public static string LogsDirFullName {
            get {
                if (ClientAppType.IsMinerClient) {
                    return TempPath.TempLogsDirFullName.Replace(TempPath.TempDirFullName, NTKeyword.TempDirParameterName);
                }
                return HomePath.HomeLogsDirFullName.Replace(HomePath.HomeDirFullName, NTKeyword.HomeDirParameterName);
            }
        }

        public static string AppRuntime {
            get {
                if (ClientAppType.IsMinerStudio) {
                    return "群控客户端";
                }
                else if (ClientAppType.IsMinerClient) {
                    return "挖矿端";
                }
                return "未知";
            }
        }
        #endregion

        #region IsMinerClient
        /// <summary>
        /// 为了让IDE显式的引用计数为0该文件里其它地方直接使用<see cref="MinerClientTempPath.IsMinerClient"/>
        /// </summary>
        public static bool IsMinerClient {
            get => ClientAppType.IsMinerClient;
        }

        public static Visibility IsMinerClientVisible {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return Visibility.Visible;
                }
                if (ClientAppType.IsMinerClient) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 为了让IDE显式的引用计数为0该文件里其它地方直接使用<see cref="MinerClientTempPath.IsMinerStudio"/>
        /// </summary>
        public static bool IsMinerStudio {
            get => ClientAppType.IsMinerStudio;
        }

        public static Visibility IsMinerStudioVisible {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return Visibility.Visible;
                }
                if (ClientAppType.IsMinerStudio) {
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
                if (ClientAppType.IsMinerStudio) {
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

        public static bool IsNotDevMode {
            get {
                return !WpfUtil.IsDevMode;
            }
        }

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
                if (WpfUtil.IsInDesignMode) {
                    return Visibility.Visible;
                }
                if (NTMinerContext.Instance.GpuSet.GpuType.IsAmd()) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public static bool IsHasATIGpu {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return true;
                }
                return AdlHelper.IsHasATIGpu;
            }
        }
        #endregion

        public static Visibility IsMixedGpuVisible {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return Visibility.Visible;
                }
                return NTMinerContext.Instance.GpuSet.GpuType.IsNvidia() && AdlHelper.IsHasATIGpu ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #region IsBrand
        public static bool IsPoolBrand {
            get {
                return NTMinerContext.IsPoolBrand;
            }
        }

        public static Visibility IsPoolBrandVisible {
            get {
                return NTMinerContext.IsPoolBrand ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public static Visibility IsPoolBrandCollapsed {
            get { return NTMinerContext.IsPoolBrand ? Visibility.Collapsed : Visibility.Visible; }
        }

        public static bool IsKernelBrand {
            get {
                return NTMinerContext.IsKernelBrand;
            }
        }

        public static Visibility IsKernelBrandVisible {
            get {
                return NTMinerContext.IsKernelBrand ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public static Visibility IsKernelBrandCollapsed {
            get { return NTMinerContext.IsKernelBrand ? Visibility.Collapsed : Visibility.Visible; }
        }

        public static bool IsBrandSpecified {
            get { return NTMinerContext.IsBrandSpecified; }
        }

        public static Visibility IsBrandSpecifiedVisible {
            get {
                return NTMinerContext.IsBrandSpecified ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public static Visibility IsBrandSpecifiedCollapsed {
            get { return NTMinerContext.IsBrandSpecified ? Visibility.Collapsed : Visibility.Visible; }
        }
        #endregion

        #region MainWindowHeight MainWindowWidth
        public static double MainWindowHeight {
            get {
                return AppRoot.MainWindowHeight;
            }
        }

        public static double MainWindowWidth {
            get {
                return AppRoot.MainWindowWidth;
            }
        }
        #endregion

        #region EnumItems
        public static IEnumerable<EnumItem<SupportedGpu>> SupportedGpuEnumItems {
            get {
                return Enums.SupportedGpuEnumItems;
            }
        }

        public static IEnumerable<EnumItem<GpuType>> GpuTypeEnumItems {
            get {
                return Enums.GpuTypeEnumItems;
            }
        }

        public static IEnumerable<EnumItem<PublishStatus>> PublishStatusEnumItems {
            get {
                return Enums.PublishStatusEnumItems;
            }
        }

        public static IEnumerable<EnumItem<MineStatus>> MineStatusEnumItems {
            get {
                return Enums.MineStatusEnumItems;
            }
        }

        public static IEnumerable<EnumItem<UserStatus>> UserStatusEnumItems {
            get {
                return Enums.UserStatusEnumItems;
            }
        }

        public static IEnumerable<EnumItem<ServerMessageType>> ServerMessageTypeEnumItems {
            get {
                return Enums.ServerMessageTypeEnumItems;
            }
        }

        public static IEnumerable<EnumItem<LocalMessageType>> LocalMessageTypeEnumItems {
            get { return Enums.LocalMessageTypeEnumItems; }
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
                return EntryAssemblyInfo.CurrentVersionStr;
            }
        }

        /// <summary>
        /// 为了让IDE显式的引用计数为0该文件里其它地方直接使用<see cref="EntryAssemblyInfo.CurrentVersionTag"/>
        /// </summary>
        public static string VersionTag {
            get {
                return EntryAssemblyInfo.CurrentVersionTag;
            }
        }

        public static string VersionFullName {
            get {
                return $"v{EntryAssemblyInfo.CurrentVersionStr}({EntryAssemblyInfo.CurrentVersionTag})";
            }
        }
        #endregion

        #region Gpu
        public static Version MinAmdDriverVersion {
            get {
                return NTMinerContext.Instance.MinAmdDriverVersion;
            }
        }

        public static Version MinNvidiaDriverVersion {
            get {
                return NTMinerContext.Instance.MinNvidiaDriverVersion;
            }
        }

        public static string GpuSetInfo {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return "p106-100 x 8";
                }
                return NTMinerContext.Instance.GpuSetInfo;
            }
        }

        public static string DriverVersion {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return "0.0";
                }
                var gpuSet = NTMinerContext.Instance.GpuSet;
                return gpuSet.DriverVersion;
            }
        }

        public static SolidColorBrush DriverVersionColor {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return WpfUtil.RedBrush;
                }
                var gpuSet = NTMinerContext.Instance.GpuSet;
                if (gpuSet.IsLowDriverVersion) {
                    return WpfUtil.RedBrush;
                }
                return AppUtil.GetResource<SolidColorBrush>("LableColor");
            }
        }

        public static string DriverVersionToolTip {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return string.Empty;
                }
                var gpuSet = NTMinerContext.Instance.GpuSet;
                if (gpuSet.IsLowDriverVersion) {
                    return "显卡驱动版本较低，工具箱里有驱动下载地址";
                }
                return "显卡驱动版本";
            }
        }
        #endregion

        #region Windows
        public static string WindowsEdition {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return "WindowsEdition";
                }
                return Windows.OS.Instance.WindowsEdition?.Replace("Windows ", "Win");
            }
        }

        public static string WindowsEditionToolTip {
            get {
                // Win7下WinDivert.sys文件签名问题
                if (VirtualRoot.IsLTWin10) {
                    return AppRoot.LowWinMessage;
                }
                return "操作系统";
            }
        }

        public static SolidColorBrush WindowsEditionColor {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return WpfUtil.RedBrush;
                }
                // Win7下WinDivert.sys文件签名问题
                if (VirtualRoot.IsLTWin10) {
                    return WpfUtil.RedBrush;
                }
                return AppUtil.GetResource<SolidColorBrush>("LableColor");
            }
        }

        // 因为虚拟内存修改后重启电脑才会生效所以这里用静态绑定没有问题
        public static string TotalVirtualMemoryGbText {
            get {
                return (VirtualRoot.DriveSet.OSVirtualMemoryMb / NTKeyword.DoubleK).ToString("f1") + "G";
            }
        }
        #endregion

        public static ICommand ExtractCosturaCompressedDlls { get; private set; } = new DelegateCommand(() => {
            CosturaUtil.ExtractCosturaCompressedDlls();
        });

        public static ICommand WindowsProperty { get; private set; } = new DelegateCommand(() => {
            Process.Start("control.exe", "system");
        });

        public static ICommand ShowSignUpPage { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowSignUpPageCommand());
        });

        public static ICommand ShowIcons { get; private set; } = new DelegateCommand(() => {
            Views.Ucs.Icons.ShowWindow();
        });

        #region OpenMinerClientFinder
        public static ICommand OpenMinerClientFinder { get; private set; } = new DelegateCommand(() => {
            AppRoot.OpenMinerClientFinder();
        });
        #endregion

        public static ICommand OpenDir { get; private set; } = new DelegateCommand<string>((dir) => {
            if (dir.StartsWith(NTKeyword.TempDirParameterName)) {
                dir = dir.Replace(NTKeyword.TempDirParameterName, TempPath.TempDirFullName);
            }
            else if (dir.StartsWith(NTKeyword.HomeDirParameterName)) {
                dir = dir.Replace(NTKeyword.HomeDirParameterName, HomePath.HomeDirFullName);
            }
            Process.Start(dir);
        });

        public static ICommand ViewUrl { get; private set; } = new DelegateCommand<string>(url => {
            if (string.IsNullOrEmpty(url)) {
                return;
            }
            VirtualRoot.Execute(new UnTopmostCommand());
            Process.Start(url);
        });

        public static string ExportServerJsonMenuName {
            get {
                return "导出" + HomePath.ExportServerJsonFileName;
            }
        }

        public static ICommand ExportServerJson { get; private set; } = new DelegateCommand(() => {
            try {
                NTMinerContext.Instance.ExportServerVersionJson(HomePath.ExportServerJsonFileFullName);
                VirtualRoot.Out.ShowSuccess($"{HomePath.ExportServerJsonFileName}", header: "导出成功");
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        });

        public static string ExportServerJsonFileName {
            get {
                return HomePath.ExportServerJsonFileName;
            }
        }

        public static ICommand SetServerJsonVersion { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowDialogWindowCommand(message: $"您确定刷新{HomePath.ExportServerJsonFileName}吗？", title: "确认", onYes: () => {
                try {
                    RpcRoot.OfficialServer.AppSettingService.SetAppSettingAsync(new AppSettingData {
                        Key = HomePath.ExportServerJsonFileName,
                        Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")
                    }, (response, e) => {
                        if (response.IsSuccess()) {
                            VirtualRoot.Out.ShowSuccess($"刷新成功");
                        }
                        else {
                            VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                        }
                    });
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                    VirtualRoot.Out.ShowError($"刷新失败", autoHideSeconds: 4);
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

        public static ICommand ShowGpuNamePage { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowGpuNamePageCommand());
        });

        public static ICommand ShowActionCountPage { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowActionCountPageCommand());
        });

        public static ICommand ShowMqCountsPage { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowMqCountsPageCommand());
        });

        public static ICommand ShowChangePassword { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowChangePassword());
        });

        public static ICommand ShowWsServerNodes { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowWsServerNodePageCommand());
        });

        public static ICommand ShowOverClockDatas { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowOverClockDataPageCommand());
        });

        public static ICommand ShowNTMinerWallets { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowNTMinerWalletPageCommand());
        });

        public static ICommand ShowProperty { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowPropertyCommand());
        });

        public static ICommand JoinQQGroup { get; private set; } = new DelegateCommand(() => {
            string url = "https://jq.qq.com/?_wv=1027&k=5ZPsuCk";
            url = NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItemValue(NTKeyword.ThisSystemSysDicCode, "QQGroupJoinUrl", defaultValue: url);
            VirtualRoot.Execute(new UnTopmostCommand());
            Process.Start(url);
        });

        public static ICommand ShareTutorial { get; private set; } = new DelegateCommand(() => {
            string url = "https://zhuanlan.zhihu.com/p/161614545";
            url = NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItemValue(NTKeyword.ThisSystemSysDicCode, "ShareTutorial", defaultValue: url);
            VirtualRoot.Execute(new UnTopmostCommand());
            Process.Start(url);
        });

        public static ICommand SpeedTutorial { get; private set; } = new DelegateCommand(() => {
            string url = "https://zhuanlan.zhihu.com/p/161614356";
            url = NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItemValue(NTKeyword.ThisSystemSysDicCode, "SpeedTutorial", defaultValue: url);
            VirtualRoot.Execute(new UnTopmostCommand());
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
            VirtualRoot.Execute(new ShowRestartWindowsCommand(countDownSeconds: 4));
        });

        public static ICommand ShowVirtualMemory { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowVirtualMemoryCommand());
        });

        public static ICommand ShowSysDic { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowSysDicPageCommand());
        });
        public static ICommand ShowCoinGroups { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowCoinGroupsCommand());
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
        public static ICommand ShowMinerClientFinderConfig { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowMinerClientFinderConfigCommand());
        });
        public static ICommand ShowOnlineUpdate { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new UpgradeCommand(string.Empty, null));
        });
        public static ICommand ShowHelp { get; private set; } = new DelegateCommand(() => {
            string url = "http://dl.ntminer.top/";
            url = NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItemValue(NTKeyword.ThisSystemSysDicCode, "HelpUrl", defaultValue: url);
            VirtualRoot.Execute(new UnTopmostCommand());
            Process.Start(url);
        });
        public static ICommand ShowCalcConfig { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowCalcConfigCommand());
        });
        public static ICommand ShowHomeDir { get; private set; } = new DelegateCommand(() => {
            Process.Start(HomePath.HomeDirFullName);
        });
        public static ICommand OpenLocalLiteDb { get; private set; } = new DelegateCommand(() => {
            AppRoot.OpenLiteDb(HomePath.LocalDbFileFullName);
        });
        public static ICommand OpenServerLiteDb { get; private set; } = new DelegateCommand(() => {
            AppRoot.OpenLiteDb(HomePath.ServerDbFileFullName);
        });

        public static ICommand ShowCalc { get; private set; } = new DelegateCommand<CoinViewModel>(coinVm => {
            VirtualRoot.Execute(new ShowCalcCommand(coinVm));
        });

        public static ICommand ShowLocalIps { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowLocalIpsCommand());
        });

        public static ICommand OpenWindowsAutoLogonMoreInfo { get; private set; } = new DelegateCommand(() => {
            string url = "https://support.microsoft.com/zh-cn/help/324737/how-to-turn-on-automatic-logon-in-windows";
            url = NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItemValue(NTKeyword.ThisSystemSysDicCode, "WindowsAutoLogonMoreInfo", defaultValue: url);
            VirtualRoot.Execute(new UnTopmostCommand());
            Process.Start(url);
        });

        public static ICommand OpenOfficialSite { get; private set; } = new DelegateCommand(() => {
            string url = HomePageUrl;
            VirtualRoot.Execute(new UnTopmostCommand());
            Process.Start(url);
        });

        public static ICommand OpenDualMineDescription { get; private set; } = new DelegateCommand(() => {
            string url = "https://zhuanlan.zhihu.com/p/346734794";
            Process.Start(url);
        });

        public static string QQGroup {
            get {
                if (WpfUtil.IsInDesignMode) {
                    return string.Empty;
                }
                return NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItemValue(NTKeyword.ThisSystemSysDicCode, "QQGroup", defaultValue: "863725136");
            }
        }

        public static string HomePageUrl {
            get {
                string url = "http://dl.ntminer.top";
                if (WpfUtil.IsInDesignMode) {
                    return url;
                }
                string dicItemValue = NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItemValue(NTKeyword.ThisSystemSysDicCode, "HomePageUrl", defaultValue: url);
                if (!string.IsNullOrEmpty(dicItemValue)) {
                    url = dicItemValue;
                }
                return url;
            }
        }

        public static string OfficialSiteName {
            get {
                string url = HomePageUrl;
                if (!string.IsNullOrEmpty(url)) {
                    if (url.StartsWith("https://")) {
                        return url.Substring("https://".Length);
                    }
                    if (url.StartsWith("http://")) {
                        return url.Substring("http://".Length);
                    }
                }
                return url;
            }
        }

        public static string AppMinerName {
            get {
                const string txt = "开源矿工";
                if (WpfUtil.IsInDesignMode) {
                    return txt;
                }
                return NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItemValue(NTKeyword.ThisSystemSysDicCode, "AppMinerName", txt);
            }
        }

        public static string AppMinerDescription {
            get {
                const string txt = " - 做最好的矿工";
                if (WpfUtil.IsInDesignMode) {
                    return txt;
                }
                return NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItemDescription(NTKeyword.ThisSystemSysDicCode, "AppMinerName", txt);
            }
        }

        public static string AppMinerIntro {
            get {
                const string txt = "开源、开放、安全、专业、更高收益。QQ群863725136";
                if (WpfUtil.IsInDesignMode) {
                    return txt;
                }
                return NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItemValue(NTKeyword.ThisSystemSysDicCode, "AppMinerIntro", txt);
            }
        }

        public static ICommand BusinessModel { get; private set; } = new DelegateCommand(() => {
            string url = "https://zhuanlan.zhihu.com/p/161614178";
            url = NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItemValue(NTKeyword.ThisSystemSysDicCode, "BusinessModelUrl", url);
            VirtualRoot.Execute(new UnTopmostCommand());
            Process.Start(url);
        });

        public static ICommand OpenGithub { get; private set; } = new DelegateCommand(() => {
            string url = "https://github.com/ntminer/NtMiner";
            url = NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItemValue(NTKeyword.ThisSystemSysDicCode, "GithubUrl", url);
            VirtualRoot.Execute(new UnTopmostCommand());
            Process.Start(url);
        });

        public static ICommand OpenLGPL { get; private set; } = new DelegateCommand(() => {
            string url = $"https://ntwebsite.{NTKeyword.CloudFileDomain}/img/LGPL.png";
            url = NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItemValue(NTKeyword.ThisSystemSysDicCode, "LGPL", url);
            VirtualRoot.Execute(new UnTopmostCommand());
            Process.Start(url);
        });

        public static ICommand OpenDiscussSite { get; private set; } = new DelegateCommand(() => {
            string url = "https://github.com/ntminer/NtMiner/issues";
            url = NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItemValue(NTKeyword.ThisSystemSysDicCode, "DiscussUrl", url);
            VirtualRoot.Execute(new UnTopmostCommand());
            Process.Start(url);
        });

        public static ICommand MinerStudioTutorial { get; private set; } = new DelegateCommand(() => {
            string url = "https://zhuanlan.zhihu.com/p/137759804";
            url = NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItemValue(NTKeyword.ThisSystemSysDicCode, "MinerStudioTutorial", url);
            VirtualRoot.Execute(new UnTopmostCommand());
            Process.Start(url);
        });

        public static ICommand DownloadMinerStudio { get; private set; } = new DelegateCommand(() => {
            string url = "https://github.com/ntminer/NtMiner/releases";
            url = NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItemValue(NTKeyword.ThisSystemSysDicCode, "DownloadMinerStudio", url);
            VirtualRoot.Execute(new UnTopmostCommand());
            Process.Start(url);
        });

        public static ICommand WindowsZoomBug { get; private set; } = new DelegateCommand(() => {
            string url = "https://zhuanlan.zhihu.com/p/161613889";
            url = NTMinerContext.Instance.ServerContext.SysDicItemSet.TryGetDicItemValue(NTKeyword.ThisSystemSysDicCode, "WindowsZoomBug", url);
            VirtualRoot.Execute(new UnTopmostCommand());
            Process.Start(url);
        });

        public static ICommand ShowQQGroupQrCode { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowQQGroupQrCodeCommand());
        });
    }
}
