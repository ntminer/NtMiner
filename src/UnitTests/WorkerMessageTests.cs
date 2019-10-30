using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using System.IO;

namespace UnitTests {
    [TestClass]
    public class WorkerMessageTests {
        [TestMethod]
        public void TestMethod1() {
            File.Delete(VirtualRoot.WorkerMessageDbFileFullName);
            Assert.IsTrue(VirtualRoot.WorkerMessages.Count == 0);
            string content = "this is a test";
            VirtualRoot.ThisWorkerInfo(nameof(WorkerMessageTests), content);
            Assert.IsTrue(VirtualRoot.WorkerMessages.Count == 1);
            Assert.IsTrue(VirtualRoot.WorkerMessages.Count == 1);
        }

        [TestMethod]
        public void BenchmarkTest() {
            File.Delete(VirtualRoot.WorkerMessageDbFileFullName);
            int times = 2000;
            Assert.IsTrue(times > NTKeyword.WorkerMessageSetCapacity);
            string content = "this is a test";
            for (int i = 0; i < times; i++) {
                VirtualRoot.ThisWorkerInfo(nameof(WorkerMessageTests), content);
            }
            Assert.IsTrue(VirtualRoot.WorkerMessages.Count == NTKeyword.WorkerMessageSetCapacity);
        }
    }
}
