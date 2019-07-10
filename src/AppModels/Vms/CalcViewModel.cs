using NTMiner.MinerServer;

namespace NTMiner.Vms {
    public class CalcViewModel : ViewModelBase {
        private CoinViewModel _selectedCoinVm;
        private double _speed = 1;
        private string _incomePerDayText;
        private string _incomeUsdPerDayText;
        private string _incomeCnyPerDayText;
        private string _coinPriceCnyText;
        private string _modifiedOnText;
        private SpeedUnitViewModel _speedUnitVm = SpeedUnitViewModel.HPerSecond;

        private void ReRender() {
            if (SelectedCoinVm == null) {
                return;
            }
            ICalcConfig calcConfig;
            if (NTMinerRoot.Instance.CalcConfigSet.TryGetCalcConfig(SelectedCoinVm, out calcConfig)) {
                var incomePerDay = NTMinerRoot.Instance.CalcConfigSet.GetIncomePerHashPerDay(SelectedCoinVm.Code);
                IncomePerDayText = (this.Speed.FromUnitSpeed(this.SpeedUnitVm.Unit) * incomePerDay.IncomeCoin).ToString("f7");
                IncomeUsdPerDayText = (this.Speed.FromUnitSpeed(this.SpeedUnitVm.Unit) * incomePerDay.IncomeUsd).ToString("f7");
                IncomeCnyPerDayText = (this.Speed.FromUnitSpeed(this.SpeedUnitVm.Unit) * incomePerDay.IncomeCny).ToString("f7");
                CoinPriceCnyText = (incomePerDay.IncomeCny / incomePerDay.IncomeCoin).ToString("f2");
                ModifiedOnText = incomePerDay.ModifiedOn.ToString("yyyy-MM-dd HH:mm");
            }
            else {
                IncomePerDayText = "0";
                IncomeUsdPerDayText = "0";
                IncomeCnyPerDayText = "0";
                CoinPriceCnyText = "0";
                ModifiedOnText = string.Empty;
            }
        }

        public CoinViewModel SelectedCoinVm {
            get => _selectedCoinVm;
            set {
                _selectedCoinVm = value;
                if (_selectedCoinVm != value) {
                    this._speed = 1;
                    OnPropertyChanged(nameof(Speed));
                }
                ICalcConfig calcConfig;
                if (NTMinerRoot.Instance.CalcConfigSet.TryGetCalcConfig(SelectedCoinVm, out calcConfig)) {
                    this._speedUnitVm = SpeedUnitViewModel.GetSpeedUnitVm(calcConfig.SpeedUnit);
                    OnPropertyChanged(nameof(SpeedUnitVm));
                }
                ReRender();
                OnPropertyChanged(nameof(SelectedCoinVm));
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

        public string IncomePerDayText {
            get {
                return _incomePerDayText;
            }
            set {
                _incomePerDayText = value;
                OnPropertyChanged(nameof(IncomePerDayText));
            }
        }

        public string IncomeUsdPerDayText {
            get {
                return _incomeUsdPerDayText;
            }
            set {
                _incomeUsdPerDayText = value;
                OnPropertyChanged(nameof(IncomeUsdPerDayText));
            }
        }

        public string IncomeCnyPerDayText {
            get {
                return _incomeCnyPerDayText;
            }
            set {
                _incomeCnyPerDayText = value;
                OnPropertyChanged(nameof(IncomeCnyPerDayText));
            }
        }

        public string CoinPriceCnyText {
            get => _coinPriceCnyText;
            set {
                _coinPriceCnyText = value;
                OnPropertyChanged(nameof(CoinPriceCnyText));
            }
        }

        public string ModifiedOnText {
            get => _modifiedOnText;
            set {
                _modifiedOnText = value;
                OnPropertyChanged(nameof(ModifiedOnText));
            }
        }

        public AppContext.CoinViewModels CoinVms {
            get {
                return AppContext.Instance.CoinVms;
            }
        }
    }
}
