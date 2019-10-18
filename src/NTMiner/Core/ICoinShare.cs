using System;

namespace NTMiner.Core {
    public interface ICoinShare {
        Guid CoinId { get; }
        int TotalShareCount { get; }
        int AcceptShareCount { get; }
        double RejectPercent { get; }
        int RejectShareCount { get; }
        DateTime ShareOn { get; }
    }
}
