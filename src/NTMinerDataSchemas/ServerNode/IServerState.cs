namespace NTMiner.ServerNode {
    public interface IServerState : IVarServerState {
        string Description { get; }

        string OSInfo { get; }
        CpuData Cpu { get; }
        ulong TotalPhysicalMemory { get; }
    }
}
