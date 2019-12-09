using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.Windows;
using System;

namespace UnitTests {
    [TestClass]
    public class CpuTests {
        [TestMethod]
        public void CpuTest1() {
            for (int i = 0; i < 100; i++) {
                Cpu.Instance.GetSensorValue(out double performance, out float temperature);
                Console.WriteLine($"CPU使用率：{performance.ToString("f1")} % CPU温度：{temperature.ToString("f1")} ℃");
                System.Threading.Thread.Sleep(10);
            }
        }
    }
}
