using System;

namespace NTMiner {
    public interface IMineWork : IEntity<Guid> {
        string Name { get; }
        string Description { get; }
    }
}
