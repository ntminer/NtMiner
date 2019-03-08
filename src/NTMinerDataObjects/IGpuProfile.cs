using System;

namespace NTMiner {
    public interface IGpuProfile : IDbEntity<Guid> {
        Guid CoinId { get; }
        int Index { get; }

        int CoreClockDelta { get; }

        int MemoryClockDelta { get; }

        int PowerCapacity { get; }

        int Cool { get; }

        bool IsEnabled { get; }
    }
}
