using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerServer {
    public interface IMinerGroupSet : IEnumerable<IMinerGroup> {
        bool TryGetMinerGroup(Guid id, out IMinerGroup group);
    }
}
