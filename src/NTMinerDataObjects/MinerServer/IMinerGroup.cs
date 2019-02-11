using System;

namespace NTMiner.MinerServer {
    public interface IMinerGroup : IEntity<Guid> {
        string Name { get; }
        string Description { get; }
    }
}
