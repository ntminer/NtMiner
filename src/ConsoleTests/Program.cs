using System;
using System.Management;
using System.Text;

namespace NTMiner {
    class Program {
        static void Main(string[] args) {
            DevMode.SetDevMode();

            ObjectJsonSerializer objectJsonSerializer = new ObjectJsonSerializer();
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
            Console.ReadKey();
        }
    }
}
