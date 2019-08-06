using NTMiner.MinerServer;
using System;

namespace NTMiner.Vms {
    public class CalcConfigViewModel : ViewModelBase, ICalcConfig {
        private double _incomePerDay;
        private double _incomeUsdPerDay;
        private double _incomeCnyPerDay;
        private string _speedUnit;
        private double _speed;
        private string _netSpeedUnit;
        private double _netSpeed;
        private double _baseNetSpeed;
        private string _coinCode;
        private DateTime _createdOn;
        private DateTime _modifiedOn;
        private double _dayWave;

        public CalcConfigViewModel(ICalcConfig data) {
            _incomePerDay = data.IncomePerDay;
            _incomeUsdPerDay = data.IncomeUsdPerDay;
            _incomeCnyPerDay = data.IncomeCnyPerDay;
            _speed = data.Speed;
            _speedUnit = data.SpeedUnit;
            _netSpeed = data.NetSpeed;
            _baseNetSpeed = data.BaseNetSpeed;
            _dayWave = data.DayWave;
            _netSpeedUnit = data.NetSpeedUnit;
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
                if (Math.Abs(_speed - value) > 0.01) {
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

        public double NetSpeed {
            get => _netSpeed;
            set {
                if (Math.Abs(_netSpeed - value) > 0.01) {
                    _netSpeed = value;
                    OnPropertyChanged(nameof(NetSpeed));
                }
            }
        }

        public double BaseNetSpeed {
            get { return _baseNetSpeed; }
            set {
                if (Math.Abs(_baseNetSpeed - value) > 0.001) {
                    _baseNetSpeed = value;
                    OnPropertyChanged(nameof(BaseNetSpeed));
                }
            }
        }

        public double DayWave {
            get => _dayWave;
            set {
                if (Math.Abs(_dayWave - value) > 0.0001) {
                    _dayWave = value;
                    OnPropertyChanged(nameof(DayWave));
                }
            }
        }

        public string NetSpeedUnit {
            get => _netSpeedUnit;
            set {
                if (_netSpeedUnit != value) {
                    _netSpeedUnit = value;
                    OnPropertyChanged(nameof(NetSpeedUnit));
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
                if (Math.Abs(_incomePerDay - value) > 0.01) {
                    _incomePerDay = value;
                    OnPropertyChanged(nameof(IncomePerDay));
                }
            }
        }

        public double IncomeUsdPerDay {
            get { return _incomeUsdPerDay; }
            set {
                if (Math.Abs(_incomeUsdPerDay - value) > 0.01) {
                    _incomeUsdPerDay = value;
                    OnPropertyChanged(nameof(IncomeUsdPerDay));
                }
            }
        }

        public double IncomeCnyPerDay {
            get { return _incomeCnyPerDay; }
            set {
                if (Math.Abs(_incomeCnyPerDay - value) > 0.01) {
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
