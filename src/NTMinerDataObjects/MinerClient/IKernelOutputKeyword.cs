using System;

namespace NTMiner.MinerClient {
    public interface IKernelOutputKeyword : ILevelEntity<Guid> {
        Guid KernelOutputId { get; }
        string WorkerMessageType { get; }
        string Keyword { get; }
    }
}
