using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.Windows;
using System;

namespace UnitTests {
    [TestClass]
    public class CpuTests {
        [TestMethod]
        public void CpuTest1() {
            for (int i = 0; i < 10; i++) {
                Console.WriteLine("电脑CPU使用率：" + Cpu.Instance.GetPerformance().ToString("f1") + " %");
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
