using NTMiner.MinerClient;
using NTMiner.MinerServer;
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
        }

        public void Update(IGpuProfile data) {
            this._coinId = data.CoinId;
            this._index = data.Index;
            this._coreClockDelta = data.CoreClockDelta;
            this._memoryClockDelta = data.MemoryClockDelta;
            this._powerCapacity = data.PowerCapacity;
            this._tempLimit = data.TempLimit;
            this._isAutoFanSpeed = data.IsAutoFanSpeed;
            this._cool = data.Cool;

            OnPropertyChanged(nameof(CoinId));
            OnPropertyChanged(nameof(Index));
            OnPropertyChanged(nameof(CoreClockDelta));
            OnPropertyChanged(nameof(MemoryClockDelta));
            OnPropertyChanged(nameof(PowerCapacity));
            OnPropertyChanged(nameof(TempLimit));
            OnPropertyChanged(nameof(IsAutoFanSpeed));
            OnPropertyChanged(nameof(Cool));
        }

        public void Update(IOverClockInput data) {
            this._coreClockDelta = data.CoreClockDelta;
            this._memoryClockDelta = data.MemoryClockDelta;
            this._powerCapacity = data.PowerCapacity;
            this._tempLimit = data.TempLimit;
            this._cool = data.Cool;

            OnPropertyChanged(nameof(CoreClockDelta));
            OnPropertyChanged(nameof(MemoryClockDelta));
            OnPropertyChanged(nameof(PowerCapacity));
            OnPropertyChanged(nameof(TempLimit));
            OnPropertyChanged(nameof(Cool));
        }

        public string GetId() {
            return $"{CoinId}_{Index}";
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
                return this.Index == NTMinerRoot.GpuAllId;
            }
        }

        public string IndexText {
            get {
                if (Index == NTMinerRoot.GpuAllId || GpuVm == null) {
                    return "all#统一超频";
                }
                return $"{Index}#{GpuVm.Name}";
            }
        }

        public int CoreClockDelta {
            get => _coreClockDelta;
            set {
                if (_coreClockDelta != value) {
                    _coreClockDelta = value;
                    OnPropertyChanged(nameof(CoreClockDelta));
                }
            }
        }

        public int MemoryClockDelta {
            get => _memoryClockDelta;
            set {
                if (_memoryClockDelta != value) {
                    _memoryClockDelta = value;
                    OnPropertyChanged(nameof(MemoryClockDelta));
                }
            }
        }

        public int PowerCapacity {
            get => _powerCapacity;
            set {
                _powerCapacity = value;
                OnPropertyChanged(nameof(PowerCapacity));
            }
        }

        public int TempLimit {
            get => _tempLimit;
            set {
                _tempLimit = value;
                OnPropertyChanged(nameof(TempLimit));
            }
        }

        public bool IsAutoFanSpeed {
            get => _isAutoFanSpeed;
            set {
                _isAutoFanSpeed = value;
                OnPropertyChanged(nameof(IsAutoFanSpeed));
            }
        }

        public int Cool {
            get => _cool;
            set {
                _cool = value;
                OnPropertyChanged(nameof(Cool));
            }
        }

        public GpuViewModel GpuVm {
            get {
                if (_gpuVm == null) {
                    AppContext.Instance.GpuVms.TryGetGpuVm(Index, out _gpuVm);
                }
                return _gpuVm;
            }
        }
    }
}
