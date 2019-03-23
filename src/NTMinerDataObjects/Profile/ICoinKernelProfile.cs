using System;

namespace NTMiner.Profile {
    public interface ICoinKernelProfile {
        Guid CoinKernelId { get; }
        bool IsDualCoinEnabled { get; }
        Guid DualCoinId { get; }
        double DualCoinWeight { get; }
        bool IsAutoDualWeight { get; }
        string CustomArgs { get; }
    }
}
