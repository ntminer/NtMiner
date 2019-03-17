using NTMiner.Wpf;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelSelectViewModel : ViewModelBase {
        private string _keyword;
        private CoinViewModel _exceptedCoin;
        private KernelViewModel _selectedResult;

        public ICommand ClearKeyword { get; private set; }

        public KernelSelectViewModel() {
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
                    TopWindow.GetTopWindow()?.Close();
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

        public CoinViewModel ExceptedCoin {
            get => _exceptedCoin;
            set {
                if (_exceptedCoin != value) {
                    _exceptedCoin = value;
                    OnPropertyChanged(nameof(ExceptedCoin));
                    OnPropertyChanged(nameof(QueryResults));
                }
            }
        }

        public List<KernelViewModel> QueryResults {
            get {
                IQueryable<KernelViewModel> query = KernelViewModels.Current.AllKernels.AsQueryable();
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
                if (ExceptedCoin != null) {
                    query = query.Where(a => !a.SupportedCoinVms.Contains(ExceptedCoin));
                }
                return query.OrderBy(a => a.Code + a.Version).ToList();
            }
        }
    }
}
