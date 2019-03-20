using System;

namespace NTMiner.Profile {
    public interface ICoinProfile {
        Guid CoinId { get; }
        Guid PoolId { get; }
        string Wallet { get; }
        bool IsHideWallet { get; }
        Guid CoinKernelId { get; }
        Guid DualCoinPoolId { get; }
        string DualCoinWallet { get; }
        bool IsDualCoinHideWallet { get; }        
    }
}
