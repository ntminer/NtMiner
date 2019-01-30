using NTMiner.ServiceContracts.ControlCenter.DataObjects;
using System;
using System.Collections.Generic;

namespace NTMiner.Data {
    public interface IWalletSet {
        void AddOrUpdate(WalletData data);
        void Remove(Guid id);
        List<WalletData> GetWallets();
    }
}
