using NTMiner.Core;
using NTMiner.Core.MinerServer;
using NTMiner.Core.Profile;

namespace NTMiner.JsonDb {
    public interface ILocalJsonDb : IJsonDb {
        CoinKernelProfileData[] CoinKernelProfiles { get; }
        CoinProfileData[] CoinProfiles { get; }
        MinerProfileData MinerProfile { get; }
        MineWorkData MineWork { get; }
        PoolProfileData[] PoolProfiles { get; }
        PoolData[] Pools { get; }
        WalletData[] Wallets { get; }
    }
}