using System;

namespace NTMiner.Core {
    public interface IKernelOutputFilter : IEntity<Guid> {
        Guid KernelOutputId { get; }
        string RegexPattern { get; }
    }
}
