using NTMiner.Core.Gpus.Impl;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using System.Collections.Generic;
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
                        foreach (var coinVm in CoinViewModels.Current.MainCoins) {
                            var coinOverClock = data.CoinOverClocks.FirstOrDefault(a => a.CoinId == coinVm.Id);
                            var gpuProfiles = data.GpuProfiles.Where(a => a.CoinId == coinVm.Id).ToArray();
                            if (coinOverClock == null) {
                                coinOverClock = new CoinOverClockData() {
                                    CoinId = coinVm.Id,
                                    IsOverClockEnabled = false,
                                    IsOverClockGpuAll = true
                                };
                            }
                            coinVm.IsOverClockEnabled = coinOverClock.IsOverClockEnabled;
                            coinVm.IsOverClockGpuAll = coinOverClock.IsOverClockGpuAll;
                            List<GpuProfileViewModel> gpuProfileVms = new List<GpuProfileViewModel>();
                            GpuProfileViewModel gpuAllProfileVm = null;
                            #region
                            foreach (var gpu in data.Gpus.OrderBy(a => a.Index)) {
                                var gpuProfile = gpuProfiles.FirstOrDefault(a => a.Index == gpu.Index);
                                if (gpuProfile == null) {
                                    gpuProfile = new GpuProfileData(coinVm.Id, gpu.Index);
                                }
                                var gpuVm = new GpuViewModel(gpu, data.Gpus);
                                if (gpu.Index == NTMinerRoot.GpuAllId) {
                                    gpuAllProfileVm = new GpuProfileViewModel(gpuProfile, gpuVm);
                                }
                                else {
                                    gpuProfileVms.Add(new GpuProfileViewModel(gpuProfile, gpuVm));
                                }
                            }
                            if (gpuAllProfileVm == null) {
                                gpuAllProfileVm = new GpuProfileViewModel(
                                    new GpuProfileData(coinVm.Id, NTMinerRoot.GpuAllId), new GpuViewModel(new GpuData {
                                        Index = NTMinerRoot.GpuAllId,
                                        Name = "All",
                                        CoreClockDeltaMax = 0,
                                        CoreClockDeltaMin = 0,
                                        MemoryClockDeltaMax = 0,
                                        MemoryClockDeltaMin = 0
                                    }, data.Gpus));
                            }
                            #endregion
                            coinVm.GpuAllProfileVm = gpuAllProfileVm;
                            coinVm.GpuProfileVms = gpuProfileVms;
                        }
                        this.CoinVm = CoinVms.MainCoins.FirstOrDefault(a => a.IsOverClockEnabled);
                        if (this.CoinVm == null) {
                            this.CoinVm = CoinVms.MainCoins.FirstOrDefault();
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

        private CoinViewModel _coinVm;
        public CoinViewModel CoinVm {
            get { return _coinVm; }
            set {
                _coinVm = value;
                OnPropertyChanged(nameof(CoinVm));
            }
        }
    }
}
