using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerServer {
    public interface IColumnsShowSet : IEnumerable<IColumnsShow> {
        bool TryGetColumnsShow(Guid columnsShowId, out IColumnsShow columnsShow);
        bool Contains(Guid columnsShowId);
    }
}
