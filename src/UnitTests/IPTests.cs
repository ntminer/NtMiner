using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using NTMiner.Ip.Impl;
using NTMiner.MinerClient;
using System;
using System.Net;

namespace UnitTests {
    [TestClass]
    public class IPTests {
        [TestMethod]
        public void GetAllDevicesOnLANTest() {
            foreach (IPAddress ip in IpSet.Instance) {
                Console.WriteLine("IP : {0}", ip);
            }
        }

        [TestMethod]
        public void GetLocalIpTest() {
            foreach (ILocalIp item in VirtualRoot.LocalIpSet) {
                Console.WriteLine(item.ToString());
            }
        }
    }
}
