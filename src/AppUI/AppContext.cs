using NTMiner.Bus;
using NTMiner.MinerServer;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace NTMiner {
    public partial class AppContext {
        private static readonly List<IDelegateHandler> _contextHandlers = new List<IDelegateHandler>();

        /// <summary>
        /// 命令窗口。使用该方法的代码行应将前两个参数放在第一行以方便vs查找引用时展示出参数信息
        /// </summary>
        private static DelegateHandler<TCmd> Window<TCmd>(string description, LogEnum logType, Action<TCmd> action)
            where TCmd : ICmd {
            return VirtualRoot.Path(description, logType, action).AddToCollection(_contextHandlers);
        }

        /// <summary>
        /// 事件响应
        /// </summary>
        private static DelegateHandler<TEvent> On<TEvent>(string description, LogEnum logType, Action<TEvent> action)
            where TEvent : IEvent {
            return VirtualRoot.Path(description, logType, action).AddToCollection(_contextHandlers);
        }

        private static AppContext _current = null;
        private static readonly object _currentLocker = new object();
        public static AppContext Current {
            get {
                if (_current == null) {
                    lock (_currentLocker) {
                        if (_current == null) {
                            _current = new AppContext();
                        }
                    }
                }
                return _current;
            }
        }

        public static void Open() {
            foreach (var handler in _contextHandlers) {
                handler.IsPaused = false;
            }
        }

        public static void Close() {
            foreach (var handler in _contextHandlers) {
                handler.IsPaused = true;
            }
        }

        private AppContext() {
        }

        private MinerClientsWindowViewModel _minerClientsWindowVm;
        private readonly object _minerClientsWindowVmLocker = new object();
        public MinerClientsWindowViewModel MinerClientsWindowVm {
            get {
                if (_minerClientsWindowVm == null) {
                    lock (_minerClientsWindowVmLocker) {
                        if (_minerClientsWindowVm == null) {
                            _minerClientsWindowVm = new MinerClientsWindowViewModel();
                        }
                    }
                }
                return _minerClientsWindowVm;
            }
        }

        private MinerProfileViewModel _minerProfileVm;
        private readonly object _minerProfileVmLocker = new object();
        public MinerProfileViewModel MinerProfileVm {
            get {
                if (_minerProfileVm == null) {
                    lock (_minerProfileVmLocker) {
                        if (_minerProfileVm == null) {
                            _minerProfileVm = new MinerProfileViewModel();
                        }
                    }
                }
                return _minerProfileVm;
            }
        }

        #region context
        private CoinViewModels _coinVms;
        private readonly object _coinVmsLocker = new object();
        public CoinViewModels CoinVms {
            get {
                if (_coinVms == null) {
                    lock (_coinVmsLocker) {
                        if (_coinVms == null) {
                            _coinVms = new CoinViewModels();
                        }
                    }
                }
                return _coinVms;
            }
        }

        private GpuSpeedViewModels _gpuSpeedVms;
        private readonly object _gpuSpeedVmsLocker = new object();
        public GpuSpeedViewModels GpuSpeedVms {
            get {
                if (_gpuSpeedVms == null) {
                    lock (_gpuSpeedVmsLocker) {
                        if (_gpuSpeedVms == null) {
                            _gpuSpeedVms = new GpuSpeedViewModels();
                        }
                    }
                }
                return _gpuSpeedVms;
            }
        }

        private StartStopMineButtonViewModel _startStopMineButtonVm;
        private readonly object _startStopMineButtonVmLocker = new object();
        public StartStopMineButtonViewModel StartStopMineButtonVm {
            get {
                if (_startStopMineButtonVm == null) {
                    lock (_startStopMineButtonVmLocker) {
                        if (_startStopMineButtonVm == null) {
                            _startStopMineButtonVm = new StartStopMineButtonViewModel();
                        }
                    }
                }
                return _startStopMineButtonVm;
            }
        }

        private PoolKernelViewModels _poolKernelVms;
        private readonly object _poolKernelVmsLocker = new object();
        public PoolKernelViewModels PoolKernelVms {
            get {
                if (_poolKernelVms == null) {
                    lock (_poolKernelVmsLocker) {
                        if (_poolKernelVms == null) {
                            _poolKernelVms = new PoolKernelViewModels();
                        }
                    }
                }
                return _poolKernelVms;
            }
        }

        private CoinGroupViewModels _coinGroupVms;
        private readonly object _coinGroupVmsLocker = new object();
        public CoinGroupViewModels CoinGroupVms {
            get {
                if (_coinGroupVms == null) {
                    lock (_coinGroupVmsLocker) {
                        if (_coinGroupVms == null) {
                            _coinGroupVms = new CoinGroupViewModels();
                        }
                    }
                }
                return _coinGroupVms;
            }
        }

        private CoinKernelViewModels _coinKernelVms;
        private readonly object _coinKernelVmsLocker = new object();
        public CoinKernelViewModels CoinKernelVms {
            get {
                if (_coinKernelVms == null) {
                    lock (_coinKernelVmsLocker) {
                        if (_coinKernelVms == null) {
                            _coinKernelVms = new CoinKernelViewModels();
                        }
                    }
                }
                return _coinKernelVms;
            }
        }

        private CoinProfileViewModels _coinProfileVms;
        private readonly object _coinProfileVmsLocker = new object();
        public CoinProfileViewModels CoinProfileVms {
            get {
                if (_coinProfileVms == null) {
                    lock (_coinProfileVmsLocker) {
                        if (_coinProfileVms == null) {
                            _coinProfileVms = new CoinProfileViewModels();
                        }
                    }
                }
                return _coinProfileVms;
            }
        }

        private CoinSnapshotDataViewModels _coinSnapshotDataVms;
        private readonly object _coinSnapshotDataVmsLocker = new object();
        public CoinSnapshotDataViewModels CoinSnapshotDataVms {
            get {
                if (_coinSnapshotDataVms == null) {
                    lock (_coinSnapshotDataVmsLocker) {
                        if (_coinSnapshotDataVms == null) {
                            _coinSnapshotDataVms = new CoinSnapshotDataViewModels();
                        }
                    }
                }
                return _coinSnapshotDataVms;
            }
        }

        private ColumnsShowViewModels _columnsShowVms;
        private readonly object _columnsShowVmsLocker = new object();
        public ColumnsShowViewModels ColumnsShowVms {
            get {
                if (_columnsShowVms == null) {
                    lock (_columnsShowVmsLocker) {
                        if (_columnsShowVms == null) {
                            _columnsShowVms = new ColumnsShowViewModels();
                        }
                    }
                }
                return _columnsShowVms;
            }
        }

        private DriveSetViewModel _driveSetVm;
        private readonly object _driveSetVmLocker = new object();
        public DriveSetViewModel DriveSetVm {
            get {
                if (_driveSetVm == null) {
                    lock (_driveSetVmLocker) {
                        if (_driveSetVm == null) {
                            _driveSetVm = new DriveSetViewModel();
                        }
                    }
                }
                return _driveSetVm;
            }
        }

        private VirtualMemorySetViewModel _virtualMemorySetVm;
        private readonly object _virtualMemorySetVmLocker = new object();
        public VirtualMemorySetViewModel VirtualMemorySetVm {
            get {
                if (_virtualMemorySetVm == null) {
                    lock (_virtualMemorySetVmLocker) {
                        if (_virtualMemorySetVm == null) {
                            _virtualMemorySetVm = new VirtualMemorySetViewModel();
                        }
                    }
                }
                return _virtualMemorySetVm;
            }
        }

        private GpuProfileViewModels _gpuProfileVms;
        private readonly object _gpuProfileVmsLocker = new object();
        public GpuProfileViewModels GpuProfileVms {
            get {
                if (_gpuProfileVms == null) {
                    lock (_gpuProfileVmsLocker) {
                        if (_gpuProfileVms == null) {
                            _gpuProfileVms = new GpuProfileViewModels();
                        }
                    }
                }
                return _gpuProfileVms;
            }
        }

        private GpuViewModels _gpuVms;
        private readonly object _gpuVmsLocker = new object();
        public GpuViewModels GpuVms {
            get {
                if (_gpuVms == null) {
                    lock (_gpuVmsLocker) {
                        if (_gpuVms == null) {
                            _gpuVms = new GpuViewModels();
                        }
                    }
                }
                return _gpuVms;
            }
        }

        private GroupViewModels _groupVms;
        private readonly object _groupVmsLocker = new object();
        public GroupViewModels GroupVms {
            get {
                if (_groupVms == null) {
                    lock (_groupVmsLocker) {
                        if (_groupVms == null) {
                            _groupVms = new GroupViewModels();
                        }
                    }
                }
                return _groupVms;
            }
        }

        private KernelInputViewModels _kernelInputVms;
        private readonly object _kernelInputVmsLocker = new object();
        public KernelInputViewModels KernelInputVms {
            get {
                if (_kernelInputVms == null) {
                    lock (_kernelInputVmsLocker) {
                        if (_kernelInputVms == null) {
                            _kernelInputVms = new KernelInputViewModels();
                        }
                    }
                }
                return _kernelInputVms;
            }
        }

        private KernelOutputFilterViewModels _kernelOutputFilterVms;
        private readonly object _kernelOutputFilterVmsLocker = new object();
        public KernelOutputFilterViewModels KernelOutputFilterVms {
            get {
                if (_kernelOutputFilterVms == null) {
                    lock (_kernelOutputFilterVmsLocker) {
                        if (_kernelOutputFilterVms == null) {
                            _kernelOutputFilterVms = new KernelOutputFilterViewModels();
                        }
                    }
                }
                return _kernelOutputFilterVms;
            }
        }

        private KernelOutputTranslaterViewModels _kernelOutputTranslaterVms;
        private readonly object _kernelOutputTranslaterVmsLocker = new object();
        public KernelOutputTranslaterViewModels KernelOutputTranslaterVms {
            get {
                if (_kernelOutputTranslaterVms == null) {
                    lock (_kernelOutputTranslaterVmsLocker) {
                        if (_kernelOutputTranslaterVms == null) {
                            _kernelOutputTranslaterVms = new KernelOutputTranslaterViewModels();
                        }
                    }
                }
                return _kernelOutputTranslaterVms;
            }
        }

        private KernelOutputViewModels _kernelOutputVms;
        private readonly object _kernelOutputVmsLocker = new object();
        public KernelOutputViewModels KernelOutputVms {
            get {
                if (_kernelOutputVms == null) {
                    lock (_kernelOutputVmsLocker) {
                        if (_kernelOutputVms == null) {
                            _kernelOutputVms = new KernelOutputViewModels();
                        }
                    }
                }
                return _kernelOutputVms;
            }
        }

        private KernelViewModels _kernelVms;
        private readonly object _kernelVmsLocker = new object();
        public KernelViewModels KernelVms {
            get {
                if (_kernelVms == null) {
                    lock (_kernelVmsLocker) {
                        if (_kernelVms == null) {
                            _kernelVms = new KernelViewModels();
                        }
                    }
                }
                return _kernelVms;
            }
        }

        private MinerGroupViewModels _minerGroupVms;
        private readonly object _minerGroupVmsLocker = new object();
        public MinerGroupViewModels MinerGroupVms {
            get {
                if (_minerGroupVms == null) {
                    lock (_minerGroupVmsLocker) {
                        if (_minerGroupVms == null) {
                            _minerGroupVms = new MinerGroupViewModels();
                        }
                    }
                }
                return _minerGroupVms;
            }
        }

        private MineWorkViewModels _mineWorkVms;
        private readonly object _mineWorkVmsLocker = new object();
        public MineWorkViewModels MineWorkVms {
            get {
                if (_mineWorkVms == null) {
                    lock (_mineWorkVmsLocker) {
                        if (_mineWorkVms == null) {
                            _mineWorkVms = new MineWorkViewModels();
                        }
                    }
                }
                return _mineWorkVms;
            }
        }

        private OverClockDataViewModels _overClockDataVms;
        private readonly object _overClockDataVmsLocker = new object();
        public OverClockDataViewModels OverClockDataVms {
            get {
                if (_overClockDataVms == null) {
                    lock (_overClockDataVmsLocker) {
                        if (_overClockDataVms == null) {
                            _overClockDataVms = new OverClockDataViewModels();
                        }
                    }
                }
                return _overClockDataVms;
            }
        }

        private PoolProfileViewModels _poolProfileVms;
        private readonly object _poolProfileVmsLocker = new object();
        public PoolProfileViewModels PoolProfileVms {
            get {
                if (_poolProfileVms == null) {
                    lock (_poolProfileVmsLocker) {
                        if (_poolProfileVms == null) {
                            _poolProfileVms = new PoolProfileViewModels();
                        }
                    }
                }
                return _poolProfileVms;
            }
        }

        private PoolViewModels _poolVms;
        private readonly object _poolVmsLocker = new object();
        public PoolViewModels PoolVms {
            get {
                if (_poolVms == null) {
                    lock (_poolVmsLocker) {
                        if (_poolVms == null) {
                            _poolVms = new PoolViewModels();
                        }
                    }
                }
                return _poolVms;
            }
        }

        private ShareViewModels _shareVms;
        private readonly object _shareVmsLocker = new object();
        public ShareViewModels ShareVms {
            get {
                if (_shareVms == null) {
                    lock (_shareVmsLocker) {
                        if (_shareVms == null) {
                            _shareVms = new ShareViewModels();
                        }
                    }
                }
                return _shareVms;
            }
        }

        private WalletViewModels _walletVms;
        private readonly object _walletVmsLocker = new object();
        public WalletViewModels WalletVms {
            get {
                if (_walletVms == null) {
                    lock (_walletVmsLocker) {
                        if (_walletVms == null) {
                            _walletVms = new WalletViewModels();
                        }
                    }
                }
                return _walletVms;
            }
        }

        private UserViewModels _userVms;
        private readonly object _userVmsLocker = new object();
        public UserViewModels UserVms {
            get {
                if (_userVms == null) {
                    lock (_userVmsLocker) {
                        if (_userVms == null) {
                            _userVms = new UserViewModels();
                        }
                    }
                }
                return _userVms;
            }
        }

        private SysDicViewModels _sysDicVms;
        private readonly object _sysDicVmsLocker = new object();
        public SysDicViewModels SysDicVms {
            get {
                if (_sysDicVms == null) {
                    lock (_sysDicVmsLocker) {
                        if (_sysDicVms == null) {
                            _sysDicVms = new SysDicViewModels();
                        }
                    }
                }
                return _sysDicVms;
            }
        }

        private SysDicItemViewModels _sysDicItemVms;
        private readonly object _sysDicItemVmsLocker = new object();
        public SysDicItemViewModels SysDicItemVms {
            get {
                if (_sysDicItemVms == null) {
                    lock (_sysDicItemVmsLocker) {
                        if (_sysDicItemVms == null) {
                            _sysDicItemVms = new SysDicItemViewModels();
                        }
                    }
                }
                return _sysDicItemVms;
            }
        }

        private GpuStatusBarViewModel _gpuStatusBarVm;
        private readonly object _gpuStatusBarVmLocker = new object();
        public GpuStatusBarViewModel GpuStatusBarVm {
            get {
                if (_gpuStatusBarVm == null) {
                    lock (_gpuStatusBarVmLocker) {
                        if (_gpuStatusBarVm == null) {
                            _gpuStatusBarVm = new GpuStatusBarViewModel();
                        }
                    }
                }
                return _gpuStatusBarVm;
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

        public ICommand ExportServerJson { get; private set; } = new DelegateCommand(() => {
            try {
                string fileName = NTMinerRoot.ExportServerVersionJson();
                NotiCenterWindowViewModel.Instance.Manager.ShowSuccessMessage($"{fileName}", "导出成功");
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
