using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.Windows;
using System;

namespace UnitTests {
    [TestClass]
    public class CpuTests {
        [TestMethod]
        public void CpuTest1() {
            for (int i = 0; i < 100; i++) {
                Console.WriteLine("CPU使用率：" + Cpu.Instance.GetPerformance().ToString("f1") + " %");
                System.Threading.Thread.Sleep(10);
            }
        }

        [TestMethod]
        public void CpuTest2() {
            for (int i = 0; i < 100; i++) {
                Console.WriteLine("CPU温度：" + Cpu.Instance.GetTemperature().ToString("f1") + " ℃");
            }
        }
    }
}
