using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Cryptography;

namespace NTMiner {
    [TestClass]
    public class RandomTests {
        [TestMethod]
        public void BenchmarkTest() {
            int n = 100000;
            NTStopwatch.Start();
            for (int i = 0; i < n; i++) {
                byte[] rndBytes = new byte[4];
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(rndBytes);
                new Random(BitConverter.ToInt32(rndBytes, 0));
            }
            var elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);
            NTStopwatch.Start();
            for (int i = 0; i < n; i++) {
                VirtualRoot.GetRandomString(4);
            }
            elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);
        }
    }
}
