using System;

namespace NTMiner.MinerClient {
    public interface IKernelOutputKeyword : ILevelEntity<Guid> {
        Guid WorkerEventTypeId { get; }
        string Keyword { get; }
    }
}
