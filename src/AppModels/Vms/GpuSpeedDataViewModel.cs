using NTMiner.MinerClient;
using System;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class GpuSpeedDataViewModel : ViewModelBase {
        private readonly GpuSpeedData _data;
        private SolidColorBrush _temperatureForeground = MinerClientViewModel.DefaultForeground;

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

        public ulong TotalMemory {
            get { return _data.TotalMemory; }
            set {
                _data.TotalMemory = value;
                OnPropertyChanged(nameof(TotalMemory));
                OnPropertyChanged(nameof(TotalMemoryGbText));
            }
        }

        private const double g = 1024 * 1024 * 1024;
        public string TotalMemoryGbText {
            get {
                return Math.Round(this.TotalMemory / g, 1) + "Gb";
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

        public int Temperature {
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

        public int CoreClockDelta {
            get => _data.CoreClockDelta;
            set {
                _data.CoreClockDelta = value;
                OnPropertyChanged(nameof(CoreClockDelta));
                OnPropertyChanged(nameof(CoreClockDeltaMText));
            }
        }

        public string CoreClockDeltaMText {
            get {
                return (CoreClockDelta / 1000).ToString() + "M";
            }
        }

        public int MemoryClockDelta {
            get => _data.MemoryClockDelta;
            set {
                _data.MemoryClockDelta = value;
                OnPropertyChanged(nameof(MemoryClockDelta));
                OnPropertyChanged(nameof(MemoryClockDeltaMText));
            }
        }

        public string MemoryClockDeltaMText {
            get {
                return (MemoryClockDelta / 1000).ToString() + "M";
            }
        }

        public int Cool {
            get => _data.Cool;
            set {
                _data.Cool = value;
                OnPropertyChanged(nameof(Cool));
                OnPropertyChanged(nameof(CoolText));
            }
        }

        public string CoolText {
            get {
                return this.Cool.ToString() + "%";
            }
        }

        public double PowerCapacity {
            get => _data.PowerCapacity;
            set {
                _data.PowerCapacity = value;
                OnPropertyChanged(nameof(PowerCapacity));
                OnPropertyChanged(nameof(PowerCapacityText));
            }
        }

        public string PowerCapacityText {
            get {
                return this.PowerCapacity.ToString("f0") + "%";
            }
        }

        public int TempLimit {
            get { return _data.TempLimit; }
            set {
                _data.TempLimit = value;
                OnPropertyChanged(nameof(TempLimit));
                OnPropertyChanged(nameof(TempLimitText));
            }
        }

        public string TempLimitText {
            get {
                return this.TempLimit + "℃";
            }
        }
    }
}
