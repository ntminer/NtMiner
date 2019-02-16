using System;
using System.IO;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class Drive : ViewModelBase {
        private DriveInfo _driveInfo;

        public ICommand Set { get; private set; }

        public Drive(DriveInfo driveInfo) {
            _driveInfo = driveInfo;
            if (driveInfo.DriveType != DriveType.Fixed) {
                throw new InvalidProgramException();
            }
            this.Set = new DelegateCommand<string>(parameter => {
                double i = double.Parse(parameter);
                if (i == 0) {
                    VirtualMemory.MaxSizeMb = 0;
                }
                else {
                    VirtualMemory.MaxSizeMb = (int)(Math.Pow(2.0, i) * 1024);
                }
            });
        }

        public string Name {
            get { return _driveInfo.Name; }
        }

        public VirtualMemory VirtualMemory {
            get {
                if (VirtualMemorySet.Instance.Contains(this.Name)) {
                    return VirtualMemorySet.Instance[this.Name];
                }
                return VirtualMemory.Empty;
            }
        }

        public string DriveFormat {
            get { return _driveInfo.DriveFormat; }
        }
        public long AvailableFreeSpace {
            get { return _driveInfo.AvailableFreeSpace; }
        }
        public long TotalSize {
            get { return _driveInfo.TotalSize; }
        }

        public double HasUsedSpacePercent {
            get {
                return 1 - (double)(AvailableFreeSpace) / TotalSize;
            }
        }

        public string VolumeLabel {
            get { return _driveInfo.VolumeLabel; }
        }

        public bool IsSystemDisk {
            get {
                string systemFolder = Environment.GetFolderPath(Environment.SpecialFolder.System);
                return systemFolder.StartsWith(this.Name);
            }
        }
    }
}
