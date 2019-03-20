using NTMiner.MinerServer;
using NTMiner.Profile;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Profiles {
    public interface IWorkProfile : IMinerProfile {
        IMineWork MineWork { get; }

        ICoinProfile GetCoinProfile(Guid coinId);
        ICoinKernelProfile GetCoinKernelProfile(Guid coinKernelId);
        IPoolProfile GetPoolProfile(Guid poolId);
        bool TryGetWallet(Guid walletId, out IWallet wallet);

        void SetMinerProfileProperty(string propertyName, object value);
        void SetCoinKernelProfileProperty(Guid coinKernelId, string propertyName, object value);
        void SetCoinProfileProperty(Guid coinId, string propertyName, object value);
        void SetPoolProfileProperty(Guid poolId, string propertyName, object value);

        List<ICoinKernelProfile> GetCoinKernelProfiles();
        List<ICoinProfile> GetCoinProfiles();
        List<IPool> GetPools();
        List<IPoolProfile> GetPoolProfiles();
        List<IWallet> GetWallets();

        string GetSha1();
    }
}
