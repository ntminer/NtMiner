using System;

namespace NTMiner.MinerClient {
    public interface IKernelOutputPicker : ILevelEntity<Guid> {
        Guid WorkerEventTypeId { get; }
        string Keyword { get; }
    }
}
