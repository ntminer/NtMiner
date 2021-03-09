using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface INTMinerFileSet {
        DateTime LastChangedOn { get; }
        NTMinerFileData LatestMinerClientFile { get; }
        NTMinerFileData LatestMinerStudioFile { get; }
        void AddOrUpdate(NTMinerFileData data);
        void RemoveById(Guid id);
        IEnumerable<NTMinerFileData> AsEnumerable();
    }
}
