using NTMiner.Core.Gpus;

namespace NTMiner.Vms {
    public class GpuClockDeltaViewModel : ViewModelBase, IGpuClockDelta {
        public static readonly GpuClockDeltaViewModel Empty = new GpuClockDeltaViewModel();

        private int _coreClockDeltaMin;
        private int _coreClockDeltaMax;
        private int _memoryClockDeltaMin;
        private int _memoryClockDeltaMax;

        private GpuClockDeltaViewModel() { }

        public GpuClockDeltaViewModel(IGpuClockDelta data) {
            _coreClockDeltaMin = data.CoreClockDeltaMin;
            _coreClockDeltaMax = data.CoreClockDeltaMax;
            _memoryClockDeltaMin = data.MemoryClockDeltaMin;
            _memoryClockDeltaMax = data.MemoryClockDeltaMax;
        }

        public int CoreClockDeltaMin {
            get => _coreClockDeltaMin;
            set {
                _coreClockDeltaMin = value;
                OnPropertyChanged(nameof(CoreClockDeltaMin));
                OnPropertyChanged(nameof(CoreClockDeltaMinMText));
                OnPropertyChanged(nameof(CoreClockDeltaMinMaxMText));
            }
        }

        public string CoreClockDeltaMinMText {
            get {
                return (this.CoreClockDeltaMin / 1000).ToString();
            }
        }

        public int CoreClockDeltaMax {
            get => _coreClockDeltaMax;
            set {
                _coreClockDeltaMax = value;
                OnPropertyChanged(nameof(CoreClockDeltaMax));
                OnPropertyChanged(nameof(CoreClockDeltaMaxMText));
                OnPropertyChanged(nameof(CoreClockDeltaMinMaxMText));
            }
        }

        public string CoreClockDeltaMaxMText {
            get {
                return (this.CoreClockDeltaMax / 1000).ToString();
            }
        }

        public string CoreClockDeltaMinMaxMText {
            get {
                return $"{CoreClockDeltaMinMText} - {CoreClockDeltaMaxMText}";
            }
        }

        public int MemoryClockDeltaMin {
            get => _memoryClockDeltaMin;
            set {
                _memoryClockDeltaMin = value;
                OnPropertyChanged(nameof(MemoryClockDeltaMin));
                OnPropertyChanged(nameof(MemoryClockDeltaMinMText));
                OnPropertyChanged(nameof(MemoryClockDeltaMinMaxMText));
            }
        }

        public string MemoryClockDeltaMinMText {
            get {
                return (this.MemoryClockDeltaMin / 1000).ToString();
            }
        }

        public int MemoryClockDeltaMax {
            get => _memoryClockDeltaMax;
            set {
                _memoryClockDeltaMax = value;
                OnPropertyChanged(nameof(MemoryClockDeltaMax));
                OnPropertyChanged(nameof(MemoryClockDeltaMaxMText));
                OnPropertyChanged(nameof(MemoryClockDeltaMinMaxMText));
            }
        }

        public string MemoryClockDeltaMaxMText {
            get {
                return (this.MemoryClockDeltaMax / 1000).ToString();
            }
        }

        public string MemoryClockDeltaMinMaxMText {
            get {
                return $"{MemoryClockDeltaMinMText} - {MemoryClockDeltaMaxMText}";
            }
        }
    }
}
