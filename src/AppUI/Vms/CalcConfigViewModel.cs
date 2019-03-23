using NTMiner.MinerServer;
using System;

namespace NTMiner.Vms {
    public class CalcConfigViewModel : ViewModelBase, ICalcConfig {
        private double _incomePerDay;
        private double _incomeUsdPerDay;
        private double _incomeCnyPerDay;
        private string _speedUnit;
        private double _speed;
        private string _coinCode;
        private DateTime _createdOn;
        private DateTime _modifiedOn;

        public CalcConfigViewModel(ICalcConfig data) {
            _incomePerDay = data.IncomePerDay;
            _incomeUsdPerDay = data.IncomeUsdPerDay;
            _incomeCnyPerDay = data.IncomeCnyPerDay;
            _speed = data.Speed;
            _speedUnit = data.SpeedUnit;
            _coinCode = data.CoinCode;
            _createdOn = data.CreatedOn;
            _modifiedOn = data.ModifiedOn;
        }

        public string CoinCode {
            get => _coinCode;
            set {
                if (_coinCode != value) {
                    _coinCode = value;
                    OnPropertyChanged(nameof(CoinCode));
                }
            }
        }

        public double Speed {
            get => _speed;
            set {
                if (_speed != value) {
                    _speed = value;
                    OnPropertyChanged(nameof(Speed));
                }
            }
        }

        public string SpeedUnit {
            get => _speedUnit;
            set {
                if (_speedUnit != value) {
                    _speedUnit = value;
                    OnPropertyChanged(nameof(SpeedUnit));
                    OnPropertyChanged(nameof(SpeedUnitVm));
                }
            }
        }

        public SpeedUnitViewModel SpeedUnitVm {
            get {
                return SpeedUnitViewModel.GetSpeedUnitVm(this.SpeedUnit);
            }
            set {
                SpeedUnit = value.Unit;
            }
        }

        public double IncomePerDay {
            get => _incomePerDay;
            set {
                if (_incomePerDay != value) {
                    _incomePerDay = value;
                    OnPropertyChanged(nameof(IncomePerDay));
                }
            }
        }

        public double IncomeUsdPerDay {
            get { return _incomeUsdPerDay; }
            set {
                if (_incomeUsdPerDay != value) {
                    _incomeUsdPerDay = value;
                    OnPropertyChanged(nameof(IncomeUsdPerDay));
                }
            }
        }

        public double IncomeCnyPerDay {
            get { return _incomeCnyPerDay; }
            set {
                if (_incomeCnyPerDay != value) {
                    _incomeCnyPerDay = value;
                    OnPropertyChanged(nameof(IncomeCnyPerDay));
                }
            }
        }

        public DateTime CreatedOn {
            get => _createdOn;
            set {
                if (_createdOn != value) {
                    _createdOn = value;
                    OnPropertyChanged(nameof(CreatedOn));
                }
            }
        }

        public DateTime ModifiedOn {
            get => _modifiedOn;
            set {
                if (_modifiedOn != value) {
                    _modifiedOn = value;
                    OnPropertyChanged(nameof(ModifiedOn));
                }
            }
        }
    }
}
