using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;

namespace UnitTests {
    [TestClass]
    public class HttpClientTests {
        [TestMethod]
        public void TestMethod1() {
            using (HttpClient client = new HttpClient()) {
                Assert.AreEqual(100, client.Timeout.TotalSeconds);
            }
        }
    }
}
