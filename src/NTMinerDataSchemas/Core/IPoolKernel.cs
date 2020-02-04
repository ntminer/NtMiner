using System;

namespace NTMiner.Core {
    public interface IPoolKernel : IEntity<Guid> {
        Guid Id { get; }
        Guid PoolId { get; }
        Guid KernelId { get; }
        string Args { get; }
    }
}
