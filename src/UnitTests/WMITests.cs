using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Management;
using System.Text;

namespace NTMiner {
    [TestClass]
    public class WMITests {
        [TestMethod]
        public void TestMethod1() {
            ManagementObjectSearcher managementObject = new ManagementObjectSearcher("select * from Win32_VideoController");
            var str = new StringBuilder();
            foreach (ManagementObject m in managementObject.Get()) {
                foreach (var kv in m.Properties) {
                    str.Append(kv.Name);
                    str.Append(": ");
                    str.Append(kv.Value);
                    str.Append("\n");
                }
            }
            Console.WriteLine(str);
        }

        [TestMethod]
        public void ProcessInfoTest() {
            string wmiQuery = $"select * from Win32_Process where Name='devenv.exe'";// VS的进程
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQuery))
            using (ManagementObjectCollection retObjectCollection = searcher.Get()) {
                foreach (ManagementObject retObject in retObjectCollection) {
                    foreach (var kv in retObject.Properties) {
                        Console.WriteLine(kv.Name + ":" + kv.Value);
                    }
                }
            }
        }

        [TestMethod]
        public void ProcessInfoTest2() {
            string wmiQuery = $"select Caption,ExecutablePath from Win32_Process";
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQuery))
            using (ManagementObjectCollection retObjectCollection = searcher.Get()) {
                foreach (ManagementObject retObject in retObjectCollection) {
                    Console.WriteLine($"Caption: {retObject["Caption"].ToString().PadRight(50)},ExecutablePath: {retObject["ExecutablePath"]}");
                }
            }
        }

        [TestMethod]
        public void GetCommandLineTest() {
            var cmdLines = NTMiner.Windows.WMI.GetCommandLines("devenv.exe");
            foreach (var item in cmdLines) {
                Console.WriteLine(item);
            }
        }

        [TestMethod]
        public void IpTest() {
            var str = new StringBuilder();
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc) {
                if (!(bool)mo["IPEnabled"]) {
                    continue;
                }
                if (mo["DefaultIPGateway"] == null || ((string[])mo["DefaultIPGateway"]).Length == 0) {
                    continue;
                }
                foreach (var kv in mo.Properties) {
                    str.Append(kv.Name);
                    str.Append(": ");
                    if (kv.Value != null && kv.IsArray) {
                        foreach (var item in (IEnumerable)kv.Value) {
                            str.Append(item).Append(";");
                        }
                    }
                    else {
                        str.Append(kv.Value);
                    }
                    str.Append("\n");
                }
                str.Append("==========================\n");
            }
            Console.WriteLine(str);
        }
    }
}
