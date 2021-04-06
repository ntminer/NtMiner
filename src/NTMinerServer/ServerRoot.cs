using LiteDB;
using NTMiner.Core;
using NTMiner.Core.Impl;
using NTMiner.IpSet;
using NTMiner.IpSet.Impl;
using NTMiner.ServerNode;
using System;
using System.IO;

namespace NTMiner {
    public static class ServerRoot {
        private static IClientTestId _clientTestId;
        public static IClientTestId ClientTestId {
            get { return _clientTestId; }
        }
        internal static void SetClientTestId(IClientTestId clientTestId) {
            _clientTestId = clientTestId;
        }
        public static readonly IRemoteIpSet _remoteEndPointSet = new RemoteIpSet();
        public static IRemoteIpSet RemoteEndPointSet {
            get {
                return _remoteEndPointSet;
            }
        }

        static ServerRoot() {
            System.Net.ServicePointManager.DefaultConnectionLimit = 512;
        }

        public static bool IsMinerClientTestId(Guid clientId) {
            return _clientTestId != null && _clientTestId.MinerClientTestId != Guid.Empty && _clientTestId.MinerClientTestId == clientId;
        }

        public static bool IsStudioClientTestId(Guid clientId) {
            return _clientTestId != null && _clientTestId.StudioClientTestId != Guid.Empty && _clientTestId.StudioClientTestId == clientId;
        }

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
