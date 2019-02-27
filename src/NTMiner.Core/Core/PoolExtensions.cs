using System;
using System.Collections.Generic;
using System.Net;

namespace NTMiner.Core {
    public static class PoolExtensions {
        public static string GetHost(this IPool pool) {
            if (pool == null) {
                return string.Empty;
            }
            string poolServer = pool.Server;
            if (string.IsNullOrEmpty(poolServer)) {
                return string.Empty;
            }
            int index = poolServer.IndexOf(':');
            if (index > 0) {
                return poolServer.Substring(0, index);
            }
            return poolServer;
        }

        private static readonly Dictionary<IPool, string> s_ipDic = new Dictionary<IPool, string>();
        public static string GetIp(this IPool pool) {
            if (string.IsNullOrEmpty(pool.Server)) {
                return string.Empty;
            }
            if (s_ipDic.ContainsKey(pool)) {
                return s_ipDic[pool];
            }
            try {
                string hostNameOrAddress = GetHost(pool);
                IPAddress[] ips = Dns.GetHostAddresses(hostNameOrAddress);
                string ip;
                if (ips.Length > 0) {
                    ip = ips[0].ToString();
                }
                else {
                    ip = string.Empty;
                }
                s_ipDic.Add(pool, ip);
                return ip;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine($"获取矿池ip地址失败：{pool.Server}", e);
                return string.Empty;
            }
        }

        public static int GetPort(this IPool pool) {
            if (pool == null) {
                return 0;
            }
            if (string.IsNullOrEmpty(pool.Server)) {
                return 0;
            }
            int index = pool.Server.IndexOf(':');
            if (index > 0) {
                int port;
                int.TryParse(pool.Server.Substring(index + 1), out port);
                return port;
            }
            return 0;
        }
    }
}
