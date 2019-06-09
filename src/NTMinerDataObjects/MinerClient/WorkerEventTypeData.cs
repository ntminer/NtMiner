using System;

namespace NTMiner.MinerClient {
    public class WorkerEventTypeData : IWorkerEventType, IDbEntity<Guid> {
        public WorkerEventTypeData() { }

        public Guid GetId() {
            return this.Id;
        }

        [LiteDB.BsonIgnore]
        public DataLevel DataLevel { get; set; }

        public void SetDataLevel(DataLevel dataLevel) {
            this.DataLevel = dataLevel;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
