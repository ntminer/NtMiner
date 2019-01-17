using NTMiner.ServiceContracts.DataObjects;

namespace NTMiner.Vms {
    public class CalcViewModel : ViewModelBase {
        private CoinViewModel _selectedCoinVm;
        private double _speed;
        private SpeedUnitViewModel _speedUnitVm;

        public CoinViewModel SelectedCoinVm {
            get => _selectedCoinVm;
            set {
                _selectedCoinVm = value;
                OnPropertyChanged(nameof(SelectedCoinVm));
                ICalcConfig calcConfig;
                if (NTMinerRoot.Current.CalcConfigSet.TryGetCalcConfig(value, out calcConfig)) {
                    this.SpeedUnitVm = SpeedUnitViewModel.GetSpeedUnitVm(calcConfig.SpeedUnit);
                    this.Speed = 1;
                }
            }
        }

        public double Speed {
            get => _speed;
            set {
                _speed = value;
                OnPropertyChanged(nameof(Speed));
                OnPropertyChanged(nameof(IncomePerDayText));
            }
        }


        public SpeedUnitViewModel SpeedUnitVm {
            get => _speedUnitVm;
            set {
                _speedUnitVm = value;
                OnPropertyChanged(nameof(SpeedUnitVm));
                OnPropertyChanged(nameof(IncomePerDayText));
            }
        }

        public string IncomePerDayText {
            get {
                ICalcConfig calcConfig;
                if (SelectedCoinVm != null && NTMinerRoot.Current.CalcConfigSet.TryGetCalcConfig(SelectedCoinVm, out calcConfig)) {
                    return (this.Speed.FromUnitSpeed(this.SpeedUnitVm.Unit) * NTMinerRoot.Current.CalcConfigSet.GetIncomePerHashPerDay(SelectedCoinVm)).ToString("f7");
                }
                return "0";
            }
        }


        public CoinViewModels CoinVms {
            get {
                return CoinViewModels.Current;
            }
        }
    }
}
