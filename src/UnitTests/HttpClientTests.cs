using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using System.Net.Http;

namespace UnitTests {
    [TestClass]
    public class HttpClientTests {
        [TestMethod]
        public void TestMethod1() {
            using (HttpClient client = new HttpClient()) {
                Assert.AreEqual(100, client.Timeout.TotalSeconds);
            }
            using (HttpClient client = HttpClientFactory.Create()) {
                Assert.AreEqual(60, client.Timeout.TotalSeconds);
            }
        }
    }
}
