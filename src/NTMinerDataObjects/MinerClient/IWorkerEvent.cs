using System;

namespace NTMiner.MinerClient {
    public interface IWorkerEvent : IEntity<Guid> {
        Guid EventTypeId { get; }
        string Content { get; }
        DateTime EventOn { get; }
    }
}
