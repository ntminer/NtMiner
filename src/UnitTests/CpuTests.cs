using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace UnitTests {
    [TestClass]
    public class CpuTests {
        [TestMethod]
        public void CpuTest1() {
            PerformanceCounter cpuCounter;

            cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            Console.WriteLine("电脑CPU使用率：" + cpuCounter.NextValue() + "%");
            Console.WriteLine();

            for (int i = 0; i < 10; i++) {
                Console.WriteLine("电脑CPU使用率：" + cpuCounter.NextValue().ToString("f1") + " %");
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
