using NTMiner.MinerServer;
using NTMiner.Profile;
using System.Collections.Generic;

namespace NTMiner {
    public interface IProfileController {
        List<MineWorkData> MineWorks();
        MinerProfileData MinerProfile(MinerProfileRequest request);
        CoinProfileData CoinProfile(CoinProfileRequest request);
        PoolProfileData PoolProfile(PoolProfileRequest request);
        CoinKernelProfileData CoinKernelProfile(CoinKernelProfileRequest request);
    }
}
