using System;

namespace NTMiner.Core {
    public class WalletData : IWallet, IDbEntity<Guid> {
        public WalletData() {
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public Guid CoinId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public int SortNumber { get; set; }
    }
}
