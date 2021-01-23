using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using WebSocketSharp;

namespace NTMiner {
    [TestClass]
    public class WebSocketTests {
        [TestMethod]
        public void PrivateFiledTest() {
            FieldInfo fieldInfo = typeof(WebSocket).GetField("_retryCountForConnect", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(fieldInfo);
            fieldInfo = typeof(WebSocket).GetField("goodgoodstudydaydayup", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNull(fieldInfo);
        }
    }
}
