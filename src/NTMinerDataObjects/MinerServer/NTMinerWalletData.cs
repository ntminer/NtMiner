using System;

namespace NTMiner.MinerServer {
    public class NTMinerWalletData : INTMinerWallet, IDbEntity<Guid> {
        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }
        public string CoinCode { get; set; }
        public string Wallet { get; set; }
    }
}
