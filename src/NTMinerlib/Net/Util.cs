using System;
using System.Net;
using System.Net.NetworkInformation;

namespace NTMiner.Net {
    public static class Util {
        public static bool Ping(string hostOrIp) {
            try {
                using (Ping p1 = new Ping()) {
                    PingReply reply = p1.Send(hostOrIp);
                    if (reply == null) {
                        return false;
                    }
                    return reply.Status == IPStatus.Success;
                }
            }
            catch {
                return false;
            }
        }

        private static readonly long aBegin = ConvertToIpNum("10.0.0.0");
        private static readonly long aEnd = ConvertToIpNum("10.255.255.255");
        private static readonly long bBegin = ConvertToIpNum("172.16.0.0");
        private static readonly long bEnd = ConvertToIpNum("172.31.255.255");
        private static readonly long cBegin = ConvertToIpNum("192.168.0.0");
        private static readonly long cEnd = ConvertToIpNum("192.168.255.255");
        /// <summary>
        /// 判断IP地址是否为内网IP地址
        /// </summary>
        /// <param name="ipAddress">IP地址字符串</param>
        /// <returns></returns>
        public static bool IsInnerIp(string ipAddress) {
            if (string.IsNullOrEmpty(ipAddress)) {
                return false;
            }
            if (ipAddress == "localhost" || ipAddress == "127.0.0.1") {
                return true;
            }
            IPAddress address;
            if (!IPAddress.TryParse(ipAddress, out address)) {
                return false;
            }
            try {
                bool isInnerIp = false;
                long ipNum = ConvertToIpNum(ipAddress);
                /**
                私有IP：A类 10.0.0.0-10.255.255.255
                B类 172.16.0.0-172.31.255.255
                C类 192.168.0.0-192.168.255.255
                当然，还有127这个网段是环回地址 
                **/
                isInnerIp = IsInner(ipNum, aBegin, aEnd) || IsInner(ipNum, bBegin, bEnd) || IsInner(ipNum, cBegin, cEnd);
                return isInnerIp;
            }
            catch {
                return false;
            }
        }

        public static bool IsLocalHost(string ipAddress) {
            if (string.IsNullOrEmpty(ipAddress)) {
                return false;
            }
            if (ipAddress == "localhost" || ipAddress == "127.0.0.1") {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 把IP地址转换为long整型。
        /// </summary>
        /// <param name="ipAddress">IP地址字符串</param>
        /// <returns></returns>
        public static long ConvertToIpNum(string ipAddress) {
            if (string.IsNullOrEmpty(ipAddress)) {
                throw new ArgumentNullException(nameof(ipAddress));
            }
            string[] parts = ipAddress.Split('.');
            if (parts.Length != 4) {
                throw new FormatException("ipAddress格式不正确:" + ipAddress);
            }
            int.TryParse(parts[0], out int a);
            int.TryParse(parts[1], out int b);
            int.TryParse(parts[2], out int c);
            int.TryParse(parts[3], out int d);

            return (long)a * 256 * 256 * 256 + (long)b * 256 * 256 + (long)c * 256 + d;
        }

        /// <summary>
        /// 将long转化为IP地址。
        /// </summary>
        /// <param name="ipValue"></param>
        /// <returns></returns>
        public static string ConvertToIpString(long ipValue) {
            string hexStr = ipValue.ToString("X8");
            int ip1 = Convert.ToInt32(hexStr.Substring(0, 2), 16);
            int ip2 = Convert.ToInt32(hexStr.Substring(2, 2), 16);
            int ip3 = Convert.ToInt32(hexStr.Substring(4, 2), 16);
            int ip4 = Convert.ToInt32(hexStr.Substring(6, 2), 16);
            if (BitConverter.IsLittleEndian) {
                return ip1 + "." + ip2 + "." + ip3 + "." + ip4;
            }
            return ip4 + "." + ip3 + "." + ip2 + "." + ip1;
        }

        /// <summary>
        /// 判断用户IP地址转换为Long型后是否在内网IP地址所在范围
        /// </summary>
        /// <param name="userIp"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static bool IsInner(long userIp, long begin, long end) {
            return (userIp >= begin) && (userIp <= end);
        }
    }
}
