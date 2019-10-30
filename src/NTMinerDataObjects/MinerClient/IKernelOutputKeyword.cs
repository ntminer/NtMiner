using System;

namespace NTMiner.MinerClient {
    public interface IKernelOutputKeyword : IDbEntity<Guid> {
        Guid KernelOutputId { get; }
        string MessageType { get; }
        string Keyword { get; }
    }
}
