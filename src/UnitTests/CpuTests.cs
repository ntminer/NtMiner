using HardwareProviders;
using HardwareProviders.CPU;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace UnitTests {
    [TestClass]
    public class CpuTests {
        [TestMethod]
        public void CpuTest1() {
            for (int i = 0; i < 10; i++) {
                Console.WriteLine("电脑CPU使用率：" + NTMiner.Windows.Cpu.Instance.GetPerformance().ToString("f1") + " %");
                System.Threading.Thread.Sleep(1000);
            }
        }

        [TestMethod]
        public void CpuTest2() {
            var cpu = Cpu.Discover();

            foreach (var item in cpu) {
                Print(item.CoreTemperatures);
                Print(item.CoreClocks);
                Print(item.CorePowers);
                Print(item.CoreVoltages);
                Print(item.CoreClocks);
            }
        }
        private static void Print(Sensor[] sensors) {
            if (sensors.Any()) {
                Console.WriteLine(string.Join(" ", sensors.Select(x => x.ToString())));
            }
        }
    }
}
