using NTMiner.MinerServer;
using NTMiner.Profile;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Profiles {
    public interface IWorkProfile : IMinerProfile {
        IMineWork MineWork { get; }

        ICoinKernelProfile GetCoinKernelProfile(Guid coinKernelId);
        void SetCoinKernelProfileProperty(Guid coinKernelId, string propertyName, object value);
        ICoinProfile GetCoinProfile(Guid coinId);
        void SetCoinProfileProperty(Guid coinId, string propertyName, object value);
        IPoolProfile GetPoolProfile(Guid poolId);
        void SetPoolProfileProperty(Guid poolId, string propertyName, object value);
        bool TryGetWallet(Guid walletId, out IWallet wallet);
        IGpuProfile GetGpuOverClockData(Guid coinId, int gpuIndex);
        IUser GetUser(string loginName);

        List<ICoinKernelProfile> GetCoinKernelProfiles();
        List<ICoinProfile> GetCoinProfiles();
        List<IGpuProfile> GetGpuOverClocks();
        List<IPool> GetPools();
        List<IPoolProfile> GetPoolProfiles();
        List<IUser> GetUsers();
        List<IWallet> GetWallets();
    }
}
