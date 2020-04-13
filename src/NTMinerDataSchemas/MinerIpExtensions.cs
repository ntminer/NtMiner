using System;

namespace NTMiner {
    public static class MinerIpExtensions {
        public static bool TryGetFirstIp(string ipReportByClient, out string outIp) {
            /* 
             * LocalIp可能是空、1个或多个Ip，多个Ip以英文“,”号分隔，Ip末尾可能带有英文"()"小括号括住的"(动态)"或"(🔒)"
             * 例如：192.168.1.110(🔒)
             * 例如：192.168.1.110(🔒),10.1.1.119(动态)
             */
            outIp = ipReportByClient;
            if (string.IsNullOrEmpty(ipReportByClient)) {
                return false;
            }
            string[] parts = outIp.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 1) {
                outIp = parts[0];
            }
            int pos = outIp.IndexOf('(');
            if (pos != -1) {
                outIp = outIp.Substring(0, pos);
            }
            return true;
        }

        public static string GetLocalIp(this IMinerIp minerIp) {
            string ip = minerIp.LocalIp;
            if (string.IsNullOrEmpty(ip)) {
                // 向后兼容
                ip = minerIp.MinerIp;
            }
            if (TryGetFirstIp(ip, out string outIp)) {
                return outIp;
            }
            return string.Empty;
        }
    }
}
