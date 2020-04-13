using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests {
    [TestClass]
    public class RamTests {
        [TestMethod]
        public void RamTest() {
            NTMiner.Windows.Ram ram = NTMiner.Windows.Ram.Instance;
            Console.WriteLine(ram.TotalPhysicalMemory);
            Console.WriteLine(ram.TotalPhysicalMemory / (1024 * 1024));
            Console.WriteLine(ram.AvailablePhysicalMemory);
        }
    }
}
