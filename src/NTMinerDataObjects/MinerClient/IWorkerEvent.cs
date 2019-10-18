using System;

namespace NTMiner.MinerClient {
    public interface IWorkerEvent : IEntity<int> {
        // Id will be auto-incremented by litedb
        int Id { get; }
        WorkerEventChannel Channel { get; }
        string Content { get; }
        DateTime EventOn { get; }
    }
}
