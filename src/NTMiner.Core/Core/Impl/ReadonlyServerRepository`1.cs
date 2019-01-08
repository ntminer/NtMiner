using NTMiner.Repositories;
using System;
using System.Collections.Generic;
using System.IO;

namespace NTMiner.Core.Impl {
    public class ReadOnlyServerRepository<T> : IRepository<T> where T : class, IDbEntity<Guid> {
        private static readonly ServerJson data;

        static ReadOnlyServerRepository() {
            if (!File.Exists(SpecialPath.LocalJsonFileFullName)) {
                data = new ServerJson();
            }
            else {
                data = Global.JsonSerializer.Deserialize<ServerJson>(File.ReadAllText(SpecialPath.LocalJsonFileFullName));
            }
        }

        public ReadOnlyServerRepository() { }

        public void Add(T entity) {
            throw new NotImplementedException();
        }

        public bool Exists(Guid key) {
            return data.Exists<T>(key);
        }

        public IList<T> GetAll() {
            return data.GetAll<T>();
        }

        public T GetByKey(Guid key) {
            return data.GetByKey<T>(key);
        }

        public void Remove(Guid id) {
            throw new NotImplementedException();
        }

        public void Update(T entity) {
            throw new NotImplementedException();
        }
    }
}
