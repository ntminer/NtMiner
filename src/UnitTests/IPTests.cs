using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using NTMiner.MinerClient;
using System;
using System.Linq;

namespace UnitTests {
    [TestClass]
    public class IPTests {
        [TestMethod]
        public void GetLocalIpTest() {
            foreach (ILocalIp item in VirtualRoot.LocalIpSet.AsEnumerable()) {
                Console.WriteLine(item.ToString());
            }
        }

        [TestMethod]
        public void PrintNetCardInfo() {
            foreach (var item in NTMiner.Net.LocalIpSet.GetNetCardInfo()) {
                Console.WriteLine("-------------------start---------------------");
                foreach (var kv in item.Properties) {
                    Console.WriteLine($"{kv.Name}: {kv.Value}");
                }
                Console.WriteLine("--------------------end----------------------");
            }
        }

        [TestMethod]
        public void IPTableTest() {
            var ip = NTMiner.Net.ArpUtil.GetIpByMac("60:EE:5C:86:AD:FC");
            Assert.AreEqual("192.168.1.11", ip);
        }
    }
}
