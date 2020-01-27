using System;

namespace NTMiner.Core.MinerServer {
    public class NTMinerWalletData : INTMinerWallet, IDbEntity<Guid> {
        public NTMinerWalletData() { }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }
        public Guid CoinId { get; set; }
        public string Wallet { get; set; }
    }
}
