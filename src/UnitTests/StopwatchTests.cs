using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;

namespace NTMiner {
    [TestClass]
    public class StopwatchTests {
        [TestMethod]
        public void Test() {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Thread.Sleep(1000);
            Console.WriteLine(watch.ElapsedMilliseconds);
            watch.Restart();
            Thread.Sleep(1000);
            Console.WriteLine(watch.ElapsedMilliseconds);
        }
    }
}
