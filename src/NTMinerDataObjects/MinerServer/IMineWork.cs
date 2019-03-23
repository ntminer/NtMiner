using System;

namespace NTMiner.MinerServer {
    public interface IMineWork : IEntity<Guid> {
        string Name { get; }
        string Description { get; }
    }
}
