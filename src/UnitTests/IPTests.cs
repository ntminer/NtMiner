using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using NTMiner.Core.MinerClient;
using System;
using System.Net;
using System.Net.Sockets;

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
        public void IpAddressTest() {
            var ip = IPAddress.Parse("[::]");
            Assert.AreEqual(AddressFamily.InterNetworkV6, ip.AddressFamily);
            Assert.AreEqual("::", ip.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void IpAddressTest1() {
            IPAddress.Parse("localhost");
        }

        [TestMethod]
        public void IpAddressTest2() {
            Assert.AreEqual(IPAddress.Any, IPAddress.Parse("0.0.0.0"));
            Assert.AreEqual("0.0.0.0", IPAddress.Any.ToString());
        }
    }
}
