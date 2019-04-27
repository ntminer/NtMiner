using NTMiner.Vms;
using System.Windows;

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
    }
}
