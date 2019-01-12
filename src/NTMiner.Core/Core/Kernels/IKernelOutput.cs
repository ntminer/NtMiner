using System;

namespace NTMiner.Core.Kernels {
    public interface IKernelOutput : IEntity<Guid> {
        string Name { get; }
    }
}
