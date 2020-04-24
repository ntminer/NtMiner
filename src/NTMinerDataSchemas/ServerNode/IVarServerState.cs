namespace NTMiner.ServerNode {
    /// <summary>
    /// 服务器的随时间一直在变的状态
    /// </summary>
    public interface IVarServerState {
        /// <summary>
        /// Address是标识，标识是必须的
        /// </summary>
        string Address { get; }
        // 以下三项是动态数据
        double CpuPerformance { get; set; }
        ulong AvailablePhysicalMemory { get; }
    }
}
