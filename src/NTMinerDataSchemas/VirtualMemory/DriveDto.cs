using System;
using System.IO;

namespace NTMiner.VirtualMemory {
    public class DriveDto : IDrive {
        public DriveDto() { }

        public DriveDto(DriveInfo info, int virtualMemoryMaxSizeMb) {
            this.Name = info.Name;
            this.DriveFormat = info.DriveFormat;
            this.AvailableFreeSpace = info.AvailableFreeSpace;
            this.TotalSize = info.TotalSize;
            this.VolumeLabel = info.VolumeLabel;
            string systemFolder = Environment.GetFolderPath(Environment.SpecialFolder.System);
            this.IsSystemDisk = systemFolder.StartsWith(info.Name);
            this.VirtualMemoryMaxSizeMb = virtualMemoryMaxSizeMb;
        }

        public string Name { get; set; }

        public string DriveFormat { get; set; }

        public long AvailableFreeSpace { get; set; }

        public long TotalSize { get; set; }

        public string VolumeLabel { get; set; }

        public bool IsSystemDisk { get; set; }

        public int VirtualMemoryMaxSizeMb { get; set; }
    }
}
