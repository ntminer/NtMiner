using NTMiner.Core.Gpus.Impl;
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
                        foreach (var coinOverClock in data.CoinOverClocks) {
                            if (CoinViewModels.Current.TryGetCoinVm(coinOverClock.CoinId, out CoinViewModel coinVm)) {
                                coinVm.IsOverClockEnabled = coinOverClock.IsOverClockEnabled;
                                coinVm.IsOverClockGpuAll = coinOverClock.IsOverClockGpuAll;
                                List<GpuProfileViewModel> gpuProfileVms = new List<GpuProfileViewModel>();
                                GpuProfileViewModel gpuProfileVm = null;
                                foreach (var gpu in data.Gpus.OrderBy(a => a.Index)) {
                                    var gpuProfileData = data.GpuProfiles.FirstOrDefault(a => a.CoinId == coinVm.Id && a.Index == gpu.Index);
                                    if (gpuProfileData == null) {
                                        gpuProfileData = new MinerClient.GpuProfileData(coinVm.Id, gpu.Index);
                                    }
                                    var gpuVm = new GpuViewModel(new Gpu() {
                                        Index = gpu.Index,
                                        CoreClockDelta = 0,
                                        FanSpeed = 0,
                                        GpuClockDelta = new MinerClient.GpuClockDelta(gpu.CoreClockDeltaMin, gpu.CoreClockDeltaMax, gpu.MemoryClockDeltaMin, gpu.MemoryClockDeltaMax),
                                        MemoryClockDelta = 0,
                                        Name = gpu.Name,
                                        OverClock = new EmptyOverClock(),
                                        PowerUsage = 0,
                                        Temperature = 0
                                    });
                                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                                        gpuProfileVm = new GpuProfileViewModel(gpuProfileData, gpuVm);
                                    }
                                    else {
                                        gpuProfileVms.Add(new GpuProfileViewModel(gpuProfileData, gpuVm));
                                    }
                                }
                                if (gpuProfileVm == null) {
                                    gpuProfileVm = new GpuProfileViewModel(new MinerClient.GpuProfileData(coinVm.Id, NTMinerRoot.GpuAllId), new GpuViewModel(new Gpu() {
                                        Index = NTMinerRoot.GpuAllId,
                                        CoreClockDelta = 0,
                                        FanSpeed = 0,
                                        GpuClockDelta = MinerClient.GpuClockDelta.Empty,
                                        MemoryClockDelta = 0,
                                        Name = "All",
                                        OverClock = new EmptyOverClock(),
                                        PowerUsage = 0,
                                        Temperature = 0
                                    }));
                                }
                                coinVm.GpuAllProfileVm = gpuProfileVm;
                                coinVm.GpuProfileVms = gpuProfileVms;
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
