using System;

namespace NTMiner.MinerClient {
    public interface IGpuProfile : IEntity<string> {
        Guid CoinId { get; }
        int Index { get; }

        int CoreClockDelta { get; }

        int MemoryClockDelta { get; }

        int PowerCapacity { get; }

        int Cool { get; }
    }
}
