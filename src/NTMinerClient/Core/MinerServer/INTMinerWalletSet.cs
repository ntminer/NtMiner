using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerServer {
    public interface INTMinerWalletSet {
        bool TryGetNTMinerWallet(Guid id, out INTMinerWallet ntMinerWallet);
        IEnumerable<INTMinerWallet> AsEnumerable();
    }
}
