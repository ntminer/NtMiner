using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class PoolSelectViewModel : ViewModelBase {
        private string _keyword;
        private CoinViewModel _coin;
        private PoolViewModel _selectedResult;
        public readonly Action<PoolViewModel> OnOk;

        public ICommand ClearKeyword { get; private set; }
        public ICommand HideView { get; set; }

        private bool _usedByPool1;
        public PoolSelectViewModel(CoinViewModel coin, PoolViewModel selected, Action<PoolViewModel> onOk, bool usedByPool1 = false) {
            _coin = coin;
            _selectedResult = selected;
            OnOk = onOk;
            _usedByPool1 = usedByPool1;
            this.ClearKeyword = new DelegateCommand(() => {
                this.Keyword = string.Empty;
            });
        }

        public string Keyword {
            get => _keyword;
            set {
                if (_keyword != value) {
                    _keyword = value;
                    OnPropertyChanged(nameof(Keyword));
                    OnPropertyChanged(nameof(QueryResults));
                }
            }
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

        public WalletViewModel Wallet {
            get {
                if (Coin == null) {
                    return null;
                }
                // 主币
                if (Coin == MinerProfileViewModel.Instance.CoinVm) {
                    return Coin.CoinProfile?.SelectedWallet;
                }
                // 辅币
                else if (Coin == MinerProfileViewModel.Instance.CoinVm.CoinKernel.CoinKernelProfile.SelectedDualCoin) {
                    return Coin.CoinProfile?.SelectedDualCoinWallet;
                }
                return null;
            }
        }

        public List<PoolViewModel> QueryResults {
            get {
                var query = Coin.Pools.Where(a =>
                    (a.Name != null && a.Name.IgnoreCaseContains(Keyword)) || a.Server != null && a.Server.IgnoreCaseContains(Keyword));
                if (_usedByPool1) {
                    return query.Where(a => !a.NotPool1).OrderBy(a => a.SortNumber).ToList();
                }
                return query.OrderBy(a => a.SortNumber).ToList();
            }
        }
    }
}
