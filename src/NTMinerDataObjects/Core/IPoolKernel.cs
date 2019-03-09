using System;

namespace NTMiner.Core.Kernels {
    public interface IPoolKernel : IEntity<Guid> {
        Guid PoolId { get; }
        Guid KernelId { get; }
        string Args { get; }
        string Description { get; }
    }
}
