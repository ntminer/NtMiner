using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using System.Collections.Generic;

namespace UnitTests {
    [TestClass]
    public class JsonSerializerTests {
        [TestMethod]
        public void DeserializeTest() {
            var dic = VirtualRoot.JsonSerializer.Deserialize<Dictionary<string, object>>(string.Empty);
            Assert.IsNull(dic);
            dic = VirtualRoot.JsonSerializer.Deserialize<Dictionary<string, object>>("{}");
            Assert.IsNotNull(dic);
            bool raiseException = false;
            try {
                dic = VirtualRoot.JsonSerializer.Deserialize<Dictionary<string, object>>("aa");
            }
            catch {
                raiseException = true;
            }
            finally {
                Assert.IsTrue(raiseException);
            }
        }
    }
}
