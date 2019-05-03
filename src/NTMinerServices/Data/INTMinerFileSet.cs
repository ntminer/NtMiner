using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Data {
    public interface INTMinerFileSet {
        NTMinerFileData LatestMinerClientFile { get; }
        void AddOrUpdate(NTMinerFileData data);
        void Remove(Guid id);
        List<NTMinerFileData> GetAll();
    }
}
