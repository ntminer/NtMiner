using System;

namespace NTMiner.MinerClient {
    public interface IKernelEvent : ILevelEntity<Guid> {
        Guid WorkerEventTypeId { get; }
        string Keyword { get; }
    }
}
