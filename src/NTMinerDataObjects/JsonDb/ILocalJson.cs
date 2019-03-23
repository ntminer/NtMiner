using NTMiner.Core;
using NTMiner.MinerServer;
using NTMiner.Profile;

namespace NTMiner.JsonDb {
    public interface ILocalJson : IJsonDb {
        CoinKernelProfileData[] CoinKernelProfiles { get; }
        CoinProfileData[] CoinProfiles { get; }
        MinerProfileData MinerProfile { get; }
        MineWorkData MineWork { get; }
        PoolProfileData[] PoolProfiles { get; }
        PoolData[] Pools { get; }
        WalletData[] Wallets { get; }
    }
}