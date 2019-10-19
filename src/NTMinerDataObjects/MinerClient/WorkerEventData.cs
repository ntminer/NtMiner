using System;

namespace NTMiner.MinerClient {
    public class WorkerEventData : IWorkerEvent, IDbEntity<Guid> {
        public WorkerEventData() { }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Channel { get; set; }

        public string Provider { get; set; }

        public string EventType { get; set; }

        public string Content { get; set; }

        public DateTime EventOn { get; set; }
    }
}
