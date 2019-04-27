using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class CoinKernelSelectViewModel : ViewModelBase {
        private string _keyword;
        private CoinViewModel _coin;
        private CoinKernelViewModel _selectedResult;
        private readonly Action<CoinKernelViewModel> _onSelectedChanged;

        public ICommand ClearKeyword { get; private set; }
        public ICommand HideView { get; set; }

        public CoinKernelSelectViewModel(CoinViewModel coin, CoinKernelViewModel selected, Action<CoinKernelViewModel> onSelectedChanged) {
            _coin = coin;
            _selectedResult = selected;
            _onSelectedChanged = onSelectedChanged;
            this.ClearKeyword = new DelegateCommand(() => {
                this.Keyword = string.Empty;
            });
        }

        public CoinKernelViewModel SelectedResult {
            get => _selectedResult;
            set {
                if (_selectedResult != value) {
                    _selectedResult = value;
                    OnPropertyChanged(nameof(SelectedResult));
                    _onSelectedChanged?.Invoke(value);
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

        public List<CoinKernelViewModel> QueryResults {
            get {
                IQueryable<CoinKernelViewModel> query = Coin.CoinKernels.Where(a => a.Kernel != null && a.IsSupported).OrderBy(a => a.SortNumber).AsQueryable();
                if (!IsDebugMode) {
                    query = query.Where(a => a.Kernel.PublishState == PublishStatus.Published);
                }
                if (!string.IsNullOrEmpty(Keyword)) {
                    string keyword = this.Keyword.ToLower();
                    query = query.
                        Where(a => (!string.IsNullOrEmpty(a.Kernel.Code) && a.Kernel.Code.ToLower().Contains(keyword))
                            || (!string.IsNullOrEmpty(a.Kernel.Version) && a.Kernel.Version.ToLower().Contains(keyword))
                            || (!string.IsNullOrEmpty(a.Kernel.Notice) && a.Kernel.Notice.ToLower().Contains(keyword)));
                }
                return query.ToList();
            }
        }
    }
}
