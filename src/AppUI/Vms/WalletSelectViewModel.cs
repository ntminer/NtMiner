using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class WalletSelectViewModel : ViewModelBase {
        private CoinViewModel _coin;
        private readonly bool _isDualCoin;
        private WalletViewModel _selectedResult;
        private readonly Action<WalletViewModel> _onSelectedChanged;

        public ICommand AddWallet { get; private set; }
        public ICommand HideView { get; set; }

        public WalletSelectViewModel(CoinViewModel coin, bool isDualCoin, WalletViewModel selected, Action<WalletViewModel> onSelectedChanged) {
            _coin = coin;
            _isDualCoin = isDualCoin;
            _selectedResult = selected;
            _onSelectedChanged = onSelectedChanged;
            if (_isDualCoin) {
                this.AddWallet = _coin.CoinProfile.AddDualCoinWallet;
            }
            else {
                this.AddWallet = _coin.CoinProfile.AddWallet;
            }
        }

        public AppContext AppContext {
            get {
                return AppContext.Current;
            }
        }

        public WalletViewModel SelectedResult {
            get => _selectedResult;
            set {
                if (_selectedResult != value) {
                    _selectedResult = value;
                    OnPropertyChanged(nameof(SelectedResult));
                    _onSelectedChanged?.Invoke(value);
                }
            }
        }

        public CoinViewModel Coin {
            get => _coin;
            set {
                if (_coin != value) {
                    _coin = value;
                    OnPropertyChanged(nameof(Coin));
                    OnPropertyChanged(nameof(QueryResults));
                }
            }
        }

        public List<WalletViewModel> QueryResults {
            get {
                return Coin.Wallets.OrderBy(a => a.SortNumber).ToList();
            }
        }
    }
}
