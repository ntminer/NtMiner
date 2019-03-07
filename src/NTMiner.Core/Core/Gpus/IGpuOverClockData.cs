using System;

namespace NTMiner.Core.Gpus {
    public interface IGpuOverClockData : IDbEntity<Guid> {
        Guid CoinId { get; }
        int Index { get; }

        int CoreClockDelta { get; }

        int MemoryClockDelta { get; }

        int PowerCapacity { get; }

        int Cool { get; }

        bool IsEnabled { get; }
    }
}
