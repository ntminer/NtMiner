using System;
using System.Text;

namespace NTMiner.Core.MinerServer {
    public class NTMinerWalletData : INTMinerWallet, IDbEntity<Guid>, ISignableData {
        public NTMinerWalletData() { }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }
        public string CoinCode { get; set; }
        public string Wallet { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
