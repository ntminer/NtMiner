using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class GpusOverClockViewModel : ViewModelBase {
        public static readonly GpusOverClockViewModel Current = new GpusOverClockViewModel();
        private GpuOverClockDataViewModel _currentGpuOverClockDataVm;
        private GpuOverClockDataViewModel _gpuAllOverClockDataVm;

        public ICommand Apply { get; private set; }

        private GpusOverClockViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            this.Apply = new DelegateCommand(() => {

            });
            _gpuAllOverClockDataVm = GpuOverClockVms.GpuAllVm;
            _currentGpuOverClockDataVm = GpuOverClockVms.List.FirstOrDefault();
        }

        public CoinViewModels CoinVms {
            get {
                return CoinViewModels.Current;
            }
        }

        public GpuOverClockDataViewModel CurrentGpuOverClockDataVm {
            get => _currentGpuOverClockDataVm;
            set {
                _currentGpuOverClockDataVm = value;
                OnPropertyChanged(nameof(CurrentGpuOverClockDataVm));
            }
        }

        public GpuOverClockDataViewModel GpuAllOverClockDataVm {
            get => _gpuAllOverClockDataVm;
            set {
                _gpuAllOverClockDataVm = value;
                OnPropertyChanged(nameof(GpuAllOverClockDataVm));
            }
        }

        public GpuOverClockDataViewModels GpuOverClockVms {
            get {
                return GpuOverClockDataViewModels.Current;
            }
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return MinerProfileViewModel.Current;
            }
        }
    }
}
