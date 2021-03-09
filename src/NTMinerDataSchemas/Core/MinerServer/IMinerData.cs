using System;

namespace NTMiner.Core.MinerServer {
    public interface IMinerData : IMinerSign, IMinerIp {
        string WorkerName { get; }
        DateTime CreatedOn { get; }
        Guid GroupId { get; }
        string MACAddress { get; }
        string MinerName { get; }
        string WindowsLoginName { get; }
        string WindowsPassword { get; }
        Guid WorkId { get; }
        bool IsOuterUserEnabled { get; }
    }
}