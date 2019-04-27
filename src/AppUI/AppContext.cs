using NTMiner.Core;
using NTMiner.MinerServer;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NTMiner {
    public class AppContext {
        /// <summary>
        /// 这个可以在关闭界面的时候释放
        /// </summary>
        public static readonly AppContext Current = new AppContext();

        private AppContext() {
        }

        private bool _isMinerClient;

        public bool IsMinerClient {
            get => _isMinerClient;
        }

        public void SetIsMinerClient(bool value) {
            _isMinerClient = value;
        }

        private CoinViewModels _coinVms;
        public CoinViewModels CoinVms {
            get {
                return _coinVms ?? (_coinVms = new CoinViewModels());
            }
        }

        private GpuSpeedViewModels _gpuSpeedVms;
        public GpuSpeedViewModels GpuSpeedVms {
            get {
                return _gpuSpeedVms ?? (_gpuSpeedVms = new GpuSpeedViewModels());
            }
        }

        private StartStopMineButtonViewModel _startStopMineButtonVm;
        public StartStopMineButtonViewModel StartStopMineButtonVm {
            get {
                return _startStopMineButtonVm ?? (_startStopMineButtonVm = new StartStopMineButtonViewModel());
            }
        }

        private PoolKernelViewModels _poolKernelVms;
        public PoolKernelViewModels PoolKernelVms {
            get {
                return _poolKernelVms ?? (_poolKernelVms = new PoolKernelViewModels());
            }
        }

        private CoinGroupViewModels _coinGroupVms;
        public CoinGroupViewModels CoinGroupVms {
            get {
                return _coinGroupVms ?? (_coinGroupVms = new CoinGroupViewModels());
            }
        }

        private CoinKernelViewModels _coinKernelVms;
        public CoinKernelViewModels CoinKernelVms {
            get {
                return _coinKernelVms ?? (_coinKernelVms = new CoinKernelViewModels());
            }
        }

        private CoinProfileViewModels _coinProfileVms;
        public CoinProfileViewModels CoinProfileVms {
            get {
                return _coinProfileVms ?? (_coinProfileVms = new CoinProfileViewModels());
            }
        }

        private CoinSnapshotDataViewModels _coinSnapshotDataVms;
        public CoinSnapshotDataViewModels CoinSnapshotDataVms {
            get {
                return _coinSnapshotDataVms ?? (_coinSnapshotDataVms = new CoinSnapshotDataViewModels());
            }
        }

        private ColumnsShowViewModels _columnsShowVms;
        public ColumnsShowViewModels ColumnsShowVms {
            get {
                return _columnsShowVms ?? (_columnsShowVms = new ColumnsShowViewModels());
            }
        }

        private DriveSet _driveSet;
        public DriveSet DriveSet {
            get {
                return _driveSet ?? (_driveSet = new DriveSet());
            }
        }

        private VirtualMemorySet _virtualMemorySet;
        public VirtualMemorySet VirtualMemorySet {
            get {
                return _virtualMemorySet ?? (_virtualMemorySet = new VirtualMemorySet());
            }
        }


        #region Commands
        public string GetIconFileFullName(ICoin coin) {
            if (coin == null || string.IsNullOrEmpty(coin.Icon)) {
                return string.Empty;
            }
            string iconFileFullName = Path.Combine(SpecialPath.CoinIconsDirFullName, coin.Icon);
            return iconFileFullName;
        }

        public string CurrentVersion => NTMinerRoot.CurrentVersion.ToString();

        public string VersionTag => NTMinerRoot.CurrentVersionTag;

        public string QQGroup => NTMinerRoot.Current.QQGroup;

        private readonly string _windowsEdition = Windows.OS.Current.WindowsEdition?.Replace("Windows ", "Win");
        public string WindowsEdition {
            get {
                return _windowsEdition;
            }
        }

        public string TotalVirtualMemoryGbText => AppContext.Current.DriveSet.VirtualMemorySet.TotalVirtualMemoryGbText;

        public string GpuSetInfo => NTMinerRoot.Current.GpuSetInfo;

        public string DriverVersion {
            get {
                var gpuSet = NTMinerRoot.Current.GpuSet;
                if (gpuSet.GpuType == GpuType.NVIDIA) {
                    var cudaVersion = gpuSet.Properties.FirstOrDefault(a => a.Code == "CudaVersion");
                    if (cudaVersion != null) {
                        return $"{gpuSet.DriverVersion} / {cudaVersion.Value}";
                    }
                }
                return gpuSet.DriverVersion;
            }
        }

        public ICommand ConfigControlCenterHost { get; private set; } = new DelegateCommand(ControlCenterHostConfig.ShowWindow);

        public readonly BitmapImage BigLogoImageSource = IconConst.BigLogoImageSource;

        public IEnumerable<EnumItem<SupportedGpu>> SupportedGpuEnumItems => SupportedGpu.AMD.GetEnumItems();

        public IEnumerable<EnumItem<GpuType>> GpuTypeEnumItems => GpuType.AMD.GetEnumItems();

        public IEnumerable<EnumItem<LogEnum>> LogTypeItems => LogEnum.DevConsole.GetEnumItems();

        public IEnumerable<EnumItem<PublishStatus>> PublishStatusEnumItems => PublishStatus.Published.GetEnumItems();

        public IEnumerable<EnumItem<MineStatus>> MineStatusEnumItems => MineStatus.All.GetEnumItems();

        public double MainWindowHeight => GetMainWindowHeight(DevMode.IsDevMode);

        public double GetMainWindowHeight(bool isDevMode) {
            if (SystemParameters.WorkArea.Size.Height >= 600) {
                return 600;
            }
            else if (SystemParameters.WorkArea.Size.Height >= 520) {
                return 520;
            }
            return 480;
        }

        public double MainWindowWidth {
            get {
                if (SystemParameters.WorkArea.Size.Width > 1000) {
                    return 1000;
                }
                else if (SystemParameters.WorkArea.Size.Width >= 860) {
                    return 860;
                }
                return 800;
            }
        }

        public ICommand ExportServerJson { get; private set; } = new DelegateCommand(() => {
            try {
                string fileName = NTMinerRoot.ExportServerVersionJson();
                NotiCenterWindowViewModel.Current.Manager.ShowSuccessMessage($"导出成功：{fileName}");
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        });

        public string ServerJsonFileName = AssemblyInfo.ServerJsonFileName;

        public ICommand SetServerJsonVersion { get; private set; } = new DelegateCommand(() => {
            try {
                DialogWindow.ShowDialog(message: $"您确定刷新{AssemblyInfo.ServerJsonFileName}吗？", title: "确认", onYes: () => {
                    try {
                        VirtualRoot.Execute(new ChangeServerAppSettingCommand(new AppSettingData {
                            Key = Current.ServerJsonFileName,
                            Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")
                        }));
                        NotiCenterWindowViewModel.Current.Manager.ShowSuccessMessage($"刷新成功");
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                        NotiCenterWindowViewModel.Current.Manager.ShowErrorMessage($"刷新失败");
                    }
                }, icon: IconConst.IconConfirm);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        });

        public ICommand ShowUsers { get; private set; } = new DelegateCommand(UserPage.ShowWindow);

        public ICommand ShowOverClockDatas { get; private set; } = new DelegateCommand(OverClockDataPage.ShowWindow);

        public ICommand ShowChartsWindow { get; private set; } = new DelegateCommand(ChartsWindow.ShowWindow);

        public ICommand ShowInnerProperty { get; private set; } = new DelegateCommand(InnerProperty.ShowWindow);

        public ICommand JoinQQGroup { get; private set; } = new DelegateCommand(() => {
            Process.Start("https://jq.qq.com/?_wv=1027&k=5ZPsuCk");
        });

        public ICommand RunAsAdministrator { get; private set; } = new DelegateCommand(AppHelper.RunAsAdministrator);

        public ICommand ShowNotificationSample { get; private set; } = new DelegateCommand(NotificationSample.ShowWindow);

        public ICommand AppExit { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new CloseNTMinerCommand());
        });

        public ICommand ShowRestartWindows { get; private set; } = new DelegateCommand(RestartWindows.ShowDialog);

        public ICommand ShowVirtualMemory { get; private set; } = new DelegateCommand(Views.Ucs.VirtualMemory.ShowWindow);

        public ICommand ShowSysDic { get; private set; } = new DelegateCommand(SysDicPage.ShowWindow);
        public ICommand ShowGroups { get; private set; } = new DelegateCommand(GroupPage.ShowWindow);
        public ICommand ShowCoins { get; private set; } = new DelegateCommand<CoinViewModel>((currentCoin) => {
            CoinPage.ShowWindow(currentCoin, "coin");
        });
        public ICommand ManageColumnsShows { get; private set; } = new DelegateCommand(ColumnsShowPage.ShowWindow);
        public ICommand ManagePools { get; private set; } = new DelegateCommand<CoinViewModel>(coinVm => {
            CoinPage.ShowWindow(coinVm, "pool");
        });
        public ICommand ManageWallet { get; private set; } = new DelegateCommand<CoinViewModel>(coinVm => {
            CoinPage.ShowWindow(coinVm, "wallet");
        });
        public ICommand ShowKernelInputs { get; private set; } = new DelegateCommand(KernelInputPage.ShowWindow);
        public ICommand ShowKernelOutputs { get; private set; } = new DelegateCommand<KernelOutputViewModel>(KernelOutputPage.ShowWindow);
        public ICommand ShowKernels { get; private set; } = new DelegateCommand(() => {
            KernelPage.ShowWindow(Guid.Empty);
        });
        public ICommand ShowAbout { get; private set; } = new DelegateCommand<string>(AboutPage.ShowWindow);
        public ICommand ShowSpeedChart { get; private set; } = new DelegateCommand(() => {
            SpeedCharts.ShowWindow();
        });
        public ICommand ShowNTMinerUpdaterConfig { get; private set; } = new DelegateCommand(NTMinerUpdaterConfig.ShowWindow);
        public ICommand ShowOnlineUpdate { get; private set; } = new DelegateCommand(() => {
            Upgrade(string.Empty, null);
        });
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
                        if (NTMinerRoot.Current.LocalAppSettingSet.TryGetAppSetting("UpdaterVersion", out IAppSetting appSetting) && appSetting.Value != null) {
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
                                        NotiCenterWindowViewModel.Current.Manager.ShowErrorMessage(message);
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
        public ICommand ShowHelp { get; private set; } = new DelegateCommand(() => {
            Process.Start("https://github.com/ntminer/ntminer");
        });
        public ICommand ShowMinerClients { get; private set; } = new DelegateCommand(() => {
            MinerClientsWindow.ShowWindow();
        });
        public ICommand ShowCalcConfig { get; private set; } = new DelegateCommand(CalcConfig.ShowWindow);
        public ICommand ShowGlobalDir { get; private set; } = new DelegateCommand(() => {
            Process.Start(VirtualRoot.GlobalDirFullName);
        });
        public ICommand OpenLocalLiteDb { get; private set; } = new DelegateCommand(() => {
            OpenLiteDb(SpecialPath.LocalDbFileFullName);
        });
        public ICommand OpenServerLiteDb { get; private set; } = new DelegateCommand(() => {
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

        public ICommand ShowCalc { get; private set; } = new DelegateCommand<CoinViewModel>(Calc.ShowWindow);

        public ICommand OpenOfficialSite { get; private set; } = new DelegateCommand(() => {
            Process.Start("https://github.com/ntminer/ntminer");
        });

        public ICommand OpenDiscussSite { get; private set; } = new DelegateCommand(() => {
            Process.Start("https://github.com/ntminer/ntminer/issues");
        });

        public ICommand DownloadMinerStudio { get; private set; } = new DelegateCommand(() => {
            Process.Start(AssemblyInfo.MinerJsonBucket + "MinerStudio.exe");
        });

        public ICommand ShowQQGroupQrCode { get; private set; } = new DelegateCommand(QQGroupQrCode.ShowWindow);
        #endregion
    }
}
