using System;
using System.Collections.Generic;

namespace NTMiner.Repositories {
    public interface IRepository<T> where T : class, IDbEntity<Guid> {
        IEnumerable<T> GetAll();

        T GetByKey(Guid key);

        bool Exists(Guid key);

        void Add(T entity);

        void Update(T entity);

        void Remove(Guid id);
    }
}
