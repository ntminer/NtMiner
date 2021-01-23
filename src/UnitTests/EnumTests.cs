using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NTMiner {
    [TestClass]
    public class EnumTests {
        [TestMethod]
        public void IsDefinedTest() {
            Assert.IsTrue(Enum.IsDefined(typeof(NTMinerAppType), NTMinerAppType.MinerClient.ToString()));
            Assert.IsTrue(Enum.IsDefined(typeof(NTMinerAppType), NTMinerAppType.MinerClient.GetName()));
            Assert.IsTrue(Enum.IsDefined(typeof(NTMinerAppType), 0));
            Assert.IsFalse(Enum.IsDefined(typeof(NTMinerAppType), 1000));
        }

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
