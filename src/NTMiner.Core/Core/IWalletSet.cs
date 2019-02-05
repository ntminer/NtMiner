using NTMiner;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IWalletSet : IEnumerable<IWallet> {
        int Count { get; }
        bool Contains(Guid walletId);
        bool TryGetWallet(Guid walletId, out IWallet wallet);
    }
}
