using System.Linq;

namespace NTMiner.Vms {
    public class OverClockDataPageViewModel : ViewModelBase {
        public static readonly OverClockDataPageViewModel Current = new OverClockDataPageViewModel();
        private CoinViewModel _currentCoin;

        private OverClockDataPageViewModel() {
            _currentCoin = Vm.Root.CoinVms.MainCoins.FirstOrDefault();
        }

        public Vm Vm {
            get {
                return Vm.Instance;
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
