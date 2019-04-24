using System.Linq;

namespace NTMiner.Vms {
    public class OverClockDataPageViewModel : ViewModelBase {
        private CoinViewModel _currentCoin;

        public OverClockDataPageViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            _currentCoin = CoinVms.MainCoins.FirstOrDefault();
        }

        public CoinViewModels CoinVms {
            get {
                return CoinViewModels.Current;
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
