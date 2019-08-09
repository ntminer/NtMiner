using System;

namespace NTMiner.Vms {
    public class MinerStateViewModel : ViewModelBase {
        public const double DefaultSideWidth = 220;
        private double _sideWidth = DefaultSideWidth;

        public MinerStateViewModel() {
            if (!Design.IsInDesignMode) {
                throw new InvalidProgramException();
            }
            this.StateBarVm = new StateBarViewModel();
        }

        public MinerStateViewModel(StateBarViewModel stateBarVm) {
            this.StateBarVm = stateBarVm;
        }

        public StateBarViewModel StateBarVm {
            get; private set;
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return AppContext.Instance.MinerProfileVm;
            }
        }

        public double SideWidth {
            get => _sideWidth;
            set {
                _sideWidth = value;
                OnPropertyChanged(nameof(SideWidth));
            }
        }
    }
}
