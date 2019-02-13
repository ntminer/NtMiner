using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Profiles {
    public interface IWalletSet : IEnumerable<IWallet> {
        int WalletCount { get; }
        bool ContainsWallet(Guid walletId);
        bool TryGetWallet(Guid walletId, out IWallet wallet);
    }
}
