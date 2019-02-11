using System;

namespace NTMiner.MinerServer {
    public interface IWallet : IEntity<Guid> {
        Guid CoinId { get; }
        string Name { get; }
        string Address { get; }
        int SortNumber { get; }
    }
}
