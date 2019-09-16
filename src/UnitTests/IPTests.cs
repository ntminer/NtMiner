using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace UnitTests {
    [TestClass]
    public class IPTests {
        [TestMethod]
        public void GetAllDevicesOnLANTest() {
            foreach (IPAddress ip in NTMiner.Ip.Impl.IpSet.Instance) {
                Console.WriteLine("IP : {0}", ip);
            }
        }

        [TestMethod]
        public void GetLocalIpTest() {
            foreach (IPAddress item in GetLocalIps()) {
                Console.WriteLine(item.ToString());
            }
        }

        public static List<IPAddress> GetLocalIps() {
            List<IPAddress> ipaddress = new List<IPAddress>();

            //获取网卡
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces().Where(a=>a.Name.StartsWith("wlan", StringComparison.OrdinalIgnoreCase)).ToArray();
            foreach (NetworkInterface ni in interfaces) {
                var ippros = ni.GetIPProperties().UnicastAddresses;
                foreach (UnicastIPAddressInformation ip in ippros) {
                    //忽略不是ipv4的
                    if (ip.Address.AddressFamily != AddressFamily.InterNetwork) {
                        continue;
                    }
                    ipaddress.Add(ip.Address);
                }
            }
            return ipaddress;
        }
    }
}
