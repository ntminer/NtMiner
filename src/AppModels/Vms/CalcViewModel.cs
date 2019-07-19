namespace NTMiner.Vms {
    public class CalcViewModel : ViewModelBase {
        private SpeedUnitViewModel speedUnitVm;

        public AppContext.CoinViewModels CoinVms {
            get {
                return AppContext.Instance.CoinVms;
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
