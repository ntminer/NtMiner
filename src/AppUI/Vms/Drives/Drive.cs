using System;
using System.IO;

namespace NTMiner.Vms {
    public class Drive : ViewModelBase {
        private DriveInfo _driveInfo;

        public Drive(DriveInfo driveInfo) {
            _driveInfo = driveInfo;
            if (driveInfo.DriveType != DriveType.Fixed) {
                throw new InvalidProgramException();
            }
        }

        public void Refresh(DriveInfo driveInfo) {
            _driveInfo = driveInfo;
            if (driveInfo.DriveType != DriveType.Fixed) {
                throw new InvalidProgramException();
            }
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(VirtualMemory));
            OnPropertyChanged(nameof(HasVirtualMemory));
            OnPropertyChanged(nameof(DriveFormat));
            OnPropertyChanged(nameof(AvailableFreeSpace));
            OnPropertyChanged(nameof(TotalSize));
            OnPropertyChanged(nameof(HasUsedSpacePercent));
            OnPropertyChanged(nameof(VolumeLabel));
            OnPropertyChanged(nameof(IsSystemDisk));
        }

        public string Name {
            get { return _driveInfo.Name; }
        }

        public VirtualMemory VirtualMemory {
            get {
                if (VirtualMemories.Instance.Contains(this.Name)) {
                    return VirtualMemories.Instance[this.Name];
                }
                return VirtualMemory.Empty;
            }
        }

        public bool HasVirtualMemory {
            get {
                return this.VirtualMemory != null && this.VirtualMemory != VirtualMemory.Empty;
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

        private const long _m = 1024 * 1024;
        private const long _g = _m * 1024;
        private OptionalVirtualMemories _optionalVirtualMemories;
        public OptionalVirtualMemories OptionalVirtualMemories {
            get {
                if (_optionalVirtualMemories == null) {
                    long value = this.AvailableFreeSpace + VirtualMemory.MaxSizeMb * _m - _g;
                    _optionalVirtualMemories = new OptionalVirtualMemories(this, value, this.VirtualMemory);
                }
                return _optionalVirtualMemories;
            }
        }
    }
}
