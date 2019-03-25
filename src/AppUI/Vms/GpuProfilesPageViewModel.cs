using NTMiner.JsonDb;
using NTMiner.MinerClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class GpuProfilesPageViewModel : ViewModelBase {
        private Geometry _gpuIcon;
        private string _gpuIconFill = "Gray";
        private string _redText;
        private CoinViewModel _coinVm;
        private GpuProfilesJsonDb _data;
        private bool _isEnabled = false;
        private Visibility _isMinerClientVmVisible = Visibility.Collapsed;
        private readonly MinerClientsWindowViewModel _minerClientsWindowVm;
        private readonly MinerClientViewModel _minerClientVm;

        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public GpuProfilesPageViewModel(MinerClientsWindowViewModel minerClientsWindowVm) {
            _minerClientsWindowVm = minerClientsWindowVm;
            if (minerClientsWindowVm.SelectedMinerClients == null && minerClientsWindowVm.SelectedMinerClients.Length != 1) {
                throw new InvalidProgramException();
            }
            _minerClientVm = minerClientsWindowVm.SelectedMinerClients[0];
            this.Save = new DelegateCommand(() => {
                if (_data == null) {
                    return;
                }
                GpuProfilesJsonDb jsonObj = new GpuProfilesJsonDb() {
                    Gpus = _data.Gpus
                };
                foreach (var coinVm in CoinVms.MainCoins) {
                    if (coinVm.IsOverClockEnabled) {
                        jsonObj.CoinOverClocks.Add(new CoinOverClockData() {
                            CoinId = coinVm.Id,
                            IsOverClockEnabled = coinVm.IsOverClockEnabled,
                            IsOverClockGpuAll = coinVm.IsOverClockGpuAll
                        });
                        if (CoinVm.IsOverClockGpuAll) {
                            jsonObj.GpuProfiles.Add(new GpuProfileData(coinVm.GpuAllProfileVm));
                        }
                        jsonObj.GpuProfiles.AddRange(coinVm.GpuProfileVms.Select(a => new GpuProfileData(a)));
                    }
                }
                string json = VirtualRoot.JsonSerializer.Serialize(jsonObj);
                foreach (var client in minerClientsWindowVm.SelectedMinerClients) {
                    Client.NTMinerDaemonService.SaveGpuProfilesJsonAsync(client.MinerIp, json);
                }
                NotiCenterWindowViewModel.Current.Manager.ShowSuccessMessage("应用成功，请观察效果");
                CloseWindow?.Invoke();
            });
            Client.NTMinerDaemonService.GetGpuProfilesJsonAsync(_minerClientVm.MinerIp, (data, e) => {
                _data = data;
                if (e != null) {
                    Write.UserLine(e.Message, System.ConsoleColor.Red);
                }
                else if (data != null) {
                    string iconName;
                    switch (_minerClientVm.GpuType) {
                        case GpuType.NVIDIA:
                            iconName = "Icon_Nvidia";
                            GpuIconFill = "Green";
                            RedText = "超频有风险，操作需谨慎";
                            IsEnabled = true;
                            break;
                        case GpuType.AMD:
                            iconName = "Icon_AMD";
                            GpuIconFill = "Red";
                            RedText = "暂不支持A卡超频";
                            IsEnabled = false;
                            break;
                        case GpuType.Empty:
                        default:
                            iconName = "Icon_GpuEmpty";
                            GpuIconFill = "Gray";
                            RedText = "挖矿端没有显卡";
                            IsEnabled = false;
                            break;
                    }
                    GpuIcon = (Geometry)System.Windows.Application.Current.Resources[iconName];
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

        public MinerClientViewModel MinerClientVm {
            get {
                return _minerClientVm;
            }
        }

        public Visibility IsMinerClientVmVisible {
            get => _isMinerClientVmVisible;
            set {
                _isMinerClientVmVisible = value;
                OnPropertyChanged(nameof(IsMinerClientVmVisible));
            }
        }

        public MinerClientsWindowViewModel MinerClientsWindowVm {
            get {
                return _minerClientsWindowVm;
            }
        }

        public bool IsEnabled {
            get => _isEnabled;
            set {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        public CoinViewModels CoinVms {
            get {
                return CoinViewModels.Current;
            }
        }

        public CoinViewModel CoinVm {
            get { return _coinVm; }
            set {
                _coinVm = value;
                OnPropertyChanged(nameof(CoinVm));
            }
        }

        public string RedText {
            get => _redText;
            set {
                _redText = value;
                OnPropertyChanged(nameof(RedText));
            }
        }

        public Geometry GpuIcon {
            get {
                return _gpuIcon;
            }
            set {
                _gpuIcon = value;
                OnPropertyChanged(nameof(GpuIcon));
            }
        }

        public string GpuIconFill {
            get {
                return _gpuIconFill;
            }
            set {
                _gpuIconFill = value;
                OnPropertyChanged(nameof(GpuIconFill));
            }
        }
    }
}
