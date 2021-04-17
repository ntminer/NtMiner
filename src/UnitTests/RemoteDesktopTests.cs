using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.RemoteDesktop;

namespace NTMiner {
    [TestClass]
    public class RemoteDesktopTests {
        private static readonly object _locker = new object();
        [TestMethod]
        public void TestMethod1() {
            lock (_locker) {
                NTMinerRegistry.SetIsRdpEnabled(true);
                Assert.IsTrue(NTMinerRegistry.GetIsRdpEnabled());
                NTMinerRegistry.SetIsRdpEnabled(false);
                Assert.IsFalse(NTMinerRegistry.GetIsRdpEnabled());
            }
        }

        [TestMethod]
        public void EnableFirewallTest() {
            lock (_locker) {
                Firewall.EnableFirewall();
                FirewallStatus state = Firewall.Status(FirewallDomain.Domain);
                Assert.AreEqual(FirewallStatus.Enabled, state);
                state = Firewall.Status(FirewallDomain.Private);
                Assert.AreEqual(FirewallStatus.Enabled, state);
                state = Firewall.Status(FirewallDomain.Public);
                Assert.AreEqual(FirewallStatus.Enabled, state);
            }
        }

        [TestMethod]
        public void DisableFirewallTest() {
            lock (_locker) {
                Firewall.DisableFirewall();
                FirewallStatus state = Firewall.Status(FirewallDomain.Domain);
                Assert.AreEqual(FirewallStatus.Disabled, state);
                state = Firewall.Status(FirewallDomain.Private);
                Assert.AreEqual(FirewallStatus.Disabled, state);
                state = Firewall.Status(FirewallDomain.Public);
                Assert.AreEqual(FirewallStatus.Disabled, state);
            }
        }

        [TestMethod]
        public void RdpRuleTest() {
            lock (_locker) {
                Firewall.EnableFirewall();
                Firewall.AddRdpRule();
                Assert.IsTrue(Firewall.IsRdpRuleExists());
                Firewall.DisableFirewall();
                Assert.IsTrue(Firewall.IsRdpRuleExists());
                Firewall.RemoveRdpRule();
            }
        }

        [TestMethod]
        public void MinerClientRuleTest() {
            lock (_locker) {
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
}
