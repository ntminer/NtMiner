using System;

namespace NTMiner.ServiceContracts.DataObjects {
    public interface ICoinKernelProfile {
        Guid CoinKernelId { get; }
        bool IsDualCoinEnabled { get; }
        Guid DualCoinId { get; }
        double DualCoinWeight { get; }
        string CustomArgs { get; }
    }
}
