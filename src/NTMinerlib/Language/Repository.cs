using NTMiner.Language.Impl;
using NTMiner.Repositories;
using System;
using System.IO;

namespace NTMiner.Language {
    public static class Repository {
        public static IRepository<T> CreateLanguageRepository<T>() where T : class, IDbEntity<Guid> {
            if (DevMode.IsDevMode) {
                return new CommonRepository<T>(Path.Combine(Global.GlobalDirFullName, "lang.litedb"));
            }
            else {
                return new ReadOnlyLanguageRepository<T>();
            }
        }
    }
}
