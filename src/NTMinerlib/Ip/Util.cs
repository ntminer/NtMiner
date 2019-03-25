using System;

namespace NTMiner.Ip {
    public static class Util {
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
            try {
                bool isInnerIp = false;
                long ipNum = GetIpNum(ipAddress);
                /**
                私有IP：A类 10.0.0.0-10.255.255.255
                B类 172.16.0.0-172.31.255.255
                C类 192.168.0.0-192.168.255.255
                当然，还有127这个网段是环回地址 
                **/
                long aBegin = GetIpNum("10.0.0.0");
                long aEnd = GetIpNum("10.255.255.255");
                long bBegin = GetIpNum("172.16.0.0");
                long bEnd = GetIpNum("172.31.255.255");
                long cBegin = GetIpNum("192.168.0.0");
                long cEnd = GetIpNum("192.168.255.255");
                isInnerIp = IsInner(ipNum, aBegin, aEnd) || IsInner(ipNum, bBegin, bEnd) || IsInner(ipNum, cBegin, cEnd);
                return isInnerIp;
            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// 把IP地址转换为Long型数字
        /// </summary>
        /// <param name="ipAddress">IP地址字符串</param>
        /// <returns></returns>
        private static long GetIpNum(String ipAddress) {
            String[] ip = ipAddress.Split('.');
            long a = int.Parse(ip[0]);
            long b = int.Parse(ip[1]);
            long c = int.Parse(ip[2]);
            long d = int.Parse(ip[3]);

            long ipNum = a * 256 * 256 * 256 + b * 256 * 256 + c * 256 + d;
            return ipNum;
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
