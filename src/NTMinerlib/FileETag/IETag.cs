using System;

namespace NTMiner.FileETag {
    public interface IETag : IEntity<Guid> {
        string Key { get; }
        string Value { get; }
    }
}
