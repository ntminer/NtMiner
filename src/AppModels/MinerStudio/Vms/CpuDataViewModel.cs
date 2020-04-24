using NTMiner.ServerNode;
using NTMiner.Vms;

namespace NTMiner.MinerStudio.Vms {
    public class CpuDataViewModel : ViewModelBase, ICpuData {
        private string _clockSpeed;
        private string _identifier;
        private string _name;
        private int _numberOfLogicalCores;
        private string _processorArchitecture;
        private string _processorLevel;
        private string _vendorIdentifier;

        public CpuDataViewModel(ICpuData data) {
            _clockSpeed = data.ClockSpeed;
            _identifier = data.Identifier;
            _name = data.Name;
            _numberOfLogicalCores = data.NumberOfLogicalCores;
            _processorArchitecture = data.ProcessorArchitecture;
            _processorLevel = data.ProcessorLevel;
            _vendorIdentifier = data.VendorIdentifier;
        }

        public void Update(ICpuData data) {
            this.ClockSpeed = data.ClockSpeed;
            this.Identifier = data.Identifier;
            this.Name = data.Name;
            this.NumberOfLogicalCores = data.NumberOfLogicalCores;
            this.ProcessorArchitecture = data.ProcessorArchitecture;
            this.ProcessorLevel = data.ProcessorLevel;
            this.VendorIdentifier = data.VendorIdentifier;
        }

        public string ClockSpeed {
            get => _clockSpeed;
            set {
                if (_clockSpeed != value) {
                    _clockSpeed = value;
                    OnPropertyChanged(nameof(ClockSpeed));
                }
            }
        }

        public string Identifier {
            get => _identifier;
            set {
                if (_identifier != value) {
                    _identifier = value;
                    OnPropertyChanged(nameof(Identifier));
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

        public int NumberOfLogicalCores {
            get => _numberOfLogicalCores;
            set {
                if (_numberOfLogicalCores != value) {
                    _numberOfLogicalCores = value;
                    OnPropertyChanged(nameof(NumberOfLogicalCores));
                }
            }
        }

        public string ProcessorArchitecture {
            get => _processorArchitecture;
            set {
                if (_processorArchitecture != value) {
                    _processorArchitecture = value;
                    OnPropertyChanged(nameof(ProcessorArchitecture));
                }
            }
        }

        public string ProcessorLevel {
            get => _processorLevel;
            set {
                if (_processorLevel != value) {
                    _processorLevel = value;
                    OnPropertyChanged(nameof(ProcessorLevel));
                }
            }
        }

        public string VendorIdentifier {
            get => _vendorIdentifier;
            set {
                if (_vendorIdentifier != value) {
                    _vendorIdentifier = value;
                    OnPropertyChanged(nameof(VendorIdentifier));
                }
            }
        }
    }
}
