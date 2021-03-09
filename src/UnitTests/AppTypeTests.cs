using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NTMiner {
    [TestClass]
    public class AppTypeTests {
        [TestMethod]
        public void IsMinerClientTest() {
            Assert.IsTrue(typeof(Daemon.DaemonUtil).Assembly.GetManifestResourceInfo(NTKeyword.NTMinerDaemonKey) != null);
        }

        [TestMethod]
        public void IsMinerStudioTest() {
            var assembly = typeof(MsRemoteDesktop).Assembly;
            Type type = assembly.GetType(typeof(MsRemoteDesktop).FullName);
            Assert.IsNotNull(type);
            type = assembly.GetType("NTMiner.aaaaa");
            Assert.IsNull(type);
        }
    }
}
