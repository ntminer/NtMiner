using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Data {
    public interface INTMinerWalletSet {
        bool Contains(Guid id);
        void AddOrUpdate(NTMinerWalletData data);
        void Remove(Guid id);
        NTMinerWalletData GetNTMinerWallet(Guid id);
        List<NTMinerWalletData> GetAll();
    }
}
