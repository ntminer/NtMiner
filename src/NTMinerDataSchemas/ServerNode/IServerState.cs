namespace NTMiner.ServerNode {
    public interface IServerState {
        string Address { get; }

        string Description { get; }

        int CpuPerformance { get; }

        int TotalPhysicalMemoryMb { get; }

        int AvailablePhysicalMemoryMb { get; }
    }
}
