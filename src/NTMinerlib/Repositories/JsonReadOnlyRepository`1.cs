using NTMiner.JsonDb;
using System;
using System.Collections.Generic;

namespace NTMiner.Repositories {
    public class JsonReadOnlyRepository<T> : IRepository<T>, IJsonReadOnlyRepository where T : class, IDbEntity<Guid> {
        private readonly IJsonDb _jsonDb;
        public JsonReadOnlyRepository(IJsonDb jsonDb) {
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
