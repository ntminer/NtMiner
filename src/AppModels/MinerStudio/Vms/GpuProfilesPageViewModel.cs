using NTMiner.Core;
using NTMiner.Core.MinerClient;
using NTMiner.Core.Profile;
using NTMiner.JsonDb;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.MinerStudio.Vms {
    public class GpuProfilesPageViewModel : ViewModelBase {
        public readonly Guid Id = Guid.NewGuid();
        private Geometry _gpuIcon;
        private string _gpuIconFill = "Gray";
        private string _redText;
        private CoinViewModel _coinVm;
        private GpuProfilesJsonDb _data;
        private bool _isEnabled = false;
        private readonly MinerClientViewModel _minerClientVm;
        private readonly ObservableCollection<MinerClientViewModel> _minerClientVms;

        public ICommand Save { get; private set; }

        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public GpuProfilesPageViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(NTKeyword.WpfDesignOnly);
            }
        }

        public GpuProfilesPageViewModel(MinerClientsWindowViewModel minerClientsWindowVm) {
            if (minerClientsWindowVm.SelectedMinerClients == null || minerClientsWindowVm.SelectedMinerClients.Length == 0) {
                throw new InvalidProgramException();
            }
            _minerClientVm = minerClientsWindowVm.SelectedMinerClients[0];
            _minerClientVms = new ObservableCollection<MinerClientViewModel>(minerClientsWindowVm.SelectedMinerClients);
            if (AppRoot.CoinVms.TryGetCoinVm(_minerClientVm.MainCoinCode, out CoinViewModel outCoinVm)) {
                this._coinVm = outCoinVm;
            }
            if (this._coinVm == null) {
                this._coinVm = CoinVms.MainCoins.FirstOrDefault();
            }
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
                            jsonObj.GpuProfiles.Add(new GpuProfileData().Update(coinVm.GpuAllProfileVm));
                        }
                        jsonObj.GpuProfiles.AddRange(coinVm.GpuProfileVms.Select(a => new GpuProfileData().Update(a)));
                    }
                }
                string json = VirtualRoot.JsonSerializer.Serialize(jsonObj);
                foreach (var client in minerClientsWindowVm.SelectedMinerClients) {
                    MinerStudioRoot.MinerStudioService.SaveGpuProfilesJsonAsync(client, json);
                }
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
            });
        }

        public void SetData(GpuProfilesJsonDb data) {
            _data = data;
            if (data != null) {
                string iconName;
                GpuType gpuType = _minerClientVm.GpuType;
                if (gpuType == GpuType.Empty) {
                    gpuType = data.GpuType;
                }
                IsEnabled = data.Gpus != null && data.Gpus.Length != 0;
                RedText = "超频有风险，操作需谨慎";
                switch (_minerClientVm.GpuType) {
                    case GpuType.NVIDIA:
                        iconName = "Icon_Nvidia";
                        GpuIconFill = "Red";
                        break;
                    case GpuType.AMD:
                        iconName = "Icon_Amd";
                        GpuIconFill = "Green";
                        break;
                    case GpuType.Empty:
                    default:
                        iconName = "Icon_GpuEmpty";
                        GpuIconFill = "Gray";
                        break;
                }
                GpuIcon = AppUtil.GetResource<Geometry>(iconName);
                foreach (var coinVm in AppRoot.CoinVms.MainCoins) {
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
                        if (gpu.Index == NTMinerContext.GpuAllId) {
                            gpuAllProfileVm = new GpuProfileViewModel(gpuProfile, gpuVm);
                        }
                        else {
                            gpuProfileVms.Add(new GpuProfileViewModel(gpuProfile, gpuVm));
                        }
                    }
                    if (gpuAllProfileVm == null) {
                        gpuAllProfileVm = new GpuProfileViewModel(
                            new GpuProfileData(coinVm.Id, NTMinerContext.GpuAllId), new GpuViewModel(new GpuData {
                                GpuType = gpuType,
                                Index = NTMinerContext.GpuAllId,
                                Name = "All"
                            }, data.Gpus));
                    }
                    #endregion
                    coinVm.GpuAllProfileVm = gpuAllProfileVm;
                    coinVm.GpuProfileVms = gpuProfileVms;
                }
            }
        }

        public MinerClientViewModel MinerClientVm {
            get {
                return _minerClientVm;
            }
        }

        public ObservableCollection<MinerClientViewModel> MinerClientVms {
            get {
                return _minerClientVms;
            }
        }

        public bool IsEnabled {
            get => _isEnabled;
            set {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        public AppRoot.CoinViewModels CoinVms {
            get {
                return AppRoot.CoinVms;
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
