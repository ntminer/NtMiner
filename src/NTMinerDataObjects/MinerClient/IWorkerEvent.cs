using System;

namespace NTMiner.MinerClient {
    public interface IWorkerEvent : IEntity<Guid> {
        // Id will be auto-incremented by litedb
        int Id { get; }
        Guid EventTypeId { get; }
        string Content { get; }
        DateTime EventOn { get; }
    }
}
