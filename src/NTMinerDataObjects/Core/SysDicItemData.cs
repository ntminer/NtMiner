using System;

namespace NTMiner.Core {
    public class SysDicItemData : ISysDicItem, IDbEntity<Guid> {
        public SysDicItemData() { }

        public Guid GetId() {
            return this.Id;
        }

        [LiteDB.BsonIgnore]
        public DataLevel DataLevel { get; set; }

        public void SetDataLevel(DataLevel dataLevel) {
            this.DataLevel = dataLevel;
        }

        public Guid Id { get; set; }

        public Guid DicId { get; set; }

        public string Code { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }

        public int SortNumber { get; set; }
    }
}
