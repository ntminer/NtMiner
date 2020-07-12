using NTMiner.Gpus;
using System;

namespace NTMiner.Core.MinerServer {
    public interface IOverClockData : IOverClockInput, IEntity<Guid> {
        Guid Id { get; }
        Guid CoinId { get; }

        string Name { get; }

        GpuType GpuType { get; }
    }
}
