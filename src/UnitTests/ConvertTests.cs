using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests {
    [TestClass]
    public class ConvertTests {
        [TestMethod]
        public void ConvertTest1() {
            double d = 1.11;
            Assert.AreEqual(1, Convert.ToInt32(d)); 
            d = 1.99;
            Assert.AreEqual(2, Convert.ToInt32(d));
        }
    }
}
