using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class ChartsWindowViewModel : ViewModelBase {
        private List<ChartViewModel> _chartVms;
        private int _totalMiningCount;
        private int _totalOnlineCount;

        public ChartsWindowViewModel() {
        }

        public bool IsShowAll {
            get {
                return ChartVms.All(a => a.IsShow);
            }
            set {
                foreach (var item in ChartVms) {
                    item.IsShow = value;
                }
                OnPropertyChanged(nameof(IsShowAll));
            }
        }

        public int TotalMiningCount {
            get => _totalMiningCount;
            set {
                if (_totalMiningCount != value) {
                    _totalMiningCount = value;
                    OnPropertyChanged(nameof(TotalMiningCount));
                }
            }
        }

        public int TotalOnlineCount {
            get => _totalOnlineCount;
            set {
                if (_totalOnlineCount != value) {
                    _totalOnlineCount = value;
                    OnPropertyChanged(nameof(TotalOnlineCount));
                }
            }
        }

        public List<ChartViewModel> ChartVms {
            get {
                if (_chartVms == null) {
                    _chartVms = new List<ChartViewModel>();
                    foreach (var coinVm in AppContext.Instance.MinerClientsWindowVm.MineCoinVms.AllCoins.OrderBy(a => a.SortNumber)) {
                        _chartVms.Add(new ChartViewModel(coinVm));
                    }
                }
                return _chartVms;
            }
        }
    }
}
