namespace NTMiner.Vms {
    public class DrivesViewModel : ViewModelBase {
        public static readonly DrivesViewModel Current = new DrivesViewModel();
        private bool _isNeedRestartWindows;

        private DrivesViewModel() {
        }

        public DriveSet DriveSet {
            get {
                return DriveSet.Current;
            }
        }

        public bool IsNeedRestartWindows {
            get => _isNeedRestartWindows;
            set {
                _isNeedRestartWindows = value;
                OnPropertyChanged(nameof(IsNeedRestartWindows));
            }
        }
    }
}
