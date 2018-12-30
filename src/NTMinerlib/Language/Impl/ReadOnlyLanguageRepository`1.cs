using NTMiner.Repositories;
using System;
using System.Collections.Generic;
using System.IO;

namespace NTMiner.Language.Impl {
    public class ReadOnlyLanguageRepository<T> : IRepository<T> where T : class, IDbEntity<Guid> {
        private static readonly LangJson data;

        static ReadOnlyLanguageRepository() {
            string langJsonFileFullName = Path.Combine(Global.GlobalDirFullName, "lang.json");
            if (!File.Exists(langJsonFileFullName)) {
                data = new LangJson();
            }
            else {
                data = Global.JsonSerializer.Deserialize<LangJson>(File.ReadAllText(langJsonFileFullName));
            }
        }

        public ReadOnlyLanguageRepository() { }

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
