using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrimS.Telnet;
using System.Threading;

namespace UnitTests {
    [TestClass]
    public class TelnetTests {
        [TestMethod]
        public void ReadmeExample() {
            using (Client client = new Client("127.0.0.1", 3337, new CancellationToken())) {
                Assert.IsTrue(client.IsConnected, "该测试需要挖矿端守护进程已运行");
            }
        }
    }
}
