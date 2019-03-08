using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IJsonDb {
        ulong TimeStamp { get; }
        bool Exists<T>(Guid key) where T : IDbEntity<Guid>;
        T GetByKey<T>(Guid key) where T : IDbEntity<Guid>;
        IEnumerable<T> GetAll<T>() where T : IDbEntity<Guid>;
    }
}
