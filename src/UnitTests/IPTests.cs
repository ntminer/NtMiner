using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using NTMiner.MinerClient;
using System;

namespace UnitTests {
    [TestClass]
    public class IPTests {
        [TestMethod]
        public void GetLocalIpTest() {
            foreach (ILocalIp item in VirtualRoot.LocalIpSet.AsEnumerable()) {
                Console.WriteLine(item.ToString());
            }
        }
    }
}
