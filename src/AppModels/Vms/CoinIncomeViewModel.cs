using NTMiner.MinerServer;
using System;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class CoinIncomeViewModel : ViewModelBase {
        private double _speed = 1;
        private string _incomePerDayText;
        private string _incomeCnyPerDayText;
        private string _coinPriceCnyText;
        private readonly CoinViewModel _coinVm;
        private SpeedUnitViewModel _speedUnitVm = null;
        private SolidColorBrush _backgroundBrush;
        private DateTime _modifiedOn;

        public CoinIncomeViewModel(CoinViewModel coinVm) {
            _coinVm = coinVm;
        }

        private static readonly SolidColorBrush White = new SolidColorBrush(Colors.White);
        private static readonly SolidColorBrush Red = new SolidColorBrush(Color.FromRgb(0xFF, 0xCC, 0x00));
        public void Refresh() {
            if (NTMinerRoot.Instance.CalcConfigSet.TryGetCalcConfig(_coinVm, out ICalcConfig calcConfig)) {
                var incomePerDay = NTMinerRoot.Instance.CalcConfigSet.GetIncomePerHashPerDay(_coinVm.Code);
                var v = this.Speed.FromUnitSpeed(this.SpeedUnitVm.Unit) * incomePerDay.IncomeCoin;
                if (v >= 100) {
                    IncomePerDayText = v.ToString("f2");
                }
                else {
                    IncomePerDayText = v.ToString("f7");
                }
                IncomeCnyPerDayText = (this.Speed.FromUnitSpeed(this.SpeedUnitVm.Unit) * incomePerDay.IncomeCny).ToString("f2");
                CoinPriceCnyText = (incomePerDay.IncomeCny / incomePerDay.IncomeCoin).ToString("f2");
                ModifiedOn = incomePerDay.ModifiedOn;
                if (ModifiedOn.AddMinutes(15) < DateTime.Now) {
                    BackgroundBrush = Red;
                }
                else {
                    BackgroundBrush = White;
                }
            }
            else {
                IncomePerDayText = "0";
                IncomeCnyPerDayText = "0";
                CoinPriceCnyText = "0";
                ModifiedOn = DateTime.MinValue;
                BackgroundBrush = Red;
            }
        }

        public double Speed {
            get => _speed;
            set {
                if (_speed != value) {
                    _speed = value;
                    Refresh();
                    OnPropertyChanged(nameof(Speed));
                }
            }
        }


        public SpeedUnitViewModel SpeedUnitVm {
            get {
                if (_speedUnitVm == null && NTMinerRoot.Instance.CalcConfigSet.TryGetCalcConfig(_coinVm, out ICalcConfig calcConfig)) {
                    return SpeedUnitViewModel.GetSpeedUnitVm(calcConfig.SpeedUnit);
                }
                return _speedUnitVm;
            }
            set {
                if (_speedUnitVm != value) {
                    _speedUnitVm = value;
                    Refresh();
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

        public DateTime ModifiedOn {
            get => _modifiedOn;
            set {
                _modifiedOn = value;
                OnPropertyChanged(nameof(ModifiedOn));
                OnPropertyChanged(nameof(ModifiedOnText));
            }
        }

        public string ModifiedOnText {
            get {
                return ModifiedOn.ToString("yyyy-MM-dd HH:mm");
            }
        }

        public SolidColorBrush BackgroundBrush {
            get => _backgroundBrush;
            set {
                _backgroundBrush = value;
                OnPropertyChanged(nameof(BackgroundBrush));
            }
        }
    }
}
