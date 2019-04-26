using NTMiner.MinerServer;

namespace NTMiner.Vms {
    public class CalcViewModel : ViewModelBase {
        private CoinViewModel _selectedCoinVm;
        private double _speed;
        private SpeedUnitViewModel _speedUnitVm = SpeedUnitViewModel.HPerSecond;

        public void ReRender() {
            if (SelectedCoinVm == null) {
                return;
            }
            ICalcConfig calcConfig;
            if (NTMinerRoot.Current.CalcConfigSet.TryGetCalcConfig(SelectedCoinVm, out calcConfig)) {
                var incomePerDay = NTMinerRoot.Current.CalcConfigSet.GetIncomePerHashPerDay(SelectedCoinVm.Code);
                IncomePerDayText = (this.Speed.FromUnitSpeed(this.SpeedUnitVm.Unit) * incomePerDay.IncomeCoin).ToString("f7");
                IncomeUsdPerDayText = (this.Speed.FromUnitSpeed(this.SpeedUnitVm.Unit) * incomePerDay.IncomeUsd).ToString("f7");
                IncomeCnyPerDayText = (this.Speed.FromUnitSpeed(this.SpeedUnitVm.Unit) * incomePerDay.IncomeCny).ToString("f7");
            }
            else {
                IncomePerDayText = "0";
                IncomeUsdPerDayText = "0";
                IncomeCnyPerDayText = "0";
            }
        }

        public CoinViewModel SelectedCoinVm {
            get => _selectedCoinVm;
            set {
                if (_selectedCoinVm != value) {
                    _selectedCoinVm = value;
                    this.Speed = 1;
                    ICalcConfig calcConfig;
                    if (NTMinerRoot.Current.CalcConfigSet.TryGetCalcConfig(SelectedCoinVm, out calcConfig)) {
                        this.SpeedUnitVm = SpeedUnitViewModel.GetSpeedUnitVm(calcConfig.SpeedUnit);
                    }
                    OnPropertyChanged(nameof(SelectedCoinVm));
                }
            }
        }

        public double Speed {
            get => _speed;
            set {
                if (_speed != value) {
                    _speed = value;
                    ReRender();
                    OnPropertyChanged(nameof(Speed));
                }
            }
        }


        public SpeedUnitViewModel SpeedUnitVm {
            get => _speedUnitVm;
            set {
                if (_speedUnitVm != value) {
                    _speedUnitVm = value;
                    ReRender();
                    OnPropertyChanged(nameof(SpeedUnitVm));
                }
            }
        }

        private string _incomePerDayText;
        public string IncomePerDayText {
            get {
                return _incomePerDayText;
            }
            set {
                _incomePerDayText = value;
                OnPropertyChanged(nameof(IncomePerDayText));
            }
        }

        private string _incomeUsdPerDayText;
        public string IncomeUsdPerDayText {
            get {
                return _incomeUsdPerDayText;                
            }
            set {
                _incomeUsdPerDayText = value;
                OnPropertyChanged(nameof(IncomeUsdPerDayText));
            }
        }

        private string _incomeCnyPerDayText;
        public string IncomeCnyPerDayText {
            get {
                return _incomeCnyPerDayText;
            }
            set {
                _incomeCnyPerDayText = value;
                OnPropertyChanged(nameof(IncomeCnyPerDayText));
            }
        }

        public CoinViewModels CoinVms {
            get {
                return CoinViewModels.Current;
            }
        }
    }
}
