using NTMiner.Core;
using System;

namespace NTMiner.Vms {
    public class SpeedViewModel : ViewModelBase, ISpeed {
        private double _speed;
        private DateTime _speedOn;

        public SpeedViewModel(ISpeed speed) {
            this.Value = speed.Value;
        }

        public void Update(ISpeed data) {
            this.Value = data.Value;
            this.SpeedOn = data.SpeedOn;
        }

        public double Value {
            get => _speed;
            set {
                if (_speed != value) {
                    _speed = value;
                    OnPropertyChanged(nameof(Value));
                    OnPropertyChanged(nameof(SpeedText));
                }
            }
        }

        public string SpeedText {
            get {
                return Value.ToUnitSpeedText();
            }
        }

        public DateTime SpeedOn {
            get {
                return _speedOn;
            }
            set {
                if (_speedOn != value) {
                    _speedOn = value;
                    OnPropertyChanged(nameof(SpeedOn));
                }
            }
        }
    }
}
