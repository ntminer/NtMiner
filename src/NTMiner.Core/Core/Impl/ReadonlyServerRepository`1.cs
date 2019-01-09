using NTMiner.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class ReadOnlyServerRepository<T> : IRepository<T> where T : class, IDbEntity<Guid> {
        public ReadOnlyServerRepository() { }

        public void Add(T entity) {
            throw new NotImplementedException();
        }

        public bool Exists(Guid key) {
            return ServerJson.Instance.Exists<T>(key);
        }

        public IList<T> GetAll() {
            return ServerJson.Instance.GetAll<T>().ToList();
        }

        public T GetByKey(Guid key) {
            return ServerJson.Instance.GetByKey<T>(key);
        }

        public void Remove(Guid id) {
            throw new NotImplementedException();
        }

        public void Update(T entity) {
            throw new NotImplementedException();
        }
    }
}
