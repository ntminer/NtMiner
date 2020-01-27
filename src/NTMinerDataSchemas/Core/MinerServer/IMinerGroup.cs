using System;

namespace NTMiner.Core.MinerServer {
    public interface IMinerGroup : IEntity<Guid> {
        Guid Id { get; }
        string Name { get; }
        string Description { get; }
    }
}
