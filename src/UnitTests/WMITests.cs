using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Management;
using System.Text;

namespace UnitTestProject1 {
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
        public void IpTest() {
            var str = new StringBuilder();
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc) {
                if (!(bool)mo["IPEnabled"]) {
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
