using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace NTMiner {
    [TestClass]
    public class NTStopwatchTests {
        [TestMethod]
        public void Test1() {
            Task task1 = Task.Factory.StartNew(() => {
                NTStopwatch.Start();
                System.Threading.Thread.Sleep(1000);
                var elapsedMilliseconds = NTStopwatch.Stop();
                Console.WriteLine(elapsedMilliseconds);
            });
            Task task2 = Task.Factory.StartNew(() => {
                NTStopwatch.Start();
                System.Threading.Thread.Sleep(1000);
                var elapsedMilliseconds = NTStopwatch.Stop();
                Console.WriteLine(elapsedMilliseconds);
            });
            Task task3 = Task.Factory.StartNew(() => {
                NTStopwatch.Start();
                System.Threading.Thread.Sleep(1000);
                var elapsedMilliseconds = NTStopwatch.Stop();
                Console.WriteLine("task3 1 " + elapsedMilliseconds);
                NTStopwatch.Start();
                System.Threading.Thread.Sleep(1000);
                elapsedMilliseconds = NTStopwatch.Stop();
                Console.WriteLine("task3 2 " + elapsedMilliseconds);
            });
            Task.WaitAll(task1, task2, task3);
        }
    }
}
