using System;
using System.Collections.Generic;

namespace NTMiner.ServiceContracts {
    public interface IProfileService : IDisposable {
        MineWorkData GetMineWork(Guid workId);

        List<MineWorkData> GetMineWorks();

        MinerProfileData GetMinerProfile(Guid workId);

        CoinProfileData GetCoinProfile(Guid workId, Guid coinId);

        PoolProfileData GetPoolProfile(Guid workId, Guid poolId);

        CoinKernelProfileData GetCoinKernelProfile(Guid workId, Guid coinKernelId);
    }
}
