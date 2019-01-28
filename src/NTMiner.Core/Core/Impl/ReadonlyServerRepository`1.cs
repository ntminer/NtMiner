using NTMiner.Repositories;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class ReadOnlyServerRepository<T> : IRepository<T>, IReadOnlyRepository where T : class, IDbEntity<Guid> {
        public ReadOnlyServerRepository() { }

        public void Add(T entity) {
            // noting need todo
        }

        public bool Exists(Guid key) {
            return ServerJson.Instance.Exists<T>(key);
        }

        public IEnumerable<T> GetAll() {
            return ServerJson.Instance.GetAll<T>();
        }

        public T GetByKey(Guid key) {
            return ServerJson.Instance.GetByKey<T>(key);
        }

        public void Remove(Guid id) {
            // noting need todo
        }

        public void Update(T entity) {
            // noting need todo
        }
    }
}
