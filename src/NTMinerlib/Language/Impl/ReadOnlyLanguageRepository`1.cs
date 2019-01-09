using NTMiner.Repositories;
using System;
using System.Collections.Generic;

namespace NTMiner.Language.Impl {
    public class ReadOnlyLanguageRepository<T> : IRepository<T> where T : class, IDbEntity<Guid> {
        public ReadOnlyLanguageRepository() { }

        public void Add(T entity) {
            throw new NotImplementedException();
        }

        public bool Exists(Guid key) {
            return LangJson.Instance.Exists<T>(key);
        }

        public IEnumerable<T> GetAll() {
            return LangJson.Instance.GetAll<T>();
        }

        public T GetByKey(Guid key) {
            return LangJson.Instance.GetByKey<T>(key);
        }

        public void Remove(Guid id) {
            throw new NotImplementedException();
        }

        public void Update(T entity) {
            throw new NotImplementedException();
        }
    }
}
