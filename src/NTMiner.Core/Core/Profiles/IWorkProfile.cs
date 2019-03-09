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
        IGpuProfile GetGpuProfile(Guid coinId, int gpuIndex);
        IUser GetUser(string loginName);

        void SetMinerProfileProperty(string propertyName, object value);
        void SetCoinKernelProfileProperty(Guid coinKernelId, string propertyName, object value);
        void SetCoinProfileProperty(Guid coinId, string propertyName, object value);
        void SetPoolProfileProperty(Guid poolId, string propertyName, object value);

        List<ICoinKernelProfile> GetCoinKernelProfiles();
        List<ICoinProfile> GetCoinProfiles();
        List<IGpuProfile> GetGpuOverClocks();
        List<IPool> GetPools();
        List<IPoolProfile> GetPoolProfiles();
        List<IUser> GetUsers();
        List<IWallet> GetWallets();

        string GetSha1();
    }
}
