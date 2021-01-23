using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NTMiner {
    [TestClass]
    public class UriTests {
        [TestMethod]
        public void Test1() {
            string url = "http://ntminer.com/helloworld?aaa=sss#s=ddd";
            Uri.TryCreate(url, UriKind.Absolute, out Uri uri);
            Assert.AreEqual(80, uri.Port);
            Assert.AreEqual("ntminer.com", uri.Host);
            Assert.AreEqual(uri.Authority, uri.Host);
            Assert.AreEqual(uri.DnsSafeHost, uri.Host);
            Assert.AreEqual("/helloworld", uri.AbsolutePath);
            Assert.AreEqual("#s=ddd", uri.Fragment);
            Assert.AreEqual("?aaa=sss", uri.Query);
            url = "https://ntminer.com/helloworld?aaa=sss#s=ddd";
            Uri.TryCreate(url, UriKind.Absolute, out uri);
            Assert.AreEqual(443, uri.Port);
        }
    }
}
