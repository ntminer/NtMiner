using System;

namespace NTMiner.MinerServer {
    public interface INTMinerWallet : IDbEntity<Guid> {
        string CoinCode { get; }
        string Wallet { get; }
    }
}
