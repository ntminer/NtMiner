using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;

namespace UnitTests {
    [TestClass]
    public class IPTests {
        [TestMethod]
        public void GetAllDevicesOnLANTest() {
            foreach (IPAddress ip in NTMiner.Ip.Impl.IpSet.Instance) {
                Console.WriteLine("IP : {0}", ip);
            }
        }
    }
}
