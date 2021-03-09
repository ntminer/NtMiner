using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NTMiner {
    [TestClass]
    public class VersionTests {
        [TestMethod]
        public void VersionParseTest() {
            // 注意09变成了9
            Assert.AreEqual(new Version(457, 9), Version.Parse("457.09"));
        }
    }
}
