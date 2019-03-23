using NTMiner.Language.Impl;
using NTMiner.Repositories;
using System;
using System.IO;

namespace NTMiner.Language {
    public static class Repository {
        public static IRepository<T> CreateLanguageRepository<T>() where T : class, IDbEntity<Guid> {
            if (DevMode.IsDebugMode) {
                return new CommonRepository<T>(Path.Combine(VirtualRoot.GlobalDirFullName, "lang.litedb"));
            }
            else {
                return new ReadOnlyLanguageRepository<T>();
            }
        }
    }
}
