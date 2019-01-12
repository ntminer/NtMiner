using System;

namespace NTMiner.Core.Kernels {
    public interface IKernelOutputFilter : IEntity<Guid> {
        Guid KernelOutputId { get; }
        Guid KernelId { get; }
        string RegexPattern { get; }
    }
}
