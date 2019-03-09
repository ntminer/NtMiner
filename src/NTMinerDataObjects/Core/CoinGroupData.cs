using System;

namespace NTMiner.Core {
    public class CoinGroupData : ICoinGroup, IDbEntity<Guid> {
        public CoinGroupData() { }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public Guid GroupId { get; set; }

        public Guid CoinId { get; set; }

        public int SortNumber { get; set; }
    }
}
