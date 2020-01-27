using System;

namespace NTMiner.Core.MinerServer {
    public interface IMinerData {
        string Id { get; }
        Guid ClientId { get; }
        string ClientName { get; }
        DateTime CreatedOn { get; }
        Guid GroupId { get; }
        string MinerIp { get; }
        string MACAddress { get; }
        string MinerName { get; }
        string WindowsLoginName { get; }
        string WindowsPassword { get; }
        Guid WorkId { get; }
    }
}