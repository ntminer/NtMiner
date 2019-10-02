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
            for (int i = 0; i < 100; i++) {
                Console.WriteLine("电脑CPU使用率：" + NTMiner.Windows.Cpu.Instance.GetPerformance().ToString("f1") + " %");
                System.Threading.Thread.Sleep(10);
            }
        }

        [TestMethod]
        public void CpuTest2() {
            var cpus = Cpu.Discover();

            foreach (var cpu in cpus) {
                Print(cpu.CoreTemperatures);
                Console.WriteLine(cpu.PackageTemperature.ToString());
            }
        }
        private static void Print(Sensor[] sensors) {
            if (sensors.Any()) {
                Console.WriteLine(string.Join(" ", sensors.Select(x => x.ToString())));
            }
        }
    }
}
