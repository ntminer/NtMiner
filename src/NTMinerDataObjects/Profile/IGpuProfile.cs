using System;

namespace NTMiner.Profile {
    public interface IGpuProfile {
        Guid CoinId { get; }
        int Index { get; }

        int CoreClockDelta { get; }

        int MemoryClockDelta { get; }

        int PowerCapacity { get; }

        int Cool { get; }

        bool IsEnabled { get; }
    }
}
