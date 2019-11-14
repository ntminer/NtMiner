using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using System.Linq;

namespace UnitTests {
    [TestClass]
    public class LocalMessageTests {
        [TestMethod]
        public void BenchmarkTest() {
            VirtualRoot.Execute(new ClearLocalMessageSetCommand());
            int times = 2000;
            Assert.IsTrue(times > NTKeyword.LocalMessageSetCapacity);
            string content = "this is a test";
            for (int i = 0; i < times; i++) {
                VirtualRoot.LocalInfo(nameof(LocalMessageTests), content);
            }
            Assert.IsTrue(VirtualRoot.LocalMessages.Count() == NTKeyword.LocalMessageSetCapacity);
        }
    }
}
