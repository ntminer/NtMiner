using NTMiner.Core.Gpus;

namespace NTMiner.Vms {
    public class GpuClockDeltaViewModel : ViewModelBase, IGpuClockDelta {
        private int _coreClockDeltaMin;
        private int _coreClockDeltaMax;
        private int _memoryClockDeltaMin;
        private int _memoryClockDeltaMax;

        public int CoreClockDeltaMin {
            get => _coreClockDeltaMin;
            set {
                _coreClockDeltaMin = value;
                OnPropertyChanged(nameof(CoreClockDeltaMin));
                OnPropertyChanged(nameof(CoreClockDeltaMinMText));
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
            }
        }

        public string CoreClockDeltaMaxMText {
            get {
                return (this.CoreClockDeltaMax / 1000).ToString();
            }
        }

        public int MemoryClockDeltaMin {
            get => _memoryClockDeltaMin;
            set {
                _memoryClockDeltaMin = value;
                OnPropertyChanged(nameof(MemoryClockDeltaMin));
                OnPropertyChanged(nameof(MemoryClockDeltaMinMText));
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
            }
        }

        public string MemoryClockDeltaMaxMText {
            get {
                return (this.MemoryClockDeltaMax / 1000).ToString();
            }
        }
    }
}
