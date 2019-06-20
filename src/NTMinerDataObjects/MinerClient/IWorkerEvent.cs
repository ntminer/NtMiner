using System;

namespace NTMiner.MinerClient {
    public interface IWorkerEvent : IEntity<Guid> {
        Guid EventTypeId { get; }
        string Description { get; }
        DateTime EventOn { get; }
    }
}
