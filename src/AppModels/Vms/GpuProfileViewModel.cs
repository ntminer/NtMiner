using NTMiner.Core;
using NTMiner.Core.Profile;
using System;

namespace NTMiner.Vms {
    public class GpuProfileViewModel : ViewModelBase, IGpuProfile {
        private Guid _coinId;
        private int _index;
        private int _coreClockDelta;
        private int _memoryClockDelta;
        private int _powerCapacity;
        private int _cool;
        private GpuViewModel _gpuVm;
        private int _tempLimit;
        private bool _isAutoFanSpeed;
        private int _coreVoltage;
        private int _memoryVoltage;
        private int _currentMemoryTimingLevel;

        public GpuProfileViewModel() {
        }

        public GpuProfileViewModel(IGpuProfile data, GpuViewModel gpuVm) : this() {
            _coinId = data.CoinId;
            _index = data.Index;
            _coreClockDelta = data.CoreClockDelta;
            _memoryClockDelta = data.MemoryClockDelta;
            _powerCapacity = data.PowerCapacity;
            _tempLimit = data.TempLimit;
            _isAutoFanSpeed = data.IsAutoFanSpeed;
            _cool = data.Cool;
            _gpuVm = gpuVm;
            _coreVoltage = data.CoreVoltage;
            _memoryVoltage = data.MemoryVoltage;
            _currentMemoryTimingLevel = data.CurrentMemoryTimingLevel;
        }

        public void Update(IGpuProfile data) {
            this._coinId = data.CoinId;
            this._index = data.Index;
            this._isAutoFanSpeed = data.IsAutoFanSpeed;

            OnPropertyChanged(nameof(CoinId));
            OnPropertyChanged(nameof(Index));
            OnPropertyChanged(nameof(IsAutoFanSpeed));
            this.Update((IOverClockInput)data);
        }

        public void Update(IOverClockInput data) {
            this._coreClockDelta = data.CoreClockDelta;
            this._memoryClockDelta = data.MemoryClockDelta;
            this._powerCapacity = data.PowerCapacity;
            this._tempLimit = data.TempLimit;
            this._cool = data.Cool;
            this._coreVoltage = data.CoreVoltage;
            this._memoryVoltage = data.MemoryVoltage;
            this._currentMemoryTimingLevel = data.CurrentMemoryTimingLevel;

            OnPropertyChanged(nameof(CoreClockDelta));
            OnPropertyChanged(nameof(MemoryClockDelta));
            OnPropertyChanged(nameof(PowerCapacity));
            OnPropertyChanged(nameof(TempLimit));
            OnPropertyChanged(nameof(Cool));
            OnPropertyChanged(nameof(CoreVoltage));
            OnPropertyChanged(nameof(MemoryVoltage));
            OnPropertyChanged(nameof(CurrentMemoryTimingLevel));
        }

        public string GetId() {
            return $"{CoinId.ToString()}_{Index.ToString()}";
        }

        public Guid CoinId {
            get => _coinId;
            set {
                _coinId = value;
                OnPropertyChanged(nameof(CoinId));
            }
        }

        public int Index {
            get => _index;
            set {
                _index = value;
                OnPropertyChanged(nameof(Index));
                OnPropertyChanged(nameof(IndexText));
            }
        }

        public bool IsGpuAllVm {
            get {
                if (this.CoinId == Guid.Empty) {
                    return false;
                }
                return this.Index == NTMinerContext.GpuAllId;
            }
        }

        public string IndexText {
            get {
                if (Index == NTMinerContext.GpuAllId || GpuVm == null) {
                    return "all#统一超频";
                }
                return $"{Index.ToString()}#{GpuVm.Name}";
            }
        }

        public int CoreClockDelta {
            get => _coreClockDelta;
            set {
                if (_coreClockDelta != value) {
                    _coreClockDelta = value;
                    OnPropertyChanged(nameof(CoreClockDelta));
                    Save();
                }
            }
        }

        public int MemoryClockDelta {
            get => _memoryClockDelta;
            set {
                if (_memoryClockDelta != value) {
                    _memoryClockDelta = value;
                    OnPropertyChanged(nameof(MemoryClockDelta));
                    Save();
                }
            }
        }

        public int CoreVoltage {
            get => _coreVoltage;
            set {
                if (_coreVoltage != value) {
                    _coreVoltage = value;
                    OnPropertyChanged(nameof(CoreVoltage));
                    Save();
                }
            }
        }

        public int MemoryVoltage {
            get => _memoryVoltage;
            set {
                if (_memoryVoltage != value) {
                    _memoryVoltage = value;
                    OnPropertyChanged(nameof(MemoryVoltage));
                    Save();
                }
            }
        }

        public int PowerCapacity {
            get => _powerCapacity;
            set {
                if (_powerCapacity != value) {
                    _powerCapacity = value;
                    OnPropertyChanged(nameof(PowerCapacity));
                    Save();
                }
            }
        }

        public int TempLimit {
            get => _tempLimit;
            set {
                if (_tempLimit != value) {
                    _tempLimit = value;
                    OnPropertyChanged(nameof(TempLimit));
                    Save();
                }
            }
        }

        public bool IsAutoFanSpeed {
            get => _isAutoFanSpeed;
            set {
                if (_isAutoFanSpeed != value) {
                    _isAutoFanSpeed = value;
                    OnPropertyChanged(nameof(IsAutoFanSpeed));
                    Save();
                }
            }
        }

        public int Cool {
            get => _cool;
            set {
                if (_cool != value) {
                    _cool = value;
                    OnPropertyChanged(nameof(Cool));
                    Save();
                }
            }
        }

        public int CurrentMemoryTimingLevel {
            get => _currentMemoryTimingLevel;
            set {
                if (_currentMemoryTimingLevel != value) {
                    _currentMemoryTimingLevel = value;
                    OnPropertyChanged(nameof(CurrentMemoryTimingLevel));
                    Save();
                }
            }
        }

        public GpuViewModel GpuVm {
            get {
                if (_gpuVm == null) {
                    AppRoot.GpuVms.TryGetGpuVm(Index, out _gpuVm);
                }
                return _gpuVm;
            }
        }

        private void Save() {
            VirtualRoot.Execute(new AddOrUpdateGpuProfileCommand(this));
        }
    }
}
