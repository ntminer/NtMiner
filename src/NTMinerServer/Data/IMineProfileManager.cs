using NTMiner.Profile;
using System;

namespace NTMiner.Data {
    public interface IMineProfileManager {
        MinerProfileData GetMinerProfile(Guid workId);
        void SetMinerProfile(Guid workId, MinerProfileData data);
        void SetMinerProfileProperty(Guid workId, string propertyName, object value);

        CoinProfileData GetCoinProfile(Guid workId, Guid coinId);
        void SetCoinProfileProperty(Guid workId, Guid coinId, string propertyName, object value);

        PoolProfileData GetPoolProfile(Guid workId, Guid poolId);
        void SetPoolProfileProperty(Guid workId, Guid poolId, string propertyName, object value);

        CoinKernelProfileData GetCoinKernelProfile(Guid workId, Guid coinKernelId);
        void SetCoinKernelProfileProperty(Guid workId, Guid coinKernelId, string propertyName, object value);

        GpuProfileData GetGpuProfile(Guid workId, Guid coinId);
        void SetGpuProfile(Guid workId, Guid coinId, string propertyName, object value);
    }
}
