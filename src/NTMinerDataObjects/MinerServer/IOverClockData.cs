using System;

namespace NTMiner.MinerServer {
    public interface IOverClockData : IEntity<Guid> {
        Guid CoinId { get; }

        string Name { get; }

        int CoreClockDelta { get; }

        int MemoryClockDelta { get; }

        int PowerCapacity { get; }

        int Cool { get; }
    }
}
