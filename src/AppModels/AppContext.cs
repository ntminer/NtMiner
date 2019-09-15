using NTMiner.Bus;
using NTMiner.Vms;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public partial class AppContext {
        public static readonly AppContext Instance = new AppContext();

        public static ExtendedNotifyIcon NotifyIcon;
        public static Action<RemoteDesktopInput> RemoteDesktop;

        private static readonly List<IDelegateHandler> _contextHandlers = new List<IDelegateHandler>();

        private AppContext() {
        }

        #region static methods
        /// <summary>
        /// 命令窗口。使用该方法的代码行应将前两个参数放在第一行以方便vs查找引用时展示出参数信息
        /// </summary>
        public static DelegateHandler<TCmd> Window<TCmd>(string description, LogEnum logType, Action<TCmd> action)
            where TCmd : ICmd {
            return VirtualRoot.Path(description, logType, action).AddToCollection(_contextHandlers);
        }

        /// <summary>
        /// 事件响应
        /// </summary>
        public static DelegateHandler<TEvent> On<TEvent>(string description, LogEnum logType, Action<TEvent> action)
            where TEvent : IEvent {
            return VirtualRoot.Path(description, logType, action).AddToCollection(_contextHandlers);
        }

        public static void Enable() {
            foreach (var handler in _contextHandlers) {
                handler.HandlerId.IsEnabled = true;
            }
        }

        public static void Disable() {
            foreach (var handler in _contextHandlers) {
                handler.HandlerId.IsEnabled = false;
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

        public PackageViewModels PackageVms {
            get {
                return PackageViewModels.Instance;
            }
        }

        public CoinGroupViewModels CoinGroupVms {
            get {
                return CoinGroupViewModels.Instance;
            }
        }

        public FileWriterViewModels FileWriterVms {
            get {
                return FileWriterViewModels.Instance;
            }
        }

        public FragmentWriterViewModels FragmentWriterVms {
            get {
                return FragmentWriterViewModels.Instance;
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
    }
}
