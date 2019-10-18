using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using NTMiner.MinerClient;
using System.IO;
using System.Linq;

namespace UnitTests {
    [TestClass]
    public class WorkerEventTests {
        [TestMethod]
        public void TestMethod1() {
            File.Delete(VirtualRoot.WorkerEventDbFileFullName);
            Assert.IsTrue(VirtualRoot.WorkerEvents.LastWorkerEventId == 0);
            WorkerEventChannel eventChannel = WorkerEventChannel.Local;
            string content = "this is a test";
            VirtualRoot.WorkerEvent(eventChannel, WorkerEventType.Info, content);
            Assert.IsTrue(VirtualRoot.WorkerEvents.LastWorkerEventId == 1);
            var workerEvnets = VirtualRoot.WorkerEvents.GetEvents(WorkerEventChannel.Undefined, string.Empty).ToList();
            Assert.IsTrue(workerEvnets.Count == 1);
            Assert.AreEqual(VirtualRoot.WorkerEvents.LastWorkerEventId, workerEvnets[0].Id);
            workerEvnets = VirtualRoot.WorkerEvents.GetEvents(eventChannel, string.Empty).ToList();
            Assert.IsTrue(workerEvnets.Count == 1);
            workerEvnets = VirtualRoot.WorkerEvents.GetEvents(WorkerEventChannel.Undefined, "test").ToList();
            Assert.IsTrue(workerEvnets.Count == 1);
            workerEvnets = VirtualRoot.WorkerEvents.GetEvents(eventChannel, "test").ToList();
            Assert.IsTrue(workerEvnets.Count == 1);
            workerEvnets = VirtualRoot.WorkerEvents.GetEvents(WorkerEventChannel.Undefined, "aaaaaa").ToList();
            Assert.IsTrue(workerEvnets.Count == 0);
        }

        [TestMethod]
        public void BenchmarkTest() {
            File.Delete(VirtualRoot.WorkerEventDbFileFullName);
            Assert.IsTrue(VirtualRoot.WorkerEvents.LastWorkerEventId == 0);
            int times = 2000;
            WorkerEventChannel eventChannel = WorkerEventChannel.Local;
            string content = "this is a test";
            for (int i = 0; i < times; i++) {
                VirtualRoot.WorkerEvent(eventChannel, WorkerEventType.Info, content);
            }
            Assert.IsTrue(VirtualRoot.WorkerEvents.LastWorkerEventId == times);
            var workerEvnets = VirtualRoot.WorkerEvents.GetEvents(eventChannel, "test").ToList();
            Assert.AreEqual(VirtualRoot.WorkerEventSetSliding, workerEvnets.Count);
        }
    }
}
