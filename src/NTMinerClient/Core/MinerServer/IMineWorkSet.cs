using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerServer {
    public interface IMineWorkSet {
        bool TryGetMineWork(Guid mineWorkId, out IMineWork mineWork);
        bool Contains(Guid mineWorkId);
        IEnumerable<IMineWork> AsEnumerable();
    }
}
