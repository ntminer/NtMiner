using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerStudio {
    public interface IMineWorkSet {
        bool Contains(Guid mineWorkId);
        IEnumerable<MineWorkData> AsEnumerable();
    }
}
