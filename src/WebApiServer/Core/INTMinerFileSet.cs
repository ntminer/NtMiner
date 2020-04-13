using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface INTMinerFileSet {
        NTMinerFileData LatestMinerClientFile { get; }
        void AddOrUpdate(NTMinerFileData data);
        void RemoveById(Guid id);
        List<NTMinerFileData> GetAll();
    }
}
