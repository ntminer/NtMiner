namespace NTMiner.ServerNode {
    public interface IServerState {
        string Address { get; }

        string Description { get; }

        CpuData Cpu { get; }
        ulong TotalPhysicalMemory { get; }

        // 以下三项是动态数据
        double CpuPerformance { get; set; }
        float CpuTemperature { get; set; }
        ulong AvailablePhysicalMemory { get; }
    }
}
