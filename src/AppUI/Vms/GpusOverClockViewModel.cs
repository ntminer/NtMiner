using NTMiner.Core;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class GpusOverClockViewModel : ViewModelBase {
        public static readonly GpusOverClockViewModel Current = new GpusOverClockViewModel();
        private CoinViewModel _currentCoin;

        public ICommand Apply { get; private set; }

        private GpusOverClockViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            this.Apply = new DelegateCommand(() => {
                var list = GpuOverClockVms.ToArray();
                foreach (var item in list) {
                    VirtualRoot.Execute(new AddOrUpdateGpuOverClockDataCommand(item));
                }
                if (GpuAllOverClockDataVm.IsEnabled) {
                    VirtualRoot.Execute(new OverClockCommand(GpuAllOverClockDataVm));
                }
                else {
                    foreach (var item in GpuOverClockVms) {
                        if (item.IsEnabled && item.Index != NTMinerRoot.GpuAllId) {
                            VirtualRoot.Execute(new OverClockCommand(item));
                        }
                    }
                }
            });
            _currentCoin = MinerProfileViewModel.Current.CoinVm;
            if (_currentCoin == null) {
                _currentCoin = CoinViewModels.Current.MainCoins.FirstOrDefault();
            }
        }

        public CoinViewModels CoinVms {
            get {
                return CoinViewModels.Current;
            }
        }

        public CoinViewModel CurrentCoin {
            get => _currentCoin;
            set {
                _currentCoin = value;
                OnPropertyChanged(nameof(CurrentCoin));
                OnPropertyChanged(nameof(GpuAllOverClockDataVm));
                OnPropertyChanged(nameof(GpuOverClockVms));
            }
        }

        public GpuOverClockDataViewModel GpuAllOverClockDataVm {
            get {
                if (CurrentCoin == null) {
                    return null;
                }
                return GpuOverClockDataViewModels.Current.GpuAllVm(CurrentCoin.Id);
            }
        }

        public List<GpuOverClockDataViewModel> GpuOverClockVms {
            get {
                if (CurrentCoin == null) {
                    return null;
                }
                return GpuOverClockDataViewModels.Current.List(CurrentCoin.Id);
            }
        }
    }
}
