using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class PoolSelectViewModel : ViewModelBase {
        private CoinViewModel _coin;
        private PoolViewModel _selectedResult;
        private readonly Action<PoolViewModel> _onSelectedChanged;

        public ICommand HideView { get; set; }

        public PoolSelectViewModel(CoinViewModel coin, PoolViewModel selected, Action<PoolViewModel> onSelectedChanged) {
            _coin = coin;
            _selectedResult = selected;
            _onSelectedChanged = onSelectedChanged;
        }

        public AppContext AppContext {
            get {
                return AppContext.Current;
            }
        }

        public PoolViewModel SelectedResult {
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

        public List<PoolViewModel> QueryResults {
            get {
                return Coin.Pools.OrderBy(a => a.SortNumber).ToList();
            }
        }
    }
}
