using NTMiner.Core;
using NTMiner.MinerServer;

namespace NTMiner.Vms {
    public class CalcViewModel : ViewModelBase {
        private CoinViewModel _selectedCoinVm;
        private double _speed;
        private SpeedUnitViewModel _speedUnitVm;

        public CoinViewModel SelectedCoinVm {
            get => _selectedCoinVm;
            set {
                if (_selectedCoinVm != value) {
                    _selectedCoinVm = value;
                    OnPropertyChanged(nameof(SelectedCoinVm));
                    ICalcConfig calcConfig;
                    if (NTMinerRoot.Current.CalcConfigSet.TryGetCalcConfig(value, out calcConfig)) {
                        this.SpeedUnitVm = SpeedUnitViewModel.GetSpeedUnitVm(calcConfig.SpeedUnit);
                        this.Speed = 1;
                    }
                }
            }
        }

        public double Speed {
            get => _speed;
            set {
                if (_speed != value) {
                    _speed = value;
                    OnPropertyChanged(nameof(Speed));
                    OnPropertyChanged(nameof(IncomePerDayText));
                    OnPropertyChanged(nameof(IncomeUsdPerDayText));
                    OnPropertyChanged(nameof(IncomeCnyPerDayText));
                }
            }
        }


        public SpeedUnitViewModel SpeedUnitVm {
            get => _speedUnitVm;
            set {
                if (_speedUnitVm != value) {
                    _speedUnitVm = value;
                    OnPropertyChanged(nameof(SpeedUnitVm));
                    OnPropertyChanged(nameof(IncomePerDayText));
                    OnPropertyChanged(nameof(IncomeUsdPerDayText));
                    OnPropertyChanged(nameof(IncomeCnyPerDayText));
                }
            }
        }

        private CoinViewModel _incomeCoinVm;
        private IncomePerDay _incomePerDay = IncomePerDay.Zero;
        public IncomePerDay IncomePerDay {
            get {
                if (_incomeCoinVm == null || this.SelectedCoinVm != _incomeCoinVm) {
                    _incomeCoinVm = SelectedCoinVm;
                    _incomePerDay = NTMinerRoot.Current.CalcConfigSet.GetIncomePerHashPerDay(_incomeCoinVm.Code);
                }
                return _incomePerDay;
            }
        }

        public string IncomePerDayText {
            get {
                ICalcConfig calcConfig;
                if (SelectedCoinVm != null && NTMinerRoot.Current.CalcConfigSet.TryGetCalcConfig(SelectedCoinVm, out calcConfig)) {
                    return (this.Speed.FromUnitSpeed(this.SpeedUnitVm.Unit) * IncomePerDay.IncomeCoin).ToString("f7");
                }
                return "0";
            }
        }

        public string IncomeUsdPerDayText {
            get {
                ICalcConfig calcConfig;
                if (SelectedCoinVm != null && NTMinerRoot.Current.CalcConfigSet.TryGetCalcConfig(SelectedCoinVm, out calcConfig)) {
                    return (this.Speed.FromUnitSpeed(this.SpeedUnitVm.Unit) * IncomePerDay.IncomeUsd).ToString("f2");
                }
                return "0";
            }
        }

        public string IncomeCnyPerDayText {
            get {
                ICalcConfig calcConfig;
                if (SelectedCoinVm != null && NTMinerRoot.Current.CalcConfigSet.TryGetCalcConfig(SelectedCoinVm, out calcConfig)) {
                    return (this.Speed.FromUnitSpeed(this.SpeedUnitVm.Unit) * IncomePerDay.IncomeCny).ToString("f2");
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
