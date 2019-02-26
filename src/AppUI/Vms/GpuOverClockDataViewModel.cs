using NTMiner.Core.Gpus;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class GpuOverClockDataViewModel : ViewModelBase, IGpuOverClockData {
        private int _index;
        private string _name;
        private int _coreClockDelta;
        private int _memoryClockDelta;
        private int _powerCapacity;
        private int _cool;
        private bool _isEnabled = true;

        public ICommand Apply { get; private set; }

        public GpuOverClockDataViewModel(int index) {
            _index = index;
            this.Apply = new DelegateCommand(() => {

            });
        }

        public GpuOverClockDataViewModel(IGpuOverClockData data) : this(data.Index) {
            _name = data.Name;
            _coreClockDelta = data.CoreClockDelta;
            _memoryClockDelta = data.MemoryClockDelta;
            _powerCapacity = data.PowerCapacity;
            _cool = data.Cool;
        }

        public void Update(IGpuOverClockData data) {
            this.Name = data.Name;
            this.CoreClockDelta = data.CoreClockDelta;
            this.MemoryClockDelta = data.MemoryClockDelta;
            this.PowerCapacity = data.PowerCapacity;
            this.Cool = data.Cool;
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
            }
        }

        public string IndexText {
            get {
                if (Index == NTMinerRoot.GpuAllId) {
                    return "统一超";
                }
                return $"GPU{Index}";
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
                _coreClockDelta = value;
                OnPropertyChanged(nameof(CoreClockDelta));
            }
        }

        public int MemoryClockDelta {
            get => _memoryClockDelta;
            set {
                _memoryClockDelta = value;
                OnPropertyChanged(nameof(MemoryClockDelta));
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
    }
}
