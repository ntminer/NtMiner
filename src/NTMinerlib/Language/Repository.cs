using NTMiner.Language.Impl;
using NTMiner.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTMiner.Language {
    public static class Repository {
        public static IRepository<T> CreateLanguageRepository<T>() where T : class, IDbEntity<Guid> {
            if (DevMode.IsDevMode) {
                return new CommonRepository<T>("");
            }
            else {
                return new ReadOnlyLanguageRepository<T>();
            }
        }
    }
}
