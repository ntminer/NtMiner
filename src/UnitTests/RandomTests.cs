using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using System;

namespace UnitTests {
    [TestClass]
    public class RandomTests {
        [TestMethod]
        public void BenchmarkTest() {
            int n = 100000;
            NTStopwatch.Start();
            for (int i = 0; i < n; i++) {
                VirtualRoot.GetRandom();
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
