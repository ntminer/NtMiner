using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NTMiner {
    [TestClass]
    public class RamTests {
        [TestMethod]
        public void RamTest() {
            NTMiner.Windows.Ram ram = Windows.Ram.Instance;
            Console.WriteLine(ram.TotalPhysicalMemory);
            Console.WriteLine(ram.TotalPhysicalMemory / (1024 * 1024) + "Mb");
            Console.WriteLine(ram.AvailablePhysicalMemory);
            Console.WriteLine(ram.AvailableVirtual/ (1024 * 1024) + "Mb");
        }
    }
}
