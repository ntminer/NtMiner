using System;
using System.Collections.Generic;
using System.Net;

namespace NTMiner.Net {
    public static class IpUtil {
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
            if (IsLocalhost(ipAddress)) {
                return true;
            }

            if (string.IsNullOrEmpty(ipAddress)) {
                return false;
            }

            if (!IPAddress.TryParse(ipAddress, out _)) {
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

        public static bool IsLocalhost(string ipAddress) {
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
        public static uint ConvertToIpNum(string ipAddress) {
            if (string.IsNullOrEmpty(ipAddress)) {
                throw new ArgumentNullException(nameof(ipAddress));
            }
            if (ipAddress == "localhost") {
                ipAddress = "127.0.0.1";
            }
            string[] parts = ipAddress.Split('.');
            if (parts.Length != 4) {
                throw new FormatException("ipAddress格式不正确:" + ipAddress);
            }
            uint.TryParse(parts[0], out uint a);
            if (a > 255) {
                a = 0;
            }
            uint.TryParse(parts[1], out uint b);
            if (b > 255) {
                b = 0;
            }
            uint.TryParse(parts[2], out uint c);
            if (c > 255) {
                c = 0;
            }
            uint.TryParse(parts[3], out uint d);
            if (d < 0 || d > 255) {
                d = 0;
            }

            return a * 256 * 256 * 256 + b * 256 * 256 + c * 256 + d;
        }

        /// <summary>
        /// 批量生成IP，包含头尾
        /// </summary>
        /// <param name="fromIp">包含头</param>
        /// <param name="toIp">包含尾</param>
        /// <returns></returns>
        public static List<string> CreateIpRange(string fromIp, string toIp) {
            List<string> list = new List<string>();
            for (uint i = ConvertToIpNum(fromIp); i <= ConvertToIpNum(toIp); i++) {
                list.Add(ConvertToIpString(i));
            }
            return list;
        }

        /// <summary>
        /// 批量生成IP，包含头
        /// </summary>
        /// <param name="fromIp">包含头</param>
        /// <param name="count"></param>
        public static List<string> CreateIpRange(string fromIp, int count) {
            List<string> list = new List<string>();
            for (uint i = ConvertToIpNum(fromIp); i < count; i++) {
                list.Add(ConvertToIpString(i));
            }
            return list;
        }

        /// <summary>
        /// 将long转化为IP地址。
        /// </summary>
        /// <param name="ipValue"></param>
        /// <returns></returns>
        public static string ConvertToIpString(uint ipValue) {
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
