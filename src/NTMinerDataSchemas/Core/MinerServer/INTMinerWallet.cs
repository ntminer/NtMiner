using System;

namespace NTMiner.Core.MinerServer {
    public interface INTMinerWallet : IDbEntity<Guid> {
        string CoinCode { get; }
        string Wallet { get; }
    }
}
