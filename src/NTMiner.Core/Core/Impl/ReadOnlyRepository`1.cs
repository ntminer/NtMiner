using NTMiner.JsonDb;
using NTMiner.Repositories;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class ReadOnlyRepository<T> : IRepository<T>, IReadOnlyRepository where T : class, IDbEntity<Guid> {
        private readonly IJsonDb _jsonDb;
        public ReadOnlyRepository(IJsonDb jsonDb) {
            _jsonDb = jsonDb;
        }

        public void Add(T entity) {
            // noting need todo
        }

        public bool Exists(Guid key) {
            return _jsonDb.Exists<T>(key);
        }

        public IEnumerable<T> GetAll() {
            return _jsonDb.GetAll<T>();
        }

        public T GetByKey(Guid key) {
            return _jsonDb.GetByKey<T>(key);
        }

        public void Remove(Guid id) {
            // noting need todo
        }

        public void Update(T entity) {
            // noting need todo
        }
    }
}
