using System;

namespace NTMiner.MinerClient {
    public interface IWorkerEventType : ILevelEntity<Guid> {
        string Name { get; }
    }
}
