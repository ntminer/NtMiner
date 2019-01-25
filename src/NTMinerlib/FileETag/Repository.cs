using NTMiner.Repositories;
using System;
using System.IO;

namespace NTMiner.FileETag {
    public static class Repository {
        public static IRepository<T> CreateETagRepository<T>() where T : class, IDbEntity<Guid> {
            return new CommonRepository<T>(Path.Combine(ClientId.GlobalDirFullName, "local.litedb"));
        }
    }
}
