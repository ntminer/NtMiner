using System;

namespace NTMiner.Profile {
    public interface IPoolProfile {
        Guid PoolId { get; }
        string UserName { get; }
        string Password { get; }
    }
}
