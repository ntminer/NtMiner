using System;

namespace NTMiner.MinerServer {
    public interface INTMinerWallet : IDbEntity<Guid> {
        Guid CoinId { get; }
        string Wallet { get; }
    }
}
