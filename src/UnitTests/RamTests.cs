using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace NTMiner {
    [TestClass]
    public class RamTests {
        [TestMethod]
        public void RamTest() {
            Windows.Ram ram = Windows.Ram.Instance;
            Console.WriteLine(ram.TotalPhysicalMemory);
            Console.WriteLine(ram.TotalPhysicalMemory / NTKeyword.IntM + "Mb");
            Console.WriteLine(ram.AvailablePhysicalMemory);
            var process = Process.GetCurrentProcess();
            Console.WriteLine((process.WorkingSet64 - process.PrivateMemorySize64) / NTKeyword.DoubleM);
            Console.WriteLine(VirtualRoot.ProcessMemoryMb);
        }

        [TestMethod]
        public void BenchmarkTest1() {
            NTStopwatch.Start();

            for (int i = 0; i < 500; i++) {
                _ = VirtualRoot.ProcessMemoryMb;
                _ = VirtualRoot.ThreadCount;
                _ = VirtualRoot.HandleCount;
            }

            var elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);
        }

        [TestMethod]
        public void Test2() {
            Console.WriteLine(VirtualRoot.ProcessMemoryMb);
            Console.WriteLine(VirtualRoot.ThreadCount);
            Console.WriteLine(VirtualRoot.HandleCount);
        }
    }
}
