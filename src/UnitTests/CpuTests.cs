using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.Windows;
using System;

namespace NTMiner {
    [TestClass]
    public class CpuTests {
        [TestMethod]
        public void CpuTest1() {
            for (int i = 0; i < 100; i++) {
                Console.WriteLine($"温度：{Cpu.Instance.GetTemperature().ToString("f1")} ℃");
                Console.WriteLine($"PerformanceCounter CpuUsage {Cpu.Instance.GetCurrentCpuUsage().ToString("f1")} %");
                System.Threading.Thread.Sleep(10);
            }
        }

        [TestMethod]
        public void CpuTest2() {
            for (int i = 0; i < 10000; i++) {
                Cpu.Instance.GetCurrentCpuUsage();
            }
        }

        [TestMethod]
        public void Test3() {
            for (int i = 0; i < 100; i++) {
                Cpu.Instance.GetTemperature();
            }
        }
    }
}
