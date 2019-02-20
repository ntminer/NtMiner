using System;

namespace NTMiner.Core {
    public interface ICoinShare {
        Guid CoinId { get; }
        int TotalShareCount { get; }
        int AcceptShareCount { get; set; }
        double RejectPercent { get; }
        int RejectShareCount { get; set; }
        DateTime ShareOn { get; set; }
    }
}
