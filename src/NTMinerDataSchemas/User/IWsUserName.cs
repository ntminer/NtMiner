using System;

namespace NTMiner.User {
    public interface IWsUserName {
        NTMinerAppType ClientType { get; }
        string ClientVersion { get; }
        Guid ClientId { get; }
        string UserId { get; }
        bool IsBinarySupported { get; }
    }
}
