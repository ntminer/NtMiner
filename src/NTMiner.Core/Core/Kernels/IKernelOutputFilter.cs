using System;

namespace NTMiner.Core.Kernels {
    public interface IKernelOutputFilter : IEntity<Guid> {
        Guid KernelOutputId { get; }
        string RegexPattern { get; }
    }
}
