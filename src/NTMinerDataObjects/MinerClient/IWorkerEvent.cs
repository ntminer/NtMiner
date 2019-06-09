using System;

namespace NTMiner.MinerClient {
    public interface IWorkerEvent : IEntity<Guid> {
        Guid TypeId { get; }
        WorkerEventSource Source { get; }
        string Description { get; }
        DateTime EventOn { get; }
    }
}
