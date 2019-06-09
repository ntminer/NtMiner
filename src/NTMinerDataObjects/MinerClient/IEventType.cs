using System;

namespace NTMiner.MinerClient {
    public interface IEventType : ILevelEntity<Guid> {
        string Name { get; }
    }
}
