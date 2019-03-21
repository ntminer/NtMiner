using System.Linq;

namespace NTMiner.Vms {
    public class OverClockDataPageViewModel : ViewModelBase {
        public static readonly OverClockDataPageViewModel Current = new OverClockDataPageViewModel();
        private CoinViewModel _currentCoin;

        private OverClockDataPageViewModel() {
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
