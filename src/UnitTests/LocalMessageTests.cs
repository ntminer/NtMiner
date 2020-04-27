using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace NTMiner {
    [TestClass]
    public class LocalMessageTests {
        [TestMethod]
        public void BenchmarkTest() {
            VirtualRoot.Execute(new ClearLocalMessageSetCommand());
            int times = 2000;
            Assert.IsTrue(times > NTKeyword.LocalMessageSetCapacity);
            // 触发LocalMessageSet对AddLocalMessageCommand命令的订阅
            _ = NTMinerContext.Instance.LocalMessageSet;
            string content = "this is a test";
            for (int i = 0; i < times; i++) {
                VirtualRoot.ThisLocalInfo(nameof(LocalMessageTests), content);
            }
            Assert.AreEqual(NTKeyword.LocalMessageSetCapacity, NTMinerContext.Instance.LocalMessageSet.AsEnumerable().Count());
        }
    }
}
