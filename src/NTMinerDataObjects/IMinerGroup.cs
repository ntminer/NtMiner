using System;

namespace NTMiner {
    public interface IMinerGroup : IEntity<Guid> {
        string Name { get; }
        string Description { get; }
    }
}
