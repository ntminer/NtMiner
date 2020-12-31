using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.Cpus.Impl;
using NTMiner.Windows;
using System;

namespace NTMiner {
    [TestClass]
    public class CpuTests {
        [TestMethod]
        public void CpuTest1() {
            for (int i = 0; i < 100; i++) {
                Console.WriteLine($"温度：{CpuPackage.GetTemperature().ToString("f1")} ℃");
                Console.WriteLine($"PerformanceCounter CpuUsage {Cpu.Instance.GetTotalCpuUsage().ToString("f1")} %");
                System.Threading.Thread.Sleep(10);
            }
        }

        [TestMethod]
        public void CpuTest2() {
            for (int i = 0; i < 10000; i++) {
                Cpu.Instance.GetTotalCpuUsage();
            }
        }

        [TestMethod]
        public void Test3() {
            // 第一次请求约需要160毫秒
            CpuPackage.GetTemperature();
        }

        [TestMethod]
        public void Test4() {
            for (int i = 0; i < 100; i++) {
                // 每次需要100毫秒
                CpuPackage.GetTemperature();
            }
        }
    }
}
