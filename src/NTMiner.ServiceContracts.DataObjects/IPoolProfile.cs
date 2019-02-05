using System;

namespace NTMiner {
    public interface IPoolProfile {
        Guid PoolId { get; }
        string UserName { get; }
        string Password { get; }
    }
}
