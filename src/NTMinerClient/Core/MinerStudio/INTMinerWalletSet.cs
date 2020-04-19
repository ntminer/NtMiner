using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerStudio {
    public interface INTMinerWalletSet {
        bool TryGetNTMinerWallet(Guid id, out INTMinerWallet ntminerWallet);
        IEnumerable<INTMinerWallet> AsEnumerable();
    }
}
