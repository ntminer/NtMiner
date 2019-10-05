using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UnitTests {
    [TestClass]
    public class CalcConfigUpdaterTests {
        [TestMethod]
        public void TestMethod1() {
            List<NTMiner.DataConfig> dataConfigs = NTMiner.Program.GetDataConfig().Result;
            foreach (var item in dataConfigs) {
                Console.WriteLine(item.ToString());
            }
        }
    }
}
