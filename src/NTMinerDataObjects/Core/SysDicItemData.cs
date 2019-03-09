using System;

namespace NTMiner.Core {
    public class SysDicItemData : ISysDicItem, IDbEntity<Guid> {
        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public Guid DicId { get; set; }

        public string Code { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }

        public int SortNumber { get; set; }
    }
}
