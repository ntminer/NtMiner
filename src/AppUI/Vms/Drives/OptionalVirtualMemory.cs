using System.Windows.Input;

namespace NTMiner.Vms {
    public class OptionalVirtualMemory : ViewModelBase {
        private readonly Drive _drive;
        private int _sizeG;
        public ICommand Set { get; private set; }

        public OptionalVirtualMemory(Drive drive, int sizeG) {
            this.Set = new DelegateCommand(() => {
                VirtualMemory.SetVirtualMemoryOfDrive(Drive, SizeMb);
            });
            _drive = drive;
            _sizeG = sizeG;
        }

        public Drive Drive {
            get {
                return _drive;
            }
        }

        public bool IsEnabled {
            get {
                return Drive.VirtualMemory.MaxSizeGb != this.SizeG;
            }
        }

        public int SizeG {
            get => _sizeG;
            set {
                _sizeG = value;
                OnPropertyChanged(nameof(SizeG));
                OnPropertyChanged(nameof(SizeGText));
                OnPropertyChanged(nameof(SizeMb));
            }
        }

        public int SizeMb {
            get {
                return SizeG * 1024;
            }
        }

        public string SizeGText {
            get {
                return SizeG + "G";
            }
        }
    }
}
