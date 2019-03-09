using System;

namespace NTMiner.Core {
    public interface ISysDic : IEntity<Guid> {
        string Code { get; }
        string Name { get; }
        string Description { get; }
        int SortNumber { get; }
    }
}
