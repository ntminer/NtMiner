using System;

namespace NTMiner.Core {
    public class WorkerMessageData : IWorkerMessage, IDbEntity<Guid> {
        public WorkerMessageData() { }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Channel { get; set; }

        public string Provider { get; set; }

        public string MessageType { get; set; }

        public string Content { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
