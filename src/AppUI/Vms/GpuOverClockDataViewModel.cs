using NTMiner.Core;
using NTMiner.Core.Gpus;
using System;
using System.Windows;
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
                VirtualRoot.Execute(new AddOrUpdateGpuOverClockDataCommand(this));
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
        }

        public void Update(IGpuOverClockData data) {
            this.CoinId = data.CoinId;
            this.Index = data.Index;
            this.Name = data.Name;
            this.CoreClockDelta = data.CoreClockDelta;
            this.MemoryClockDelta = data.MemoryClockDelta;
            this.PowerCapacity = data.PowerCapacity;
            this.Cool = data.Cool;
            this.IsEnabled = data.IsEnabled;
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
                if (this == GpuOverClockDataViewModels.Current.GpuAllVm(CoinId)) {
                    foreach (var item in GpuOverClockDataViewModels.Current.List(CoinId)) {
                        item.OnPropertyChanged(nameof(IsVisible));
                    }
                }
            }
        }

        public Visibility IsVisible {
            get {
                var gpuAllVm = GpuOverClockDataViewModels.Current.GpuAllVm(CoinId);
                if (gpuAllVm.IsEnabled) {
                    if (this == gpuAllVm) {
                        return Visibility.Visible;
                    }
                    else {
                        return Visibility.Collapsed;
                    }
                }
                if (this == gpuAllVm) {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
        }

        public bool IsGpuAllVm {
            get {
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
                    if (value < -400) {
                        throw new ValidationException("取值范围-400至400");
                    }
                    else if (value > 400) {
                        throw new ValidationException("取值范围-400至400");
                    }
                }
            }
        }

        public int MemoryClockDelta {
            get => _memoryClockDelta;
            set {
                if (_memoryClockDelta != value) {
                    _memoryClockDelta = value;
                    OnPropertyChanged(nameof(MemoryClockDelta));
                    if (value < -1000) {
                        throw new ValidationException("取值范围-1000至1000");
                    }
                    else if (value > 1000) {
                        throw new ValidationException("取值范围-1000至1000");
                    }
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

        public int Cool {
            get => _cool;
            set {
                _cool = value;
                OnPropertyChanged(nameof(Cool));
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
