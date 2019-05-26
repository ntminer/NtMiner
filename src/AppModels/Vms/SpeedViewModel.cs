using NTMiner.Core;
using System;
using System.Windows.Media;

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
                    OnPropertyChanged(nameof(LastSpeedOnText));
                    OnPropertyChanged(nameof(LastSpeedOnForeground));
                }
            }
        }

        public string LastSpeedOnText {
            get {
                if (!NTMinerRoot.Instance.IsMining || SpeedOn <= Timestamp.UnixBaseTime) {
                    return string.Empty;
                }
                TimeSpan timeSpan = DateTime.Now - SpeedOn;
                if (timeSpan.Days >= 1) {
                    return timeSpan.Days + "天前";
                }
                if (timeSpan.Hours > 0) {
                    return timeSpan.Hours + " 小时前";
                }
                if (timeSpan.Minutes > 2) {
                    return timeSpan.Minutes + "分前";
                }
                return (int)timeSpan.TotalSeconds + "秒前";
            }
        }

        private static readonly SolidColorBrush Black = new SolidColorBrush(Colors.Black);
        public static readonly SolidColorBrush Red = new SolidColorBrush(Colors.Red);
        public SolidColorBrush LastSpeedOnForeground {
            get {
                if (SpeedOn.AddSeconds(120) < DateTime.Now) {
                    return Red;
                }
                return Black;
            }
        }
    }
}
