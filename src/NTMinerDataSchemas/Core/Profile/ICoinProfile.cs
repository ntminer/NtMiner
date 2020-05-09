using System;

namespace NTMiner.Core.Profile {
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
        bool IsLowSpeedRestartComputer { get; }
        int LowSpeedRestartComputerMinutes { get; }
        double LowSpeed { get; }
    }
}
