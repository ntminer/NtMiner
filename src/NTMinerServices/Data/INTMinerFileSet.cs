using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Data {
    public interface INTMinerFileSet {
        void AddOrUpdate(NTMinerFileData data);
        void Remove(Guid id);
        List<NTMinerFileData> GetAll();
    }
}
