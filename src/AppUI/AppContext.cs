using NTMiner.Vms;

namespace NTMiner {
    public class AppContext {
        /// <summary>
        /// 这个可以在关闭界面的时候释放
        /// </summary>
        public static readonly AppContext Current = new AppContext();

        private AppContext() {
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
    }
}
