using NTMiner.MinerServer;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class GpuProfilesPageViewModel : ViewModelBase {
        private List<CoinViewModel> _coinVms;

        public GpuProfilesPageViewModel(IClientData client) {
            if (client != null) {
                Client.MinerClientService.GetGpuProfilesJsonAsync(client.MinerIp, (data, e) => {
                    if (data != null) {
                        List<CoinViewModel> coinVms = new List<CoinViewModel>();
                        foreach (var coinOverClock in data.CoinOverClocks) {
                            CoinViewModel coinVm;
                            if (CoinViewModels.Current.TryGetCoinVm(coinOverClock.CoinId, out coinVm)) {
                                coinVm.IsOverClockEnabled = coinOverClock.IsOverClockEnabled;
                                coinVm.IsOverClockGpuAll = coinOverClock.IsOverClockGpuAll;
                                coinVms.Add(coinVm);
                            }
                        }
                        this.CoinVms = coinVms;
                        this.CurrentCoin = coinVms.FirstOrDefault();
                    }
                });
            }
        }

        public List<CoinViewModel> CoinVms {
            get => _coinVms;
            set {
                _coinVms = value;
                OnPropertyChanged(nameof(CoinVms));
            }
        }

        private CoinViewModel _currentCoin;
        public CoinViewModel CurrentCoin {
            get { return _currentCoin; }
            set {
                _currentCoin = value;
                OnPropertyChanged(nameof(CurrentCoin));
            }
        }
    }
}
