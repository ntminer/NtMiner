using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.Controllers;
using System;
using System.Net.Http;

namespace NTMiner {
    [TestClass]
    public class HttpClientTests {
        [TestMethod]
        public void TestMethod1() {
            using (HttpClient client = new HttpClient()) {
                Assert.AreEqual(100, client.Timeout.TotalSeconds);
            }
            using (HttpClient client = RpcRoot.CreateHttpClient()) {
                Assert.AreEqual(10, client.Timeout.TotalSeconds);
                client.SetTimeout(5000);
                Assert.AreEqual(5000, client.Timeout.TotalMilliseconds);
            }
            using (HttpClient client = RpcRoot.CreateHttpClient()) {
                // 0不起作用
                client.SetTimeout(0);
                Assert.AreEqual(10, client.Timeout.TotalSeconds);
            }
        }

        [TestMethod]
        public void TaskTest() {
            HttpClient client = RpcRoot.CreateHttpClient();
            client.GetAsync($"http://{RpcRoot.OfficialServerAddress}/api/{RpcRoot.GetControllerName<IAppSettingController>()}/{nameof(IAppSettingController.GetTime)}")
                .ContinueWith(t => {
                    Console.WriteLine(t.Result.Content.ReadAsAsync<DateTime>().Result);
                }).Wait();
        }
    }
}
