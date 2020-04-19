namespace NTMiner.VirtualMemory {
    public interface IDrive {
        string Name { get; }
        string DriveFormat { get; }
        long AvailableFreeSpace { get; }
        long TotalSize { get; }
        string VolumeLabel { get; }
        bool IsSystemDisk { get; }
        int VirtualMemoryMaxSizeMb { get; }
    }
}
