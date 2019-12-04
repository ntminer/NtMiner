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

        [TestMethod]
        public void EnableFirewallTest() {
            Firewall.EnableFirewall();
            FirewallStatus state = Firewall.Status(FirewallDomain.Domain);
            Assert.AreEqual(FirewallStatus.Enabled, state);
            state = Firewall.Status(FirewallDomain.Private);
            Assert.AreEqual(FirewallStatus.Enabled, state);
            state = Firewall.Status(FirewallDomain.Public);
            Assert.AreEqual(FirewallStatus.Enabled, state);
        }

        [TestMethod]
        public void DisableFirewallTest() {
            Firewall.DisableFirewall();
            FirewallStatus state = Firewall.Status(FirewallDomain.Domain);
            Assert.AreEqual(FirewallStatus.Disabled, state);
            state = Firewall.Status(FirewallDomain.Private);
            Assert.AreEqual(FirewallStatus.Disabled, state);
            state = Firewall.Status(FirewallDomain.Public);
            Assert.AreEqual(FirewallStatus.Disabled, state);
        }

        [TestMethod]
        public void RdpRuleTest() {
            Firewall.EnableFirewall();
            Firewall.AddRdpRule();
            Assert.IsTrue(Firewall.IsRdpRuleExists());
            Firewall.DisableFirewall();
            Assert.IsTrue(Firewall.IsRdpRuleExists());
            Firewall.RemoveRdpRule();
            Assert.IsFalse(Firewall.IsRdpRuleExists());
        }

        [TestMethod]
        public void MinerClientRuleTest() {
            Firewall.EnableFirewall();
            Firewall.AddMinerClientRule();
            Assert.IsTrue(Firewall.IsMinerClientRuleExists());
            Firewall.DisableFirewall();
            Assert.IsTrue(Firewall.IsMinerClientRuleExists());
            Firewall.RemoveMinerClientRule();
            Assert.IsFalse(Firewall.IsMinerClientRuleExists());
        }
    }
}
