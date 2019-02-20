using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerServer {
    public interface IMinerGroupSet : IEnumerable<IMinerGroup> {
        int Count { get; }
        bool Contains(Guid id);
        bool TryGetMinerGroup(Guid id, out IMinerGroup group);
    }
}
