using System;

namespace NTMiner.Core.MinerServer {
    public interface IMinerData : IMinerSign, IMinerIp {
        string WorkerName { get; }
        DateTime CreatedOn { get; }
        // TODO:考虑增加MinerActiveOn和NetActiveOn属性持久跟踪挖矿端和挖矿端群控的活动状态以实现周期清楚7天不活跃的矿机的逻辑
        Guid GroupId { get; }
        string CpuId { get; }
        string MACAddress { get; }
        string MinerName { get; }
        string WindowsLoginName { get; }
        string WindowsPassword { get; }
        Guid WorkId { get; }
        bool IsOuterUserEnabled { get; }
    }
}