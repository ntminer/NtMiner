using System;

namespace NTMiner.Core {
    public interface ISysDicItem : IEntity<Guid> {
        Guid DicId { get; }
        string Code { get; }
        string Value { get; }
        string Description { get; }
        int SortNumber { get; }
    }
}
