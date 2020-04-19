namespace NTMiner.Vms {
    public class CalcViewModel : ViewModelBase {
        private SpeedUnitViewModel speedUnitVm;

        public AppRoot.CoinViewModels CoinVms {
            get {
                return AppRoot.CoinVms;
            }
        }

        public SpeedUnitViewModel SpeedUnitVm {
            get => speedUnitVm;
            set {
                speedUnitVm = value;
                OnPropertyChanged(nameof(SpeedUnitVm));
            }
        }
    }
}
