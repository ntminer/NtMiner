using System;

namespace NTMiner.MinerClient {
    public interface IEventType : IEntity<Guid> {
        string Name { get; }
    }
}
