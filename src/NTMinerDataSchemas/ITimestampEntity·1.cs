using System;

namespace NTMiner {
    public interface ITimestampEntity<T> : IEntity<T> {
        DateTime CreatedOn { get; }
        DateTime ModifiedOn { get; }
    }
}
