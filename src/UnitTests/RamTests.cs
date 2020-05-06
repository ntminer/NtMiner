using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NTMiner {
    [TestClass]
    public class RamTests {
        [TestMethod]
        public void RamTest() {
            Windows.Ram ram = Windows.Ram.Instance;
            Console.WriteLine(ram.TotalPhysicalMemory);
            Console.WriteLine(ram.TotalPhysicalMemory / NTKeyword.IntM + "Mb");
            Console.WriteLine(ram.AvailablePhysicalMemory);
        }
    }
}
