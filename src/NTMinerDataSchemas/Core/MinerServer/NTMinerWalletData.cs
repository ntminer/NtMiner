using System;

namespace NTMiner.Core.MinerServer {
    [DataSchemaId("39BDF505-64AF-4F1B-B6F5-549A378EBE8D")]
    public class NTMinerWalletData : INTMinerWallet, IDbEntity<Guid> {
        public NTMinerWalletData() { }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }
        public string CoinCode { get; set; }
        public string Wallet { get; set; }
    }
}
