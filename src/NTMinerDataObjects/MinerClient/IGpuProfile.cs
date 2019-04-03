using System;

namespace NTMiner.MinerClient {
    public interface IGpuProfile : IOverClockInput, IEntity<string> {
        Guid CoinId { get; }
        int Index { get; }

        bool IsGuardTemp { get; }

        int GuardTemp { get; }
    }
}
