using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class PoolSelectViewModel : ViewModelBase {
        private CoinViewModel _coin;
        private PoolViewModel _selectedResult;
        public readonly Action<PoolViewModel> OnOk;

        public ICommand HideView { get; set; }

        public PoolSelectViewModel(CoinViewModel coin, PoolViewModel selected, Action<PoolViewModel> onOk) {
            _coin = coin;
            _selectedResult = selected;
            OnOk = onOk;
        }

        public PoolViewModel SelectedResult {
            get => _selectedResult;
            set {
                if (_selectedResult != value) {
                    _selectedResult = value;
                    OnPropertyChanged(nameof(SelectedResult));
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
