using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Data {
    public interface IMinerGroupSet {
        void AddOrUpdate(MinerGroupData data);
        void Remove(Guid id);
        MinerGroupData GetMinerGroup(Guid id);
        List<MinerGroupData> GetAll();
    }
}
