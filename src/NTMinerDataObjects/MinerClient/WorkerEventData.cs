using System;

namespace NTMiner.MinerClient {
    public class WorkerEventData : IWorkerEvent, IDbEntity<Guid> {
        public WorkerEventData() { }

        public static WorkerEventData Create(IWorkerEvent data) {
            if (data == null) {
                throw new ArgumentNullException(nameof(data));
            }
            if (data is WorkerEventData result) {
                return result;
            }
            return new WorkerEventData {
                Guid = data.GetId(),
                EventTypeId = data.EventTypeId,
                Content = data.Content,
                EventOn = data.EventOn
            };
        }

        public Guid GetId() {
            return this.Guid;
        }

        // Id will be auto-incremented by litedb
        public int Id { get; set; }

        public Guid Guid { get; set; }

        public Guid EventTypeId { get; set; }

        public string Content { get; set; }

        public DateTime EventOn { get; set; }
    }
}
