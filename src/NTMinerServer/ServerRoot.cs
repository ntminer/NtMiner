using LiteDB;
using NTMiner.Core;
using NTMiner.Core.Impl;
using System.IO;

namespace NTMiner {
    public static class ServerRoot {
        private static IHostConfig _hostConfig;
        private static readonly object _locker = new object();
        public static IHostConfig HostConfig {
            get {
                if (_hostConfig == null) {
                    lock (_locker) {
                        if (_hostConfig == null) {
                            using (LiteDatabase db = new LiteDatabase($"filename={Path.Combine(HomePath.AppDomainBaseDirectory, NTKeyword.LocalDbFileName)}")) {
                                var col = db.GetCollection<HostConfigData>();
                                _hostConfig = col.FindOne(Query.All());
                            }
                            if (_hostConfig == null) {
                                throw new NTMinerException("未配置HostConfigData");
                            }
                        }
                    }
                }
                return _hostConfig;
            }
        }
    }
}
