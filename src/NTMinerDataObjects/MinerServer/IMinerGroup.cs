using System;

namespace NTMiner.MinerServer {
    public interface IMinerGroup : IEntity<Guid> {
        Guid Id { get; }
        string Name { get; }
        string Description { get; }
    }
}
