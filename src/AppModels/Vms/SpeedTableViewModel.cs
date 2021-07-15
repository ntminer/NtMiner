using System.Windows;

namespace NTMiner.Vms {
    public class SpeedTableViewModel : ViewModelBase {
        private Visibility _isPCIEVisible = Visibility.Collapsed;

        public SpeedTableViewModel() {
        }

        public Visibility IsPCIEVisible {
            get => _isPCIEVisible;
            set {
                if (_isPCIEVisible != value) {
                    _isPCIEVisible = value;
                    OnPropertyChanged(nameof(IsPCIEVisible));
                }
            }
        }

        public AppRoot.GpuSpeedViewModels GpuSpeedVms {
            get {
                return AppRoot.GpuSpeedViewModels.Instance;
            }
        }
    }
}
