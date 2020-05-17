using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.Net;
using System;
using System.Collections;
using System.IO;

namespace NTMiner {
    [TestClass]
    public class EnvironmentTests {
        [TestMethod]
        public void FolderTest() {
            string[] names = Enum.GetNames(typeof(Environment.SpecialFolder));
            // Environment.SpecialFolder的枚举值是有重复的
            Array values = Enum.GetValues(typeof(Environment.SpecialFolder));
            for (int i = 0; i < names.Length; i++) {
                Console.WriteLine($"{names[i]} {Environment.GetFolderPath((Environment.SpecialFolder)values.GetValue(i))}");
            }
        }

        [TestMethod]
        public void PathTest() {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers\\etc\\hosts");
            Console.WriteLine(path);
        }

        [TestMethod]
        public void EndOfStreamTest() {
            string tempFile = Path.Combine(HomePath.BaseDirectory, Guid.NewGuid().ToString());
            File.Create(tempFile).Close();
            using (FileStream fs = new FileStream(tempFile, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs)) {
                Assert.IsTrue(sr.EndOfStream);
            }
            File.Delete(tempFile);
        }

        [TestMethod]
        public void PathsTest() {
            var values = Enum.GetValues(typeof(Environment.SpecialFolder)) as IEnumerable;
            foreach (var value in values) {
                Console.WriteLine(Environment.GetFolderPath((Environment.SpecialFolder)value));
            }
        }

        [TestMethod]
        public void HostsTest() {
            string hostsPath = Path.Combine(HomePath.BaseDirectory, "hosts");
            File.Delete(hostsPath);
            string host = RpcRoot.OfficialServerHost;
            string ip = "127.0.0.1";
            Hosts.GetIp(host, out long r, hostsPath);
            Assert.AreEqual(-2, r);
            File.Create(hostsPath).Close(); 
            Hosts.GetIp(host, out r, hostsPath);
            Assert.AreEqual(-1, r);
            File.Delete(hostsPath);
            Hosts.SetHost(host, ip, hostsPath);
            Assert.AreEqual(ip, Hosts.GetIp(host, out r, hostsPath));
            Assert.IsTrue(r >= 0);
            File.Delete(hostsPath);
            Hosts.SetHost("test.com", "111.111.111.111", hostsPath);
            Hosts.SetHost(host, ip, hostsPath);
            Assert.AreEqual(ip, Hosts.GetIp(host, out r, hostsPath));
        }
    }
}
