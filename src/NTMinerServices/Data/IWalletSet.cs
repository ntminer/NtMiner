using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.Data {
    public interface IWalletSet {
        void AddOrUpdate(WalletData data);
        void Remove(Guid id);
        List<WalletData> GetAll();
    }
}
