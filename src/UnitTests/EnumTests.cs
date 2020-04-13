using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using System;

namespace UnitTests {
    [TestClass]
    public class EnumTests {
        [TestMethod]
        public void ToStringTest() {
            Assert.AreEqual(nameof(NTMinerAppType.MinerClient), NTMinerAppType.MinerClient.ToString());
            Assert.AreEqual(nameof(NTMinerAppType.MinerClient), NTMinerAppType.MinerClient.GetName());
        }

        [TestMethod]
        public void BenchmarkTest() {
            int n = 100000;
            NTStopwatch.Start();
            for (int i = 0; i < n; i++) {
                NTMinerAppType.MinerClient.ToString();
            }
            var elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);
            NTStopwatch.Start();
            for (int i = 0; i < n; i++) {
                NTMinerAppType.MinerClient.GetName();
            }
            elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);
        }
    }
}
