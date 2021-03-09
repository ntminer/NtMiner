using NTMiner.Core.MinerServer;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IReadOnlyNTMinerFileSet {
        IEnumerable<NTMinerFileData> AsEnumerable();
    }
}
