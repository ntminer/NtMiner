using NTMiner.Core;
using System;

namespace NTMiner.Core.Profile {
    public interface IGpuProfile : IOverClockInput, IEntity<string> {
        Guid CoinId { get; }
        int Index { get; }

        bool IsAutoFanSpeed { get; }
    }
}
