using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.RemoteDesktop;

namespace UnitTests {
    [TestClass]
    public class RemoteDesktopTests {
        [TestMethod]
        public void TestMethod1() {
            Rdp.SetRdpEnabled(true);
            Assert.IsTrue(Rdp.GetRdpEnabled());
            Rdp.SetRdpEnabled(false);
            Assert.IsFalse(Rdp.GetRdpEnabled());
        }
    }
}
