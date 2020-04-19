using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerStudio {
    public interface IMinerGroupSet {
        bool Contains(Guid id);
        IEnumerable<MinerGroupData> AsEnumerable();
    }
}
