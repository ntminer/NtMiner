using System;

namespace NTMiner.MinerClient {
    public class KernelOutputKeywordData : IKernelOutputKeyword, IDbEntity<Guid> {
        public KernelOutputKeywordData() { }

        public Guid GetId() {
            return this.Id;
        }

        [LiteDB.BsonIgnore]
        public DataLevel DataLevel { get; set; }

        public void SetDataLevel(DataLevel dataLevel) {
            this.DataLevel = dataLevel;
        }

        public Guid Id { get; set; }

        public Guid WorkerEventTypeId { get; set; }

        public string Keyword { get; set; }
    }
}
