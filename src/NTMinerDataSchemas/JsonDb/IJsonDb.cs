using System;
using System.Collections.Generic;

namespace NTMiner.JsonDb {
    public interface IJsonDb {
        long TimeStamp { get; }
        bool Exists<T>(Guid key) where T : IDbEntity<Guid>;
        T GetByKey<T>(Guid key) where T : IDbEntity<Guid>;
        IEnumerable<T> GetAll<T>() where T : IDbEntity<Guid>;
    }
}
