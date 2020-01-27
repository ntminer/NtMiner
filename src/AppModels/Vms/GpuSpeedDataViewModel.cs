using NTMiner.Core;
using NTMiner.Core.MinerClient;
using System;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class GpuSpeedDataViewModel : ViewModelBase, IGpuSpeedData {
        private readonly GpuSpeedData _data;
        private SolidColorBrush _temperatureForeground = MinerClientViewModel.DefaultForeground;

        public GpuSpeedDataViewModel(GpuSpeedData data) {
            _data = data;
        }
        public int Index {
            get { return _data.Index; }
            set {
                if (_data.Index != value) {
                    _data.Index = value;
                    OnPropertyChanged(nameof(Index));
                }
            }
        }

        public string Name {
            get { return _data.Name; }
            set {
                if (_data.Name != value) {
                    _data.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public ulong TotalMemory {
            get { return _data.TotalMemory; }
            set {
                if (_data.TotalMemory != value) {
                    _data.TotalMemory = value;
                    OnPropertyChanged(nameof(TotalMemory));
                    OnPropertyChanged(nameof(TotalMemoryGbText));
                }
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
                if (_data.MainCoinSpeed != value) {
                    _data.MainCoinSpeed = value;
                    OnPropertyChanged(nameof(MainCoinSpeed));
                    OnPropertyChanged(nameof(MainCoinSpeedText));
                }
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
                if (_data.DualCoinSpeed != value) {
                    _data.DualCoinSpeed = value;
                    OnPropertyChanged(nameof(DualCoinSpeed));
                    OnPropertyChanged(nameof(DualCoinSpeedText));
                }
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
                if (_data.Temperature != value) {
                    _data.Temperature = value;
                    OnPropertyChanged(nameof(Temperature));
                    OnPropertyChanged(nameof(TemperatureText));
                }
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
                if (_data.FanSpeed != value) {
                    _data.FanSpeed = value;
                    OnPropertyChanged(nameof(FanSpeed));
                    OnPropertyChanged(nameof(FanSpeedText));
                }
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
                if (_data.PowerUsage != value) {
                    _data.PowerUsage = value;
                    OnPropertyChanged(nameof(PowerUsage));
                    OnPropertyChanged(nameof(PowerUsageW));
                    OnPropertyChanged(nameof(PowerUsageWText));
                }
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
                if (_data.CoreClockDelta != value) {
                    _data.CoreClockDelta = value;
                    OnPropertyChanged(nameof(CoreClockDelta));
                    OnPropertyChanged(nameof(CoreClockDeltaMText));
                }
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
                if (_data.MemoryClockDelta != value) {
                    _data.MemoryClockDelta = value;
                    OnPropertyChanged(nameof(MemoryClockDelta));
                    OnPropertyChanged(nameof(MemoryClockDeltaMText));
                }
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
                if (_data.Cool != value) {
                    _data.Cool = value;
                    OnPropertyChanged(nameof(Cool));
                    OnPropertyChanged(nameof(CoolText));
                }
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
                if (_data.PowerCapacity != value) {
                    _data.PowerCapacity = value;
                    OnPropertyChanged(nameof(PowerCapacity));
                    OnPropertyChanged(nameof(PowerCapacityText));
                }
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
                if (_data.TempLimit != value) {
                    _data.TempLimit = value;
                    OnPropertyChanged(nameof(TempLimit));
                    OnPropertyChanged(nameof(TempLimitText));
                }
            }
        }

        public string TempLimitText {
            get {
                return this.TempLimit + "℃";
            }
        }

        public int CoreVoltage {
            get { return _data.CoreVoltage; }
            set {
                if (_data.CoreVoltage != value) {
                    _data.CoreVoltage = value;
                    OnPropertyChanged(nameof(CoreVoltage));
                }
            }
        }

        public int MemoryVoltage {
            get { return _data.MemoryVoltage; }
            set {
                if (_data.MemoryVoltage != value) {
                    _data.MemoryVoltage = value;
                    OnPropertyChanged(nameof(MemoryVoltage));
                }
            }
        }

        public int FoundShare {
            get { return _data.FoundShare; }
            set {
                if (_data.FoundShare != value) {
                    _data.FoundShare = value;
                    OnPropertyChanged(nameof(FoundShare));
                }
            }
        }

        public int AcceptShare {
            get { return _data.AcceptShare; }
            set {
                if (_data.AcceptShare != value) {
                    _data.AcceptShare = value;
                    OnPropertyChanged(nameof(AcceptShare));
                }
            }
        }

        public int RejectShare {
            get { return _data.RejectShare; }
            set {
                if (_data.RejectShare != value) {
                    _data.RejectShare = value;
                    OnPropertyChanged(nameof(RejectShare));
                }
            }
        }

        public int IncorrectShare {
            get { return _data.IncorrectShare; }
            set {
                if (_data.IncorrectShare != value) {
                    _data.IncorrectShare = value;
                    OnPropertyChanged(nameof(IncorrectShare));
                }
            }
        }
    }
}
