using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class ChartsWindowViewModel : ViewModelBase {
        private List<ChartViewModel> _chartVms;
        private int _totalMiningCount;
        private int _totalOnlineCount;

        public ChartsWindowViewModel() {
        }

        public bool IsAutoCloseServices {
            get => NTMinerRegistry.GetIsAutoCloseServices();
            set {
                NTMinerRegistry.SetIsAutoCloseServices(value);
                OnPropertyChanged(nameof(IsAutoCloseServices));
            }
        }

        public LangViewModels LangVms {
            get {
                return LangViewModels.Current;
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
                    foreach (var coinVm in MinerClientsWindowViewModel.Current.MineCoinVms.AllCoins.OrderBy(a => a.SortNumber)) {
                        _chartVms.Add(new ChartViewModel(coinVm, new CoinSnapshotDataViewModel(new MinerServer.CoinSnapshotData {
                            CoinCode = coinVm.Code,
                            MainCoinMiningCount = 0,
                            MainCoinOnlineCount = 0,
                            DualCoinMiningCount = 0,
                            DualCoinOnlineCount = 0,
                            ShareDelta = 0,
                            RejectShareDelta = 0,
                            Speed = 0,
                            Timestamp = DateTime.MinValue
                        })));
                    }
                }
                return _chartVms;
            }
        }
    }
}
