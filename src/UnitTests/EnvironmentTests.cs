using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using System;
using System.IO;

namespace UnitTests {
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
            string tempFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Guid.NewGuid().ToString());
            File.Create(tempFile).Close();
            using (FileStream fs = new FileStream(tempFile, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs)) {
                Assert.IsTrue(sr.EndOfStream);
            }
        }

        [TestMethod]
        public void HostsTest() {
            string hostsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hosts");
            File.Delete(hostsPath);
            long r = Hosts.GetHost("server.ntminer.com", hostsPath);
            Assert.AreEqual(-2, r);
            File.Create(hostsPath).Close(); 
            r = Hosts.GetHost("server.ntminer.com", hostsPath);
            Assert.AreEqual(-1, r);
            File.Delete(hostsPath);
        }
    }
}
