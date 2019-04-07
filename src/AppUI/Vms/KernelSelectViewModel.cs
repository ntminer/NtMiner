using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelSelectViewModel : ViewModelBase {
        private string _keyword;
        private CoinViewModel _coin;
        private KernelViewModel _selectedResult;
        private readonly bool _isExceptedCoin;
        private readonly Action<KernelViewModel> _onSelectedKernelChanged;

        public ICommand ClearKeyword { get; private set; }

        public KernelSelectViewModel(CoinViewModel coin, bool isExceptedCoin, KernelViewModel selectedKernel, Action<KernelViewModel> onSelectedKernelChanged) {
            _coin = coin;
            _selectedResult = selectedKernel;
            _onSelectedKernelChanged = onSelectedKernelChanged;
            _isExceptedCoin = isExceptedCoin;
            this.ClearKeyword = new DelegateCommand(() => {
                this.Keyword = string.Empty;
            });
        }

        public KernelViewModel SelectedResult {
            get => _selectedResult;
            set {
                if (_selectedResult != value) {
                    _selectedResult = value;
                    OnPropertyChanged(nameof(SelectedResult));
                    _onSelectedKernelChanged?.Invoke(value);
                }
            }
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

        public List<KernelViewModel> QueryResults {
            get {
                IQueryable<KernelViewModel> query;
                if (_isExceptedCoin) {
                    query = KernelViewModels.Current.AllKernels.Where(a => !a.SupportedCoinVms.Contains(Coin)).AsQueryable();
                }
                else {
                    query = Coin.CoinKernels.OrderBy(a => a.SortNumber).Select(a => a.Kernel).AsQueryable();
                }
                if (!AppStatic.IsDebugMode) {
                    query = query.Where(a => a.PublishState == PublishStatus.Published);
                }
                if (!string.IsNullOrEmpty(Keyword)) {
                    string keyword = this.Keyword.ToLower();
                    query = query.
                        Where(a => (!string.IsNullOrEmpty(a.Code) && a.Code.ToLower().Contains(keyword))
                            || (!string.IsNullOrEmpty(a.Version) && a.Version.ToLower().Contains(keyword))
                            || (!string.IsNullOrEmpty(a.Notice) && a.Notice.ToLower().Contains(keyword)));
                }
                if (_isExceptedCoin) {
                    return query.OrderByDescending(a => a.Code + a.Version).ToList();
                }
                else {
                    return query.ToList();
                }
            }
        }
    }
}
