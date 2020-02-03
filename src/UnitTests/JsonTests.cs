using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NTMiner;
using NTMiner.JsonDb;

namespace UnitTests {
    [TestClass]
    public class JsonTests {
        [TestMethod]
        public void TestMethod1() {
            GpuProfilesJsonDb data = VirtualRoot.JsonSerializer.Deserialize<GpuProfilesJsonDb>("");
            Assert.IsNull(data); 
        }

        [TestMethod]
        [ExpectedException(typeof(JsonReaderException))]
        public void TestMethod2() {
            var data = VirtualRoot.JsonSerializer.Deserialize<GpuProfilesJsonDb>("sss");
            Assert.IsNull(data);
        }
    }
}
