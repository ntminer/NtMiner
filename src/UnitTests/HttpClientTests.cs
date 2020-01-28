using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using System;
using System.Net.Http;

namespace UnitTests {
    [TestClass]
    public class HttpClientTests {
        [TestMethod]
        public void TestMethod1() {
            using (HttpClient client = new HttpClient()) {
                Assert.AreEqual(100, client.Timeout.TotalSeconds);
            }
            using (HttpClient client = RpcRoot.CreateHttpClient()) {
                Assert.AreEqual(60, client.Timeout.TotalSeconds);
            }
        }

        [TestMethod]
        public void TaskTest() {
            HttpClient client = RpcRoot.CreateHttpClient();
            client.GetAsync($"http://{NTKeyword.OfficialServerHost}:{NTKeyword.ControlCenterPort.ToString()}/api/AppSetting/GetTime")
                .ContinueWith(t => {
                    Console.WriteLine(t.Result.Content.ReadAsAsync<DateTime>().Result);
                }).Wait();
        }
    }
}
