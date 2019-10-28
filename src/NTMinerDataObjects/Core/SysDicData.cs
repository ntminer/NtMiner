using System;

namespace NTMiner.Core {
    public class SysDicData : ISysDic, IDbEntity<Guid> {
        public SysDicData() { }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int SortNumber { get; set; }
    }
}
