using System;

namespace NTMiner.Core {
    public interface IGroup : IEntity<Guid> {
        string Name { get; }
        int SortNumber { get; }
    }
}
