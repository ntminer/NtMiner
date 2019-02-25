using System.Linq;

namespace NTMiner.Vms {
    public class GpusOverClockViewModel : ViewModelBase {
        public static readonly GpusOverClockViewModel Current = new GpusOverClockViewModel();
        private GpuOverClockDataViewModel _currentGpuOverClockDataVm;

        private GpusOverClockViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            _currentGpuOverClockDataVm = GpuOverClockVms.List.FirstOrDefault();
        }

        public GpuOverClockDataViewModel CurrentGpuOverClockDataVm {
            get => _currentGpuOverClockDataVm;
            set {
                _currentGpuOverClockDataVm = value;
                OnPropertyChanged(nameof(CurrentGpuOverClockDataVm));
            }
        }

        public GpuOverClockDataViewModels GpuOverClockVms {
            get {
                return GpuOverClockDataViewModels.Current;
            }
        }
    }
}
