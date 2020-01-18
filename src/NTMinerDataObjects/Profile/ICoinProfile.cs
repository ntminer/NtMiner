using System;

namespace NTMiner.Profile {
    public interface ICoinProfile : ICanUpdateByReflection {
        Guid CoinId { get; }
        Guid PoolId { get; }
        Guid PoolId1 { get; }
        string Wallet { get; }
        bool IsHideWallet { get; }
        Guid CoinKernelId { get; }
        Guid DualCoinPoolId { get; }
        string DualCoinWallet { get; }
        bool IsDualCoinHideWallet { get; }
        double CalcInput { get; }
    }
}
