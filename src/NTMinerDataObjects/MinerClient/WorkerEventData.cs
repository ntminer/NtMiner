using System;

namespace NTMiner.MinerClient {
    public class WorkerEventData : IWorkerEvent, IDbEntity<int> {
        public WorkerEventData() { }

        public int GetId() {
            return this.Id;
        }

        // Id will be auto-incremented by litedb
        public int Id { get; set; }

        public Guid EventTypeId { get; set; }

        public string Content { get; set; }

        public DateTime EventOn { get; set; }
    }
}
