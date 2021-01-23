using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;

namespace NTMiner {
    [TestClass]
    public class NameValueCollectionTests {
        [TestMethod]
        public void Test1() {
            var headers = new NameValueCollection();
            Assert.IsNull(headers["aaa"]);
        }
    }
}
