using System;

namespace NTMiner.Core {
    public interface IWallet : IEntity<Guid> {
        Guid Id { get; }
        Guid CoinId { get; }
        string Name { get; }
        string Address { get; }
        int SortNumber { get; }
    }
}
