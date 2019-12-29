using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class CoinKernelSelectViewModel : ViewModelBase {
        private string _keyword;
        private CoinViewModel _coin;
        private CoinKernelViewModel _selectedResult;
        public readonly Action<CoinKernelViewModel> OnOk;

        public ICommand ClearKeyword { get; private set; }
        public ICommand HideView { get; set; }

        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public CoinKernelSelectViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public CoinKernelSelectViewModel(CoinViewModel coin, CoinKernelViewModel selected, Action<CoinKernelViewModel> onOk) {
            _coin = coin;
            _selectedResult = selected;
            OnOk = onOk;
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
                IQueryable<CoinKernelViewModel> query = Coin.CoinKernels
                    .Where(a => a.Kernel != null && a.IsSupported)
                    .OrderBy(a => a.Kernel.Code)
                    .ThenByDescending(a => a.Kernel.Version).AsQueryable();
                if (!WpfUtil.IsDevMode) {
                    query = query.Where(a => a.Kernel.PublishState == PublishStatus.Published);
                }
                if (!string.IsNullOrEmpty(Keyword)) {
                    query = query.
                        Where(a => (!string.IsNullOrEmpty(a.Kernel.Code) && a.Kernel.Code.IgnoreCaseContains(Keyword))
                            || (!string.IsNullOrEmpty(a.Kernel.Version) && a.Kernel.Version.IgnoreCaseContains(Keyword))
                            || (!string.IsNullOrEmpty(a.Kernel.Notice) && a.Kernel.Notice.IgnoreCaseContains(Keyword)));
                }
                return query.ToList();
            }
        }
    }
}
