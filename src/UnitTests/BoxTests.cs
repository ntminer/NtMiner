using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.Core;
using System;
using System.Globalization;

namespace NTMiner {
    [TestClass]
    public class BoxTests {
        [TestMethod]
        public void BoxTest() {
            // 装箱了
            Console.WriteLine(string.Format("{0}{1}{2}", 1, false, LocalMessageType.Info));
            // 装箱了，和上面完全一样
            Console.WriteLine($"{1}{false}{LocalMessageType.Info}");
            // 装箱了
            string.Format(CultureInfo.InvariantCulture, "0x{0:X8}", 1);
            // 装箱了
            Console.WriteLine($"{Guid.NewGuid()}");
        }
    }
}
