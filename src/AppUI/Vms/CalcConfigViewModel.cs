using NTMiner.ServiceContracts.DataObjects;
using System;

namespace NTMiner.Vms {
    public class CalcConfigViewModel : ViewModelBase, ICalcConfig {
        private double _incomePerDay;
        private string _speedUnit;
        private double _speed;
        private string _coinCode;
        private DateTime _createdOn;
        private DateTime _modifiedOn;

        public CalcConfigViewModel(ICalcConfig data) {
            _incomePerDay = data.IncomePerDay;
            _speed = data.Speed;
            _speedUnit = data.SpeedUnit;
            _coinCode = data.CoinCode;
            _createdOn = data.CreatedOn;
            _modifiedOn = data.ModifiedOn;
        }

        public string CoinCode {
            get => _coinCode;
            set {
                _coinCode = value;
                OnPropertyChanged(nameof(CoinCode));
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
                _speedUnit = value;
                OnPropertyChanged(nameof(SpeedUnit));
                OnPropertyChanged(nameof(SpeedUnitVm));
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
                _incomePerDay = value;
                OnPropertyChanged(nameof(IncomePerDay));
            }
        }

        public DateTime CreatedOn {
            get => _createdOn;
            set {
                _createdOn = value;
                OnPropertyChanged(nameof(CreatedOn));
            }
        }

        public DateTime ModifiedOn {
            get => _modifiedOn;
            set {
                _modifiedOn = value;
                OnPropertyChanged(nameof(ModifiedOn));
            }
        }
    }
}
