using System.Linq;

namespace NTMiner.Vms {
    public class NTMinerWalletPageViewModel : ViewModelBase {
        private CoinViewModel _currentCoin;

        public NTMinerWalletPageViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            _currentCoin = CoinVms.MainCoins.FirstOrDefault();
        }

        public AppContext.CoinViewModels CoinVms {
            get {
                return AppContext.Instance.CoinVms;
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
