using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using NTMiner.Core;
using System;
using System.IO;
using System.Linq;

namespace UnitTests {
    [TestClass]
    public class WorkerEventTests {
        [TestMethod]
        public void TestMethod1() {
            File.Delete(SpecialPath.WorkerEventDbFileFullName);
            Assert.IsTrue(NTMinerRoot.Instance.WorkerEventSet.LastWorkerEventId == 0);
            Guid eventTypeId = Guid.NewGuid();
            string content = "this is a test";
            VirtualRoot.Happened(new WorkerEvent(eventTypeId, content));
            Assert.IsTrue(NTMinerRoot.Instance.WorkerEventSet.LastWorkerEventId == 1);
            var workerEvnets = NTMinerRoot.Instance.WorkerEventSet.GetEvents(Guid.Empty, string.Empty).ToList();
            Assert.IsTrue(workerEvnets.Count == 1);
            Assert.AreEqual(NTMinerRoot.Instance.WorkerEventSet.LastWorkerEventId, workerEvnets[0].Id);
            workerEvnets = NTMinerRoot.Instance.WorkerEventSet.GetEvents(eventTypeId, string.Empty).ToList();
            Assert.IsTrue(workerEvnets.Count == 1);
            workerEvnets = NTMinerRoot.Instance.WorkerEventSet.GetEvents(Guid.Empty, "test").ToList();
            Assert.IsTrue(workerEvnets.Count == 1);
            workerEvnets = NTMinerRoot.Instance.WorkerEventSet.GetEvents(eventTypeId, "test").ToList();
            Assert.IsTrue(workerEvnets.Count == 1);
            workerEvnets = NTMinerRoot.Instance.WorkerEventSet.GetEvents(Guid.Empty, "aaaaaa").ToList();
            Assert.IsTrue(workerEvnets.Count == 0);
        }

        [TestMethod]
        public void BenchmarkTest() {
            File.Delete(SpecialPath.WorkerEventDbFileFullName);
            Assert.IsTrue(NTMinerRoot.Instance.WorkerEventSet.LastWorkerEventId == 0);
            int times = 2000;
            Guid eventTypeId = Guid.NewGuid();
            string content = "this is a test";
            for (int i = 0; i < times; i++) {
                VirtualRoot.Happened(new WorkerEvent(eventTypeId, content));
            }
            Assert.IsTrue(NTMinerRoot.Instance.WorkerEventSet.LastWorkerEventId == times);
            var workerEvnets = NTMinerRoot.Instance.WorkerEventSet.GetEvents(eventTypeId, "test").ToList();
            Assert.AreEqual(VirtualRoot.WorkerEventSetSliding, workerEvnets.Count);
        }
    }
}
