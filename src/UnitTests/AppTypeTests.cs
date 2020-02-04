using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;

namespace UnitTests {
    [TestClass]
    public class AppTypeTests {
        [TestMethod]
        public void IsMinerClientTest() {
            Assert.IsTrue(typeof(NTMiner.Daemon.DaemonUtil).Assembly.GetManifestResourceInfo(NTKeyword.NTMinerDaemonKey) != null);
        }

        [TestMethod]
        public void IsMinerStudioTest() {
            Assert.IsTrue(typeof(NTMiner.NTMinerServices.NTMinerServicesUtil).Assembly.GetManifestResourceInfo(NTKeyword.NTMinerServicesKey) != null);
        }
    }
}
