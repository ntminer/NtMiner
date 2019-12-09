using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace UnitTests {
    [TestClass]
    public class HttpClientTests {
        [TestMethod]
        public void TestMethod1() {
            using (HttpClient client = new HttpClient()) {
                Assert.AreEqual(100, client.Timeout.TotalSeconds);
            }
            using (HttpClient client = RpcRoot.Create()) {
                Assert.AreEqual(60, client.Timeout.TotalSeconds);
            }
        }

        [TestMethod]
        public void TaskTest() {
            try {
                using (HttpClient client = RpcRoot.Create()) {
                    Task<HttpResponseMessage> getHttpResponse = client.GetAsync($"http://{NTKeyword.OfficialServerHost}:{NTKeyword.ControlCenterPort.ToString()}/api/AppSetting/GetTime");
                    DateTime response = getHttpResponse.Result.Content.ReadAsAsync<DateTime>().Result;
                    
                }
            }
            catch (Exception e) {
            }
        }
    }
}
