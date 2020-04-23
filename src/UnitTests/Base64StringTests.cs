using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;

namespace NTMiner {
    [TestClass]
    public class Base64StringTests {
        [TestMethod]
        public void Test1() {
            for (int i = 0; i < 10000; i++) {
                var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
                Assert.IsTrue(Base64Util.IsBase64OrEmpty(base64String));
                Assert.IsTrue(base64String.All(a => Base64Util.IsBase64Char(a)));
            }
        }
    }
}
