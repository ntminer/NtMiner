using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class MinersSpeedChartsViewModel : ViewModelBase {
        private List<ControlCenterChartViewModel> _totalVms;
        private int _totalMiningCount;
        private int _totalOnlineCount;

        public int TotalMiningCount {
            get => _totalMiningCount;
            set {
                _totalMiningCount = value;
                OnPropertyChanged(nameof(TotalMiningCount));
            }
        }

        public int TotalOnlineCount {
            get => _totalOnlineCount;
            set {
                _totalOnlineCount = value;
                OnPropertyChanged(nameof(TotalOnlineCount));
            }
        }

        public List<ControlCenterChartViewModel> TotalVms {
            get {
                if (_totalVms == null) {
                    _totalVms = new List<ControlCenterChartViewModel>();
                    foreach (var coinVm in MinerClientsViewModel.Current.MineCoinVms.AllCoins.OrderBy(a => a.SortNumber)) {
                        _totalVms.Add(new ControlCenterChartViewModel(coinVm));
                    }
                }
                return _totalVms;
            }
        }
    }
}
