using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerServer {
    public interface IMineWorkSet : IEnumerable<IMineWork> {
        bool TryGetMineWork(Guid mineWorkId, out IMineWork mineWork);
        bool Contains(Guid mineWorkId);
    }
}
