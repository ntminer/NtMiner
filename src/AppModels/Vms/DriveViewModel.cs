using NTMiner.VirtualMemory;
using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class DriveViewModel : ViewModelBase, IDrive {
        private int _virtualMemoryMaxSizeMb;

        public ICommand Set { get; private set; }

        private readonly IDrive _drive;
        public DriveViewModel(IDrive drive) {
            this._drive = drive;
            this.InitialVirtualMemoryMaxSizeMb = drive.VirtualMemoryMaxSizeMb;
            this._virtualMemoryMaxSizeMb = drive.VirtualMemoryMaxSizeMb;
            this.Set = new DelegateCommand<string>(parameter => {
                double i = double.Parse(parameter);
                if (i == 0) {
                    VirtualMemoryMaxSizeMb = 0;
                }
                else {
                    VirtualMemoryMaxSizeMb = (int)(Math.Pow(2.0, i) * 1024);
                }
            });
        }

        public string Name {
            get { return _drive.Name; }
        }

        public string DriveFormat {
            get { return _drive.DriveFormat; }
        }

        public long AvailableFreeSpace {
            get { return _drive.AvailableFreeSpace; }
        }

        public long TotalSize {
            get { return _drive.TotalSize; }
        }

        public string VolumeLabel {
            get { return _drive.VolumeLabel; }
        }

        public bool IsSystemDisk {
            get {
                return _drive.IsSystemDisk;
            }
        }

        public double HasUsedSpacePercent {
            get {
                if (TotalSize == 0) {
                    return 0;
                }
                return 1 - (double)(AvailableFreeSpace) / TotalSize;
            }
        }

        public int InitialVirtualMemoryMaxSizeMb { get; private set; }

        public int VirtualMemoryMaxSizeMb {
            get => _virtualMemoryMaxSizeMb;
            set {
                if (_virtualMemoryMaxSizeMb != value) {
                    _virtualMemoryMaxSizeMb = value;
                    OnPropertyChanged(nameof(VirtualMemoryMaxSizeMb));
                    OnPropertyChanged(nameof(VirtualMemoryMaxSizeGb));
                    OnPropertyChanged(nameof(VirtualMemoryMaxSizeGbText));
                    OnPropertyChanged(nameof(VirtualMemoryMaxSizeLog2));
                }
            }
        }

        public double VirtualMemoryMaxSizeGb {
            get {
                return VirtualMemoryMaxSizeMb / NTKeyword.DoubleK;
            }
        }

        public string VirtualMemoryMaxSizeGbText {
            get {
                return VirtualMemoryMaxSizeGb.ToString("f1") + " G"; ;
            }
        }

        public double VirtualMemoryMaxSizeLog2 {
            get {
                if (VirtualMemoryMaxSizeMb == 0) {
                    return 0;
                }
                return Math.Log(VirtualMemoryMaxSizeMb / NTKeyword.DoubleK, 2);
            }
            set {
                if (value == 0) {
                    this.VirtualMemoryMaxSizeMb = 0;
                }
                else {
                    this.VirtualMemoryMaxSizeMb = (int)(Math.Pow(2, value) * 1024);
                }
                OnPropertyChanged(nameof(VirtualMemoryMaxSizeLog2));
            }
        }
    }
}
