using NTMiner.ServiceContracts.DataObjects;

namespace NTMiner.Vms {
    public class CalcViewModel : ViewModelBase {
        private CoinViewModel _selectedCoinVm;
        private double _speed;
        private SpeedUnitViewModel _speedUnitVm;
        private double _incomePerDay;

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
                ICalcConfig calcConfig;
                if (SelectedCoinVm != null && NTMinerRoot.Current.CalcConfigSet.TryGetCalcConfig(SelectedCoinVm, out calcConfig)) {
                    this.IncomePerDay = value.ToLong(this.SpeedUnitVm.Unit) * NTMinerRoot.Current.CalcConfigSet.GetIncomePerHashPerDay(SelectedCoinVm);
                }
            }
        }


        public SpeedUnitViewModel SpeedUnitVm {
            get => _speedUnitVm;
            set {
                _speedUnitVm = value;
                OnPropertyChanged(nameof(SpeedUnitVm));
            }
        }

        public double IncomePerDay {
            get => _incomePerDay;
            set {
                _incomePerDay = value;
                OnPropertyChanged(nameof(IncomePerDay));
                OnPropertyChanged(nameof(IncomePerDayText));
            }
        }

        public string IncomePerDayText {
            get {
                return this.IncomePerDay.ToString("f7");
            }
        }


        public CoinViewModels CoinVms {
            get {
                return CoinViewModels.Current;
            }
        }
    }
}
