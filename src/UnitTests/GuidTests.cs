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
            string s = guid.ToString("N");
            Assert.AreEqual(s, "0a3bf32d9d2d4a399c4675d6ac73065c");
            guid = Guid.Parse(s); 
            hashCode = guid.GetHashCode();
            if (hashCode < 0) {
                hashCode = -hashCode;
            }
            Assert.AreEqual(501827256, hashCode);
        }

        [TestMethod]
        public void ToStringTest() {
            Console.WriteLine(Guid.NewGuid().ToString("N"));
            Console.WriteLine(Guid.NewGuid().ToString("D"));
            Console.WriteLine(Guid.NewGuid().ToString("B"));
            Console.WriteLine(Guid.NewGuid().ToString("P"));
            Console.WriteLine(Guid.NewGuid().ToString("X"));
        }
    }
}
