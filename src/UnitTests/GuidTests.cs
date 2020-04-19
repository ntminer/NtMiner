using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NTMiner {
    [TestClass]
    public class GuidTests {
        [TestMethod]
        public void GetHashCodeTest() {
            Guid guid = Guid.Parse("0A3BF32D-9D2D-4A39-9C46-75D6AC73065C");
            int hashCode = guid.GetHashCode();
            if (hashCode < 0) {
                hashCode = -hashCode;
            }
            Assert.AreEqual(501827256, hashCode);
        }
    }
}
