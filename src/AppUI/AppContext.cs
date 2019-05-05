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
        public static readonly AppContext Instance = new AppContext();

        private static readonly List<IDelegateHandler> _contextHandlers = new List<IDelegateHandler>();

        private AppContext() {
        }

        #region static methods
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

        public static void Enable() {
            foreach (var handler in _contextHandlers) {
                handler.IsEnabled = true;
            }
        }

        public static void Disable() {
            foreach (var handler in _contextHandlers) {
                handler.IsEnabled = false;
            }
        }
        #endregion

        #region context
        public MinerClientsWindowViewModel MinerClientsWindowVm {
            get {
                return MinerClientsWindowViewModel.Instance;
            }
        }

        public MinerProfileViewModel MinerProfileVm {
            get {
                return MinerProfileViewModel.Instance;
            }
        }

        public CoinViewModels CoinVms {
            get {
                return CoinViewModels.Instance;
            }
        }

        public GpuSpeedViewModels GpuSpeedVms {
            get {
                return GpuSpeedViewModels.Instance;
            }
        }

        public StartStopMineButtonViewModel StartStopMineButtonVm {
            get {
                return StartStopMineButtonViewModel.Instance;
            }
        }

        public PoolKernelViewModels PoolKernelVms {
            get {
                return PoolKernelViewModels.Instance;
            }
        }

        public CoinGroupViewModels CoinGroupVms {
            get {
                return CoinGroupViewModels.Instance;
            }
        }

        public CoinKernelViewModels CoinKernelVms {
            get {
                return CoinKernelViewModels.Instance;
            }
        }

        public CoinProfileViewModels CoinProfileVms {
            get {
                return CoinProfileViewModels.Instance;
            }
        }

        public CoinSnapshotDataViewModels CoinSnapshotDataVms {
            get {
                return CoinSnapshotDataViewModels.Instance;
            }
        }

        public ColumnsShowViewModels ColumnsShowVms {
            get {
                return ColumnsShowViewModels.Instance;
            }
        }

        public DriveSetViewModel DriveSetVm {
            get {
                return DriveSetViewModel.Instance;
            }
        }

        public VirtualMemorySetViewModel VirtualMemorySetVm {
            get {
                return VirtualMemorySetViewModel.Instance;
            }
        }

        public GpuProfileViewModels GpuProfileVms {
            get {
                return GpuProfileViewModels.Instance;
            }
        }

        public GpuViewModels GpuVms {
            get {
                return GpuViewModels.Instance;
            }
        }

        public GroupViewModels GroupVms {
            get {
                return GroupViewModels.Instance;
            }
        }

        public KernelInputViewModels KernelInputVms {
            get {
                return KernelInputViewModels.Instance;
            }
        }

        public KernelOutputFilterViewModels KernelOutputFilterVms {
            get {
                return KernelOutputFilterViewModels.Instance;
            }
        }

        public KernelOutputTranslaterViewModels KernelOutputTranslaterVms {
            get {
                return KernelOutputTranslaterViewModels.Instance;
            }
        }

        public KernelOutputViewModels KernelOutputVms {
            get {
                return KernelOutputViewModels.Instance;
            }
        }

        public KernelViewModels KernelVms {
            get {
                return KernelViewModels.Instance;
            }
        }

        public MinerGroupViewModels MinerGroupVms {
            get {
                return MinerGroupViewModels.Instance;
            }
        }

        public MineWorkViewModels MineWorkVms {
            get {
                return MineWorkViewModels.Instance;
            }
        }

        public OverClockDataViewModels OverClockDataVms {
            get {
                return OverClockDataViewModels.Instance;
            }
        }

        public PoolProfileViewModels PoolProfileVms {
            get {
                return PoolProfileViewModels.Instance;
            }
        }

        public PoolViewModels PoolVms {
            get {
                return PoolViewModels.Instance;
            }
        }

        public ShareViewModels ShareVms {
            get {
                return ShareViewModels.Instance;
            }
        }

        public WalletViewModels WalletVms {
            get {
                return WalletViewModels.Instance;
            }
        }

        public UserViewModels UserVms {
            get {
                return UserViewModels.Instance;
            }
        }

        public SysDicViewModels SysDicVms {
            get {
                return SysDicViewModels.Instance;
            }
        }

        public SysDicItemViewModels SysDicItemVms {
            get {
                return SysDicItemViewModels.Instance;
            }
        }

        public GpuStatusBarViewModel GpuStatusBarVm {
            get {
                return GpuStatusBarViewModel.Instance;
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
                            Key = Instance.ServerJsonFileName,
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
