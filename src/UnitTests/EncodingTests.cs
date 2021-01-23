using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace NTMiner {
    [TestClass]
    public class EncodingTests {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test1() {
            _ = Encoding.UTF8.GetString(null);
        }

        [TestMethod]
        public void Test2() {
            string s = Encoding.UTF8.GetString(new byte[0]);
            Assert.AreEqual(string.Empty, s);
        }
    }
}
