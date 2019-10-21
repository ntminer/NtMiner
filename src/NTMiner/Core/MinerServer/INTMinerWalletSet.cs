using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerServer {
    public interface INTMinerWalletSet : IEnumerable<INTMinerWallet> {
        bool TryGetNTMinerWallet(Guid id, out INTMinerWallet ntMinerWallet);
    }
}
