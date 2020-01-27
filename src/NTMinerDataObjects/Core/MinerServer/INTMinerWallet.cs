using System;

namespace NTMiner.Core.MinerServer {
    public interface INTMinerWallet : IDbEntity<Guid> {
        Guid CoinId { get; }
        string Wallet { get; }
    }
}
