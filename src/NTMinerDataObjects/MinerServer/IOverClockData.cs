using System;

namespace NTMiner.MinerServer {
    public interface IOverClockData : IOverClockInput, IEntity<Guid> {
        Guid Id { get; }
        Guid CoinId { get; }

        string Name { get; }

        GpuType GpuType { get; }
    }
}
