using NTMiner.ServiceContracts.DataObjects;
using System;
using System.Collections.Generic;

namespace NTMiner.Data {
    public interface IMinerGroupSet {
        void AddOrUpdate(MinerGroupData data);
        void Remove(Guid id);
        List<MinerGroupData> GetMinerGroups();
    }
}
