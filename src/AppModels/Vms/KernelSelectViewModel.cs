using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelSelectViewModel : ViewModelBase {
        private string _keyword;
        private CoinViewModel _coin;
        private KernelViewModel _selectedResult;
        public readonly Action<KernelViewModel> OnOk;

        public ICommand ClearKeyword { get; private set; }
        public ICommand HideView { get; set; }

        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public KernelSelectViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public KernelSelectViewModel(CoinViewModel coin, KernelViewModel selectedKernel, Action<KernelViewModel> onOk) {
            _coin = coin;
            _selectedResult = selectedKernel;
            OnOk = onOk;
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
                IQueryable<KernelViewModel> query = AppContext.Instance.KernelVms.AllKernels.Where(a => !a.SupportedCoinVms.Contains(Coin))
                    .Where(a => a.PackageVm.AlgoIds.Contains(this.Coin.AlgoId)).AsQueryable();
                if (!WpfUtil.IsDevMode) {
                    query = query.Where(a => a.PublishState == PublishStatus.Published);
                }
                if (!string.IsNullOrEmpty(Keyword)) {
                    query = query.
                        Where(a => (!string.IsNullOrEmpty(a.Code) && a.Code.IgnoreCaseContains(Keyword))
                            || (!string.IsNullOrEmpty(a.Version) && a.Version.IgnoreCaseContains(Keyword))
                            || (!string.IsNullOrEmpty(a.Notice) && a.Notice.IgnoreCaseContains(Keyword)));
                }
                return query.OrderByDescending(a => a.Code + a.Version).ToList();
            }
        }
    }
}
