using NTMiner.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTMiner.Language.Impl {
    public class ReadOnlyLanguageRepository<T> : IRepository<T> where T : class, IDbEntity<Guid> {
        public void Add(T entity) {
            throw new NotImplementedException();
        }

        public bool Exists(Guid key) {
            throw new NotImplementedException();
        }

        public IList<T> GetAll() {
            throw new NotImplementedException();
        }

        public T GetByKey(Guid key) {
            throw new NotImplementedException();
        }

        public void Remove(Guid id) {
            throw new NotImplementedException();
        }

        public void Update(T entity) {
            throw new NotImplementedException();
        }
    }
}
