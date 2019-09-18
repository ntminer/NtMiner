using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
            foreach (IPAddress item in NTMiner.VirtualRoot.GetLocalIps()) {
                Console.WriteLine(item.ToString());
            }
        }
    }
}
