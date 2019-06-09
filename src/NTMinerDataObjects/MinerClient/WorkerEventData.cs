using System;

namespace NTMiner.MinerClient {
    public class WorkerEventData : IWorkerEvent, IDbEntity<Guid> {
        public WorkerEventData() { }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public WorkerEventSource Source { get; set; }

        public Guid TypeId { get; set; }

        public string Description { get; set; }

        public DateTime EventOn { get; set; }
    }
}
