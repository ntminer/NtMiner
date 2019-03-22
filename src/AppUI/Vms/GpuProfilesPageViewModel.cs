using NTMiner.MinerServer;
using System.Linq;

namespace NTMiner.Vms {
    public class GpuProfilesPageViewModel : ViewModelBase {
        public GpuProfilesPageViewModel(IClientData client) {
            if (client != null) {
                Client.NTMinerDaemonService.GetGpuProfilesJsonAsync(client.MinerIp, (data, e) => {
                    if (e != null) {
                        Write.UserLine(e.Message, System.ConsoleColor.Red);
                    }
                    else if (data != null) {
                        foreach (var coinOverClock in data.CoinOverClocks) {
                            CoinViewModel coinVm;
                            if (CoinViewModels.Current.TryGetCoinVm(coinOverClock.CoinId, out coinVm)) {
                                coinVm.IsOverClockEnabled = coinOverClock.IsOverClockEnabled;
                                coinVm.IsOverClockGpuAll = coinOverClock.IsOverClockGpuAll;
                                var gpuAllProfile = data.GpuProfiles.FirstOrDefault(a => a.CoinId == coinVm.Id && a.Index == NTMinerRoot.GpuAllId);
                                if (gpuAllProfile == null) {
                                    gpuAllProfile = new MinerClient.GpuProfileData(coinVm.Id, NTMinerRoot.GpuAllId);
                                }
                                coinVm.GpuAllProfileVm = new GpuProfileViewModel(gpuAllProfile);
                                coinVm.GpuProfileVms = data.GpuProfiles.Where(a => a.CoinId == coinVm.Id && a.Index != NTMinerRoot.GpuAllId).OrderBy(a => a.Index).Select(a => new GpuProfileViewModel(a)).ToList();
                            }
                        }
                        this.CurrentCoin = CoinVms.MainCoins.FirstOrDefault(a => a.IsOverClockEnabled);
                        if (this.CurrentCoin == null) {
                            this.CurrentCoin = CoinVms.MainCoins.FirstOrDefault();
                        }
                    }
                });
            }
        }

        public CoinViewModels CoinVms {
            get {
                return CoinViewModels.Current;
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
