using System;

namespace NTMiner.Core {
    public interface IPoolKernel : IEntity<Guid> {
        Guid PoolId { get; }
        Guid KernelId { get; }
        string Args { get; }
        string Description { get; }
    }
}
