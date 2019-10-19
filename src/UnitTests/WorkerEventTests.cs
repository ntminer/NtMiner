using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using System.IO;

namespace UnitTests {
    [TestClass]
    public class WorkerEventTests {
        [TestMethod]
        public void TestMethod1() {
            File.Delete(VirtualRoot.WorkerEventDbFileFullName);
            Assert.IsTrue(VirtualRoot.WorkerEvents.Count == 0);
            WorkerEventChannel eventChannel = WorkerEventChannel.This;
            string content = "this is a test";
            VirtualRoot.WorkerEvent(eventChannel, nameof(WorkerEventTests), WorkerEventType.Info, content);
            Assert.IsTrue(VirtualRoot.WorkerEvents.Count == 1);
            Assert.IsTrue(VirtualRoot.WorkerEvents.Count == 1);
        }

        [TestMethod]
        public void BenchmarkTest() {
            File.Delete(VirtualRoot.WorkerEventDbFileFullName);
            int times = 2000;
            WorkerEventChannel eventChannel = WorkerEventChannel.This;
            string content = "this is a test";
            for (int i = 0; i < times; i++) {
                VirtualRoot.WorkerEvent(eventChannel, nameof(WorkerEventTests), WorkerEventType.Info, content);
            }
            Assert.IsTrue(VirtualRoot.WorkerEvents.Count == times);
        }
    }
}
