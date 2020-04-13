using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using System;

namespace UnitTests {
    [TestClass]
    public class FuncTests {
        [TestMethod]
        public void FuncTest() {
            int n = 10000;
            NTStopwatch.Start();
            for (int i = 0; i < n; i++) {
                string text = $"{i.ToString()} {DateTime.Now.ToString()} this is a test";
            }
            var elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine("text " + elapsedMilliseconds);
            NTStopwatch.Start();
            for (int i = 0; i < n; i++) {
                Func<string> fun = ()=> $"{i.ToString()} {DateTime.Now.ToString()} this is a test";
            }
            elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine("fun " + elapsedMilliseconds);
        }
    }
}
