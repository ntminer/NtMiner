using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using NTMiner.Core;
using NTMiner.MinerClient;
using System.IO;
using System.Linq;

namespace UnitTests {
    [TestClass]
    public class WorkerEventTests {
        [TestMethod]
        public void TestMethod1() {
            File.Delete(SpecialPath.WorkerEventDbFileFullName);
            Assert.IsTrue(NTMinerRoot.Instance.WorkerEventSet.LastWorkerEventId == 0);
            WorkerEventChannel eventChannel = WorkerEventChannel.MinerClient;
            string content = "this is a test";
            VirtualRoot.Happened(new WorkerEvent(eventChannel, content));
            Assert.IsTrue(NTMinerRoot.Instance.WorkerEventSet.LastWorkerEventId == 1);
            var workerEvnets = NTMinerRoot.Instance.WorkerEventSet.GetEvents(WorkerEventChannel.Undefined, string.Empty).ToList();
            Assert.IsTrue(workerEvnets.Count == 1);
            Assert.AreEqual(NTMinerRoot.Instance.WorkerEventSet.LastWorkerEventId, workerEvnets[0].Id);
            workerEvnets = NTMinerRoot.Instance.WorkerEventSet.GetEvents(eventChannel, string.Empty).ToList();
            Assert.IsTrue(workerEvnets.Count == 1);
            workerEvnets = NTMinerRoot.Instance.WorkerEventSet.GetEvents(WorkerEventChannel.Undefined, "test").ToList();
            Assert.IsTrue(workerEvnets.Count == 1);
            workerEvnets = NTMinerRoot.Instance.WorkerEventSet.GetEvents(eventChannel, "test").ToList();
            Assert.IsTrue(workerEvnets.Count == 1);
            workerEvnets = NTMinerRoot.Instance.WorkerEventSet.GetEvents(WorkerEventChannel.Undefined, "aaaaaa").ToList();
            Assert.IsTrue(workerEvnets.Count == 0);
        }

        [TestMethod]
        public void BenchmarkTest() {
            File.Delete(SpecialPath.WorkerEventDbFileFullName);
            Assert.IsTrue(NTMinerRoot.Instance.WorkerEventSet.LastWorkerEventId == 0);
            int times = 2000;
            WorkerEventChannel eventChannel = WorkerEventChannel.MinerClient;
            string content = "this is a test";
            for (int i = 0; i < times; i++) {
                VirtualRoot.Happened(new WorkerEvent(eventChannel, content));
            }
            Assert.IsTrue(NTMinerRoot.Instance.WorkerEventSet.LastWorkerEventId == times);
            var workerEvnets = NTMinerRoot.Instance.WorkerEventSet.GetEvents(eventChannel, "test").ToList();
            Assert.AreEqual(VirtualRoot.WorkerEventSetSliding, workerEvnets.Count);
        }
    }
}
