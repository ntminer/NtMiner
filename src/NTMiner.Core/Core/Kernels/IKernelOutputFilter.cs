using System;

namespace NTMiner.Core.Kernels {
    public interface IKernelOutputFilter : IEntity<Guid> {
        Guid KernelId { get; }
        string RegexPattern { get; }
    }
}
