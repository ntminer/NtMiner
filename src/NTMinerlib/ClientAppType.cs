using System;
using System.Reflection;

namespace NTMiner {
    // 单独一个类型从而防止类型初始化异常
    public static class ClientAppType {
        // IsMinerClient和IsMinerStudio具有排它性但不具有完备性，但该NTMinerClient类库只被挖矿客户端和群控客户端使用所以也算是完备性的。
        #region IsMinerClient
        private static bool _isMinerClient;
        private static bool _isMinerClientDetected = false;
        private static readonly object _locker = new object();
        /// <summary>
        /// 表示是否是挖矿端。true表示是挖矿端，否则不是。
        /// </summary>
        public static bool IsMinerClient {
            get {
                if (_isMinerClientDetected) {
                    return _isMinerClient;
                }
                if (_isMinerStudioDetected && IsMinerStudio) {
                    _isMinerClientDetected = true;
                    return false;
                }
                lock (_locker) {
                    if (_isMinerClientDetected) {
                        return _isMinerClient;
                    }
                    if (DevMode.IsInUnitTest) {
                        _isMinerClient = true;
                    }
                    else {
                        var assembly = Assembly.GetEntryAssembly();
                        // 基于约定，根据主程序集中是否有给定名称的资源文件判断是否是挖矿客户端
                        _isMinerClient = assembly.GetManifestResourceInfo(NTKeyword.NTMinerDaemonKey) != null;
                    }
                    _isMinerClientDetected = true;
                }
                return _isMinerClient;
            }
        }
        #endregion

        #region IsMinerStudio
        private static bool _isMinerStudio;
        private static bool _isMinerStudioDetected = false;
        /// <summary>
        /// 表示是否是群控客户端。true表示是群控客户端，否则不是。
        /// </summary>
        public static bool IsMinerStudio {
            get {
                if (_isMinerStudioDetected) {
                    return _isMinerStudio;
                }
                if (_isMinerClientDetected && IsMinerClient) {
                    _isMinerStudioDetected = true;
                    return false;
                }
                lock (_locker) {
                    if (_isMinerStudioDetected) {
                        return _isMinerStudio;
                    }
                    if (Environment.CommandLine.IndexOf(NTKeyword.MinerStudioCmdParameterName, StringComparison.OrdinalIgnoreCase) != -1) {
                        _isMinerStudio = true;
                    }
                    else {
                        // 基于约定，根据主程序集中是否有给定名称的资源文件判断是否是群控客户端
                        if (DevMode.IsInUnitTest) {
                            return false;
                        }
                        var assembly = Assembly.GetEntryAssembly();
                        var type = assembly.GetType("NTMiner.MsRdpRemoteDesktop");
                        _isMinerStudio = type != null;
                    }
                    _isMinerStudioDetected = true;
                }
                return _isMinerStudio;
            }
        }
        #endregion

        public static readonly NTMinerAppType AppType = IsMinerClient ? NTMinerAppType.MinerClient : NTMinerAppType.MinerStudio;
    }
}
