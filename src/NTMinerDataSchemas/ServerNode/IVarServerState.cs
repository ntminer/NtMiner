namespace NTMiner.ServerNode {
    /// <summary>
    /// 服务器的随时间一直在变的状态
    /// </summary>
    public interface IVarServerState {
        /// <summary>
        /// Address是标识，标识是必须的
        /// </summary>
        string Address { get; }
        /// <summary>
        /// 本机CPU总使用率
        /// </summary>
        double CpuPerformance { get; }
        /// <summary>
        /// 本进程CPU使用率
        /// </summary>
        double ProcessPerformance { get; }
        /// <summary>
        /// 本机剩余内存
        /// </summary>
        ulong AvailablePhysicalMemory { get; }
        /// <summary>
        /// 本进程所用内存
        /// </summary>
        ulong WorkingSet { get; }
    }
}
