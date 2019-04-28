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

        public AppContext.CoinViewModels CoinVms {
            get {
                return AppContext.Current.CoinVms;
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
