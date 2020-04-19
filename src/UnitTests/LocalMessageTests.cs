using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using System.Linq;

namespace NTMiner {
    [TestClass]
    public class LocalMessageTests {
        [TestMethod]
        public void BenchmarkTest() {
            VirtualRoot.Execute(new ClearLocalMessageSetCommand());
            int times = 2000;
            Assert.IsTrue(times > NTKeyword.LocalMessageSetCapacity);
            string content = "this is a test";
            for (int i = 0; i < times; i++) {
                VirtualRoot.ThisLocalInfo(nameof(LocalMessageTests), content);
            }
            Assert.IsTrue(NTMinerContext.Instance.LocalMessageSet.AsEnumerable().Count() == NTKeyword.LocalMessageSetCapacity);
        }
    }
}
