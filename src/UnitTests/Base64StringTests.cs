using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.Cryptography;
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

        [TestMethod]
        public void Test2() {
            for (int i = 0; i < 100; i++) {
                var key = RSAUtil.GetRASKey();
                Assert.IsTrue(Base64Util.IsBase64OrEmpty(key.PublicKey));
                Assert.IsTrue(Base64Util.IsBase64OrEmpty(key.PrivateKey));
            }
        }

        [TestMethod]
        public void Test3() {
            string str = Encoding.UTF8.GetString(Convert.FromBase64String("BQ=="));
            // 很奇怪的一个符号，不是键盘上敲出的|
            Assert.AreEqual("", str);
        }
    }
}
