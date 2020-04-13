namespace NTMiner {
    public interface IWsServerNode {
        string Address { get; }
        string Description { get; }
        int MinerClientWsSessionCount { get; }
        int MinerStudioWsSessionCount { get; }
        int MinerClientSessionCount { get; }
        int MinerStudioSessionCount { get; }
        int CpuPerformance { get; }
        int TotalPhysicalMemoryMb { get; }
        int AvailablePhysicalMemoryMb { get; }
    }
}
