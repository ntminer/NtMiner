using System.Windows;

namespace NTMiner.Vms {
    public class SpeedTableViewModel : ViewModelBase {
        private Visibility _isOverClockVisible = Visibility.Collapsed;

        public SpeedTableViewModel() {
        }

        public Visibility IsACardVisible {
            get {
                if (NTMinerRoot.Instance.GpuSet.GpuType == GpuType.AMD) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public Visibility IsOverClockVisible {
            get { return _isOverClockVisible; }
            set {
                _isOverClockVisible = value;
                OnPropertyChanged(nameof(IsOverClockVisible));
            }
        }

        public AppContext.GpuSpeedViewModels GpuSpeedVms {
            get {
                return AppContext.GpuSpeedViewModels.Instance;
            }
        }
    }
}
