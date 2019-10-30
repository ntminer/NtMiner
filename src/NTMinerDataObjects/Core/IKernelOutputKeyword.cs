using System;

namespace NTMiner.Core {
    public interface IKernelOutputKeyword : IDbEntity<Guid> {
        Guid KernelOutputId { get; }
        string MessageType { get; }
        string Keyword { get; }
    }
}
