using NTMiner.MinerServer;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NTMiner {
    public partial class AppContext {
        /// <summary>
        /// 这个可以在关闭界面的时候释放
        /// </summary>
        public static readonly AppContext Current = new AppContext();

        private AppContext() {
        }

        private MinerClientsWindowViewModel _minerClientsWindowVm;
        public MinerClientsWindowViewModel MinerClientsWindowVm {
            get {
                return _minerClientsWindowVm ?? (_minerClientsWindowVm = new MinerClientsWindowViewModel());
            }
        }

        private MinerProfileViewModel _minerProfileVm;
        public MinerProfileViewModel MinerProfileVm {
            get {
                return _minerProfileVm ?? (_minerProfileVm = new MinerProfileViewModel());
            }
        }

        #region context
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

        private DriveSetViewModel _driveSetVm;
        public DriveSetViewModel DriveSetVm {
            get {
                return _driveSetVm ?? (_driveSetVm = new DriveSetViewModel());
            }
        }

        private VirtualMemorySetViewModel _virtualMemorySetVm;
        public VirtualMemorySetViewModel VirtualMemorySetVm {
            get {
                return _virtualMemorySetVm ?? (_virtualMemorySetVm = new VirtualMemorySetViewModel());
            }
        }

        private GpuProfileViewModels _gpuProfileVms;
        public GpuProfileViewModels GpuProfileVms {
            get {
                return _gpuProfileVms ?? (_gpuProfileVms = new GpuProfileViewModels());
            }
        }

        private GpuViewModels _gpuVms;
        public GpuViewModels GpuVms {
            get {
                return _gpuVms ?? (_gpuVms = new GpuViewModels());
            }
        }

        private GroupViewModels _groupVms;
        public GroupViewModels GroupVms {
            get {
                return _groupVms ?? (_groupVms = new GroupViewModels());
            }
        }

        private KernelInputViewModels _kernelInputVms;
        public KernelInputViewModels KernelInputVms {
            get {
                return _kernelInputVms ?? (_kernelInputVms = new KernelInputViewModels());
            }
        }

        private KernelOutputFilterViewModels _kernelOutputFilterVms;
        public KernelOutputFilterViewModels KernelOutputFilterVms {
            get {
                return _kernelOutputFilterVms ?? (_kernelOutputFilterVms = new KernelOutputFilterViewModels());
            }
        }

        private KernelOutputTranslaterViewModels _kernelOutputTranslaterVms;
        public KernelOutputTranslaterViewModels KernelOutputTranslaterVms {
            get {
                return _kernelOutputTranslaterVms ?? (_kernelOutputTranslaterVms = new KernelOutputTranslaterViewModels());
            }
        }

        private KernelOutputViewModels _kernelOutputVms;
        public KernelOutputViewModels KernelOutputVms {
            get {
                return _kernelOutputVms ?? (_kernelOutputVms = new KernelOutputViewModels());
            }
        }

        private KernelViewModels _kernelVms;
        public KernelViewModels KernelVms {
            get {
                return _kernelVms ?? (_kernelVms = new KernelViewModels());
            }
        }

        private MinerGroupViewModels _minerGroupVms;
        public MinerGroupViewModels MinerGroupVms {
            get {
                return _minerGroupVms ?? (_minerGroupVms = new MinerGroupViewModels());
            }
        }

        private MineWorkViewModels _mineWorkVms;
        public MineWorkViewModels MineWorkVms {
            get {
                return _mineWorkVms ?? (_mineWorkVms = new MineWorkViewModels());
            }
        }

        private OverClockDataViewModels _overClockDataVms;
        public OverClockDataViewModels OverClockDataVms {
            get {
                return _overClockDataVms ?? (_overClockDataVms = new OverClockDataViewModels());
            }
        }

        private PoolProfileViewModels _poolProfileVms;
        public PoolProfileViewModels PoolProfileVms {
            get {
                return _poolProfileVms ?? (_poolProfileVms = new PoolProfileViewModels());
            }
        }

        private PoolViewModels _poolVms;
        public PoolViewModels PoolVms {
            get {
                return _poolVms ?? (_poolVms = new PoolViewModels());
            }
        }

        private ShareViewModels _shareVms;
        public ShareViewModels ShareVms {
            get {
                return _shareVms ?? (_shareVms = new ShareViewModels());
            }
        }

        private WalletViewModels _walletVms;
        public WalletViewModels WalletVms {
            get {
                return _walletVms ?? (_walletVms = new WalletViewModels());
            }
        }

        private UserViewModels _userVms;
        public UserViewModels UserVms {
            get {
                return _userVms ?? (_userVms = new UserViewModels());
            }
        }

        private SysDicViewModels _sysDicVms;
        public SysDicViewModels SysDicVms {
            get {
                return _sysDicVms ?? (_sysDicVms = new SysDicViewModels());
            }
        }

        private SysDicItemViewModels _sysDicItemVms;
        public SysDicItemViewModels SysDicItemVms {
            get {
                return _sysDicItemVms ?? (_sysDicItemVms = new SysDicItemViewModels());
            }
        }

        private GpuStatusBarViewModel _gpuStatusBarVm;
        public GpuStatusBarViewModel GpuStatusBarVm {
            get {
                return _gpuStatusBarVm ?? (_gpuStatusBarVm = new GpuStatusBarViewModel());
            }
        }
        #endregion

        #region Commands
        public string CurrentVersion {
            get {
                return NTMinerRoot.CurrentVersion.ToString();
            }
        }

        public string VersionTag {
            get {
                return NTMinerRoot.CurrentVersionTag;
            }
        }

        public string QQGroup {
            get {
                return NTMinerRoot.Instance.QQGroup;
            }
        }

        private readonly string _windowsEdition = Windows.OS.Instance.WindowsEdition?.Replace("Windows ", "Win");
        public string WindowsEdition {
            get {
                return _windowsEdition;
            }
        }

        public string TotalVirtualMemoryGbText {
            get {
                return VirtualMemorySetVm.TotalVirtualMemoryGbText;
            }
        }

        public string GpuSetInfo {
            get {
                return NTMinerRoot.Instance.GpuSetInfo;
            }
        }

        public string DriverVersion {
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

        public ICommand ConfigControlCenterHost { get; private set; } = new DelegateCommand(ControlCenterHostConfig.ShowWindow);

        public BitmapImage BigLogoImageSource { get; private set; } = IconConst.BigLogoImageSource;

        public ICommand ExportServerJson { get; private set; } = new DelegateCommand(() => {
            try {
                string fileName = NTMinerRoot.ExportServerVersionJson();
                NotiCenterWindowViewModel.Instance.Manager.ShowSuccessMessage($"导出成功：{fileName}");
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        });

        public string ServerJsonFileName { get; private set; } = AssemblyInfo.ServerJsonFileName;

        public ICommand SetServerJsonVersion { get; private set; } = new DelegateCommand(() => {
            try {
                DialogWindow.ShowDialog(message: $"您确定刷新{AssemblyInfo.ServerJsonFileName}吗？", title: "确认", onYes: () => {
                    try {
                        VirtualRoot.Execute(new ChangeServerAppSettingCommand(new AppSettingData {
                            Key = Current.ServerJsonFileName,
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
            AppStatic.Upgrade(string.Empty, null);
        });
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
