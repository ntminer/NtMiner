using System;

namespace NTMiner.Vms {
    public class RestartWindowsViewModel : ViewModelBase {
        private int _seconds = 4;
        public readonly Guid Id = Guid.NewGuid();

        public RestartWindowsViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public RestartWindowsViewModel(int seconds) {
            _seconds = seconds;
        }

        public int Seconds {
            get => _seconds;
            set {
                _seconds = value;
                OnPropertyChanged(nameof(Seconds));
            }
        }
    }
}
