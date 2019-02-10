using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;

namespace UnitTestProject1 {
    [TestClass]
    public class IPTests {
        [TestMethod]
        public void GetAllDevicesOnLANTest() {
            foreach (IPAddress ip in NTMiner.VirtualRoot.IpSet) {
                Console.WriteLine("IP : {0}", ip);
            }
        }
    }
}
