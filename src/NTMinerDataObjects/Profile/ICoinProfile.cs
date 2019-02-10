using System;

namespace NTMiner.Profile {
    public interface ICoinProfile {
        Guid CoinId { get; }
        Guid PoolId { get; }
        string Wallet { get; set; }
        bool IsHideWallet { get; }
        Guid CoinKernelId { get; }
        Guid DualCoinPoolId { get; }
        string DualCoinWallet { get; set; }
        bool IsDualCoinHideWallet { get; }
    }
}
