using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net.Http;

namespace NTMiner {
    [TestClass]
    public class StreamTests {
        [TestMethod]
        public void Test1() {
            using (var fs = File.OpenRead(VirtualRoot.AppFileFullName)) {
                Assert.IsTrue(fs.CanRead);
                fs.Dispose();
                Assert.IsFalse(fs.CanRead);
            }
            using (var ms = new MemoryStream()) {
                Assert.IsTrue(ms.CanRead);
                ms.Dispose();
                Assert.IsFalse(ms.CanRead);
            }
            using (var client = new HttpClient()) {
                client.GetStreamAsync("http://www.baidu.com").ContinueWith(t => {
                    Assert.IsTrue(t.Result.CanRead);
                    Console.WriteLine(t.Result.CanRead);
                    t.Result.Dispose();
                    Assert.IsFalse(t.Result.CanSeek);
                    Console.WriteLine(t.Result.CanRead);
                }).Wait();
            }
        }
    }
}
