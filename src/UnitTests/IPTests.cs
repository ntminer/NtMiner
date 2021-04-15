using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.Core.MinerClient;
using System;
using System.Net;
using System.Net.Sockets;

namespace NTMiner {
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
            foreach (var item in VirtualRoot.LocalIpSetImpl.GetNetCardInfo()) {
                Console.WriteLine("-------------------start---------------------");
                foreach (var kv in item.Properties) {
                    Console.WriteLine($"{kv.Name}: {kv.Value}");
                }
                Console.WriteLine("--------------------end----------------------");
            }
        }

        [TestMethod]
        public void IpAddressTest() {
            var ip = IPAddress.Parse("[::]");
            Assert.AreEqual(AddressFamily.InterNetworkV6, ip.AddressFamily);
            Assert.AreEqual("::", ip.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void IpAddressTest1() {
            IPAddress.Parse(NTKeyword.Localhost);
        }

        [TestMethod]
        public void IpAddressTest2() {
            Assert.AreEqual(IPAddress.Any, IPAddress.Parse("0.0.0.0"));
            Assert.AreEqual("0.0.0.0", IPAddress.Any.ToString());
        }

        [TestMethod]
        public void IPAddressHashCodeTest() {
            IPAddress iPAddress1 = IPAddress.Parse("111.222.222.111");
            IPAddress iPAddress2 = IPAddress.Parse("111.222.222.111");
            Console.WriteLine(iPAddress1.ToString());
            Assert.AreEqual(iPAddress1.GetHashCode(), iPAddress2.GetHashCode());
        }
    }
}
