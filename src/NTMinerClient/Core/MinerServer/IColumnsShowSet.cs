using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerServer {
    public interface IColumnsShowSet {
        bool TryGetColumnsShow(Guid columnsShowId, out IColumnsShow columnsShow);
        bool Contains(Guid columnsShowId);
        IEnumerable<IColumnsShow> AsEnumerable();
    }
}
