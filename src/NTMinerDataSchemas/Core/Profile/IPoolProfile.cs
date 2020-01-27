using System;

namespace NTMiner.Core.Profile {
    public interface IPoolProfile {
        Guid PoolId { get; }
        string UserName { get; }
        string Password { get; }
    }
}
