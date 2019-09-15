using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
    }
}
