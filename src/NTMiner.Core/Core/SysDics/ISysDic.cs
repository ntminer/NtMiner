using System;

namespace NTMiner.Core.SysDics {
    public interface ISysDic : IEntity<Guid> {
        string Code { get; }
        string Name { get; }
        string Description { get; }
        int SortNumber { get; }
    }
}
