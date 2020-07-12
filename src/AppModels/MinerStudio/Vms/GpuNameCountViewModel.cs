using NTMiner.Gpus;
using NTMiner.Vms;
using System.Windows;

namespace NTMiner.MinerStudio.Vms {
    public class GpuNameCountViewModel : ViewModelBase, IGpuNameCount {
        private GpuType _gpuType;
        private string _name;
        private ulong _totalMemory;
        private int _count;

        public GpuNameCountViewModel(IGpuNameCount data) {
            _gpuType = data.GpuType;
            _name = data.Name;
            _totalMemory = data.TotalMemory;
            _count = data.Count;
        }

        public int Count {
            get => _count;
            set {
                if (_count != value) {
                    _count = value;
                    OnPropertyChanged(nameof(Count));
                }
            }
        }

        public GpuType GpuType {
            get => _gpuType;
            set {
                if (_gpuType != value) {
                    _gpuType = value;
                    OnPropertyChanged(nameof(GpuType));
                    OnPropertyChanged(nameof(GpuTypeText));
                    OnPropertyChanged(nameof(IsAmdIconVisible));
                    OnPropertyChanged(nameof(IsNvidiaIconVisible));
                }
            }
        }

        public string GpuTypeText {
            get {
                return this.GpuType.GetDescription();
            }
        }

        public Visibility IsAmdIconVisible {
            get {
                if (this.GpuType == GpuType.AMD) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public Visibility IsNvidiaIconVisible {
            get {
                if (this.GpuType == GpuType.NVIDIA) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
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
                    OnPropertyChanged(nameof(TotalMemoryGb));
                }
            }
        }

        public int TotalMemoryGb {
            get {
                return GpuName.ConvertToGb(this.TotalMemory);
            }
            set {
                this.TotalMemory = NTKeyword.ULongG * (ulong)value;
                OnPropertyChanged(nameof(TotalMemoryGb));
            }
        }
    }
}
