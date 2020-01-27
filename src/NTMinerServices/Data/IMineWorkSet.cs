using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Data {
    public interface IMineWorkSet {
        bool Contains(Guid workId);
        void AddOrUpdate(MineWorkData data);
        void Remove(Guid id);
        MineWorkData GetMineWork(Guid workId);
        List<MineWorkData> GetAll();
    }
}
