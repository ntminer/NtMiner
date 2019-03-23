using System;

namespace NTMiner.Core {
    public class GroupData : IGroup, IDbEntity<Guid> {
        public GroupData() { }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public int SortNumber { get; set; }
    }
}
