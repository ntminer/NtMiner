using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NTMiner {
    [TestClass]
    public class SecureStringTests {
        [TestMethod]
        public void Test() {
            string password = Guid.NewGuid().ToString();
            RpcUser rpcUser = new RpcUser("test", password);
            Assert.AreEqual(password, rpcUser.Password);
            for (int i = 0; i < 10000; i++) {
                _ = rpcUser.Password;
            }
            rpcUser.Logout();
        }
    }
}
