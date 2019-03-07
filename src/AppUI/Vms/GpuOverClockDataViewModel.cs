using NTMiner.Core;
using NTMiner.Core.Gpus;
using NTMiner.OverClock;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class GpuOverClockDataViewModel : ViewModelBase, IGpuOverClockData {
        private Guid _id;
        private Guid _coinId;
        private int _index;
        private string _name;
        private int _coreClockDelta;
        private int _memoryClockDelta;
        private int _powerCapacity;
        private int _cool;
        private bool _isEnabled = true;

        public ICommand Apply { get; private set; }

        public GpuOverClockDataViewModel(Guid id) {
            _id = id;
            this.Apply = new DelegateCommand(() => {
                VirtualRoot.Execute(new OverClockCommand(this));
            });
        }

        public GpuOverClockDataViewModel(IGpuOverClockData data) : this(data.GetId()) {
            _coinId = data.CoinId;
            _index = data.Index;
            _name = data.Name;
            _coreClockDelta = data.CoreClockDelta;
            _memoryClockDelta = data.MemoryClockDelta;
            _powerCapacity = data.PowerCapacity;
            _cool = data.Cool;
            _isEnabled = data.IsEnabled;
            GpuViewModels.Current.TryGetGpuVm(Index, out _gpuVm);
        }

        public void Update(IGpuOverClockData data) {
            this._coinId = data.CoinId;
            this._index = data.Index;
            this._name = data.Name;
            this._coreClockDelta = data.CoreClockDelta;
            this._memoryClockDelta = data.MemoryClockDelta;
            this._powerCapacity = data.PowerCapacity;
            this._cool = data.Cool;
            this._isEnabled = data.IsEnabled;

            OnPropertyChanged(nameof(CoinId));
            OnPropertyChanged(nameof(Index));
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(CoreClockDelta));
            OnPropertyChanged(nameof(MemoryClockDelta));
            OnPropertyChanged(nameof(PowerCapacity));
            OnPropertyChanged(nameof(Cool));
            OnPropertyChanged(nameof(IsEnabled));
        }

        public void Update(IOverClockData data) {
            this._coreClockDelta = data.CoreClockDelta;
            this._memoryClockDelta = data.MemoryClockDelta;
            this._powerCapacity = data.PowerCapacity;
            this._cool = data.Cool;

            OnPropertyChanged(nameof(CoreClockDelta));
            OnPropertyChanged(nameof(MemoryClockDelta));
            OnPropertyChanged(nameof(PowerCapacity));
            OnPropertyChanged(nameof(Cool));
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _id;
            set {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
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

        public bool IsEnabled {
            get => _isEnabled;
            set {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
                VirtualRoot.Execute(new AddOrUpdateGpuOverClockDataCommand(this));
            }
        }

        public bool IsGpuAllVm {
            get {
                if (this.CoinId == Guid.Empty) {
                    return false;
                }
                return this == GpuOverClockDataViewModels.Current.GpuAllVm(CoinId);
            }
        }

        public string IndexText {
            get {
                if (Index == NTMinerRoot.GpuAllId) {
                    return "all#统一超频";
                }
                return $"{Index}#{Name}";
            }
        }

        public string Name {
            get => _name;
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public int CoreClockDelta {
            get => _coreClockDelta;
            set {
                if (_coreClockDelta != value) {
                    _coreClockDelta = value;
                    OnPropertyChanged(nameof(CoreClockDelta));
                    VirtualRoot.Execute(new AddOrUpdateGpuOverClockDataCommand(this));
                }
            }
        }

        public int MemoryClockDelta {
            get => _memoryClockDelta;
            set {
                if (_memoryClockDelta != value) {
                    _memoryClockDelta = value;
                    OnPropertyChanged(nameof(MemoryClockDelta));
                    VirtualRoot.Execute(new AddOrUpdateGpuOverClockDataCommand(this));
                }
            }
        }

        public int PowerCapacity {
            get => _powerCapacity;
            set {
                _powerCapacity = value;
                OnPropertyChanged(nameof(PowerCapacity));
                VirtualRoot.Execute(new AddOrUpdateGpuOverClockDataCommand(this));
            }
        }

        public int Cool {
            get => _cool;
            set {
                _cool = value;
                OnPropertyChanged(nameof(Cool));
                VirtualRoot.Execute(new AddOrUpdateGpuOverClockDataCommand(this));
            }
        }

        private GpuViewModel _gpuVm;
        public GpuViewModel GpuVm {
            get {
                if (_gpuVm == null) {
                    GpuViewModels.Current.TryGetGpuVm(Index, out _gpuVm);
                }
                return _gpuVm;
            }
        }
    }
}
