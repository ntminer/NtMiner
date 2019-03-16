using NTMiner.MinerClient;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class GpuSpeedDataViewModel : ViewModelBase {
        private readonly GpuSpeedData _data;
        public GpuSpeedDataViewModel(GpuSpeedData data) {
            _data = data;
        }
        public int Index {
            get { return _data.Index; }
            set {
                _data.Index = value;
                OnPropertyChanged(nameof(Index));
            }
        }

        public string Name {
            get { return _data.Name; }
            set {
                _data.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public double MainCoinSpeed {
            get { return _data.MainCoinSpeed; }
            set {
                _data.MainCoinSpeed = value;
                OnPropertyChanged(nameof(MainCoinSpeed));
                OnPropertyChanged(nameof(MainCoinSpeedText));
            }
        }

        public string MainCoinSpeedText {
            get {
                return MainCoinSpeed.ToUnitSpeedText();
            }
        }

        public double DualCoinSpeed {
            get { return _data.DualCoinSpeed; }
            set {
                _data.DualCoinSpeed = value;
                OnPropertyChanged(nameof(DualCoinSpeed));
                OnPropertyChanged(nameof(DualCoinSpeedText));
            }
        }

        public string DualCoinSpeedText {
            get {
                return DualCoinSpeed.ToUnitSpeedText();
            }
        }

        public uint Temperature {
            get { return _data.Temperature; }
            set {
                _data.Temperature = value;
                OnPropertyChanged(nameof(Temperature));
                OnPropertyChanged(nameof(TemperatureText));
            }
        }

        public string TemperatureText {
            get {
                return this.Temperature.ToString() + "℃";
            }
        }

        private SolidColorBrush _temperatureForeground = MinerClientViewModel.DefaultForeground;
        public SolidColorBrush TemperatureForeground {
            get {
                return _temperatureForeground;
            }
            set {
                _temperatureForeground = value;
                OnPropertyChanged(nameof(TemperatureForeground));
            }
        }

        public uint FanSpeed {
            get { return _data.FanSpeed; }
            set {
                _data.FanSpeed = value;
                OnPropertyChanged(nameof(FanSpeed));
                OnPropertyChanged(nameof(FanSpeedText));
            }
        }

        public string FanSpeedText {
            get {
                return this.FanSpeed.ToString() + "%";
            }
        }

        public uint PowerUsage {
            get { return _data.PowerUsage; }
            set {
                _data.PowerUsage = value;
                OnPropertyChanged(nameof(PowerUsage));
                OnPropertyChanged(nameof(PowerUsageW));
                OnPropertyChanged(nameof(PowerUsageWText));
            }
        }

        public double PowerUsageW {
            get {
                return this.PowerUsage;
            }
        }

        public string PowerUsageWText {
            get {
                return PowerUsageW.ToString("f0") + "W";
            }
        }
    }
}
