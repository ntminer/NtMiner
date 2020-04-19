using NTMiner.Vms;
using System.Linq;

namespace NTMiner.MinerStudio.Vms {
    public class OverClockDataPageViewModel : ViewModelBase {
        private CoinViewModel _currentCoin;

        public OverClockDataPageViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            _currentCoin = CoinVms.MainCoins.FirstOrDefault();
        }

        public AppRoot.CoinViewModels CoinVms {
            get {
                return AppRoot.CoinVms;
            }
        }

        public CoinViewModel CurrentCoin {
            get => _currentCoin;
            set {
                _currentCoin = value;
                OnPropertyChanged(nameof(CurrentCoin));
            }
        }
    }
}
