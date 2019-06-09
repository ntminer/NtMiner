using System;

namespace NTMiner.MinerClient {
    public interface IMinerEvent : IEntity<Guid> {
        Guid TypeId { get; }
        string Description { get; }
        DateTime EventOn { get; }
    }
}
