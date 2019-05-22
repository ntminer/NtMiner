namespace NTMiner.Vms {
    public class MinerStateViewModel : ViewModelBase {
        public const double DefaultSideWidth = 220;
        private double _sideWidth = DefaultSideWidth;

        public MinerStateViewModel(StateBarViewModel stateBarVm) {
            this.StateBarVm = stateBarVm;
        }

        public StateBarViewModel StateBarVm {
            get; private set;
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
