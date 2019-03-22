using System;

namespace NTMiner.Profile {
    public interface IPoolProfile : IEntity<Guid> {
        Guid PoolId { get; }
        string UserName { get; }
        string Password { get; }
    }
}
