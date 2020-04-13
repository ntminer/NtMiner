using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface INTMinerWalletSet {
        bool Contains(Guid id);
        void AddOrUpdate(NTMinerWalletData data);
        void RemoveById(Guid id);
        NTMinerWalletData GetById(Guid id);
        List<NTMinerWalletData> GetAll();
    }
}
