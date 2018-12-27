using System;

namespace NTMiner.Core.SysDics {
    public interface ISysDicItem : IEntity<Guid> {
        Guid DicId { get; }
        string Code { get; }
        string Value { get; }
        string Description { get; }
        int SortNumber { get; }
    }
}
