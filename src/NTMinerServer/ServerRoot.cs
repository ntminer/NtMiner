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
        internal static void SetClientTestId(IClientTestId clientTestId) {
            _clientTestId = clientTestId;
        }
        public static readonly IRemoteIpSet _remoteIpSet = new RemoteIpSet();
        public static IRemoteIpSet RemoteIpSet {
            get {
                return _remoteIpSet;
            }
        }

        static ServerRoot() {
            System.Net.ServicePointManager.DefaultConnectionLimit = 512;
        }

        #region 曳光弹
        private static IClientTestId _clientTestId;
        public static IClientTestId ClientTestId {
            get { return _clientTestId; }
        }

        // [yè guāng dàn]
        // 曳光弹是一种装有能发光的化学药剂的炮弹或枪弹。发射后发出红色﹑黄色或者绿色的光。用来指示弹道和目标。
        // 曳光弹跟其他子弹弹头不同的是弹头在飞行中会发亮，并在光源不足或黑暗环境显示出弹道，协助射手进行弹道修正。
        public static void IfMinerClientTestIdLogElseNothing(Guid clientId, string msg) {
            if (_clientTestId != null && _clientTestId.MinerClientTestId != Guid.Empty && _clientTestId.MinerClientTestId == clientId) {
                Logger.Debug($"{nameof(NTMinerAppType.MinerClient)} {clientId.ToString()} {msg}");
            }
        }

        public static void IfStudioClientTestIdLogElseNothing(Guid studioId, string msg) {
            if (_clientTestId != null && _clientTestId.StudioClientTestId != Guid.Empty && _clientTestId.StudioClientTestId == studioId) {
                Logger.Debug($"{nameof(NTMinerAppType.MinerStudio)} {studioId.ToString()} {msg}");
            }
        }
        #endregion

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
