using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
