using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;

namespace NTMiner {
    [TestClass]
    public class PerformanceCounterTests {
        [TestMethod]
        public void BenchmarkTest() {
            var process = Process.GetCurrentProcess();
            NTStopwatch.Start();
            // 这个太慢
            PerformanceCounterCategory cat = new PerformanceCounterCategory("Process");
            // 同一个程序如果运行多次，对应的多个进程名称是通过后缀#1、#2、#3...区分的
            string[] instanceNames = cat.GetInstanceNames().Where(a => a.StartsWith(process.ProcessName)).ToArray();
            var elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);

            NTStopwatch.Start();
            for (int i = 0; i < 100; i++) {
                cat = new PerformanceCounterCategory("Process");
                // 同一个程序如果运行多次，对应的多个进程名称是通过后缀#1、#2、#3...区分的
                instanceNames = cat.GetInstanceNames().Where(a => a.StartsWith(process.ProcessName)).ToArray();
            }
            elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);
        }

        [TestMethod]
        public void BenchmarkTest1() {
            var process = Process.GetCurrentProcess();
            Console.WriteLine(process.ProcessName);
            NTStopwatch.Start();
            // 这个很快
            Process[] processes = Process.GetProcessesByName("devenv");
            Assert.IsTrue(processes.Length != 0);
            var elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);

            NTStopwatch.Start();
            for (int i = 0; i < 100; i++) {
                processes = Process.GetProcessesByName("devenv");
            }
            elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);
        }
    }
}
