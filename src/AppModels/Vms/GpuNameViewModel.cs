using NTMiner.Core;
using NTMiner.Core.Gpus;

namespace NTMiner.Vms {
    public class GpuNameViewModel : ViewModelBase, IGpuName {
        private GpuType _gpuType;
        private string _name;
        private ulong _totalMemory;

        public GpuNameViewModel(IGpuName data) {
            _gpuType = data.GpuType;
            _name = data.Name;
            _totalMemory = data.TotalMemory;
        }

        public GpuType GpuType {
            get => _gpuType;
            set {
                if (_gpuType != value) {
                    _gpuType = value;
                    OnPropertyChanged(nameof(GpuType));
                }
            }
        }

        public string Name {
            get => _name;
            set {
                if (_name != value) {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public ulong TotalMemory {
            get => _totalMemory;
            set {
                if (_totalMemory != value) {
                    _totalMemory = value;
                    OnPropertyChanged(nameof(TotalMemory));
                }
            }
        }

        public bool IsValid() {
            return GpuName.IsValid(this.GpuType, this.Name, this.TotalMemory);
        }
    }
}
