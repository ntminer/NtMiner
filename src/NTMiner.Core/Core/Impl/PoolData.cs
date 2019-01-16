using System;

namespace NTMiner.Core.Impl {
    public class PoolData : IPool, IDbEntity<Guid> {
        public PoolData() {
        }

        public Guid GetId() {
            return this.Id;
        }

        [LiteDB.BsonIgnore]
        public DataLevel DataLevel { get; set; }

        public void SetDataLevel(DataLevel dataLevel) {
            this.DataLevel = dataLevel;
        }

        public Guid Id { get; set; }

        public Guid CoinId { get; set; }

        public string Name { get; set; }

        public string Server { get; set; }

        public string Url { get; set; }

        public int SortNumber { get; set; }

        public PublishStatus PublishState { get; set; }

        public string Description { get; set; }

        public string Wallet { get; set; }
    }
}
