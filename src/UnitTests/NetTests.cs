using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Sockets;

namespace NTMiner {
    [TestClass]
    public class NetTests {
        [TestMethod]
        public void ReadmeExample() {
            using (TcpClient tcpClient = new TcpClient("127.0.0.1", NTKeyword.NTMinerDaemonPort)) {
                Assert.IsTrue(tcpClient.Connected, "该测试需要挖矿端守护进程已运行");
            }
        }
    }
}
