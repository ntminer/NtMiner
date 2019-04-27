using NTMiner.Vms;

namespace NTMiner {
    public class MinerClientAppContext {
        /// <summary>
        /// 这个可以在关闭界面的时候释放
        /// </summary>
        public static readonly MinerClientAppContext Current = new MinerClientAppContext();

        private MinerClientAppContext() {
        }

        public GpuSpeedViewModels GpuSpeedVms { get; private set; } = new GpuSpeedViewModels();

        public StartStopMineButtonViewModel StartStopMineButtonVm {
            get; private set;
        } = new StartStopMineButtonViewModel();

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
    }
}
