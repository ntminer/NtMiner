using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner;
using NTMiner.Core;
using NTMiner.MinerClient;
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
            var data = new WorkerEventData {
                Content = "this is a test",
                EventOn = DateTime.Now,
                EventTypeId = Guid.NewGuid(),
                Guid = Guid.NewGuid(),
                Id = 0
            };
            VirtualRoot.Happened(new WorkerEventHappenedEvent(data));
            Assert.IsTrue(NTMinerRoot.Instance.WorkerEventSet.LastWorkerEventId == 1);
            var workerEvnets = NTMinerRoot.Instance.WorkerEventSet.GetEvents(Guid.Empty, string.Empty).ToList();
            Assert.IsTrue(workerEvnets.Count == 1);
            Assert.AreEqual(data.Guid, workerEvnets[0].GetId());
            Assert.AreEqual(NTMinerRoot.Instance.WorkerEventSet.LastWorkerEventId, workerEvnets[0].Id);
            workerEvnets = NTMinerRoot.Instance.WorkerEventSet.GetEvents(data.EventTypeId, string.Empty).ToList();
            Assert.IsTrue(workerEvnets.Count == 1);
            workerEvnets = NTMinerRoot.Instance.WorkerEventSet.GetEvents(Guid.Empty, "test").ToList();
            Assert.IsTrue(workerEvnets.Count == 1);
            workerEvnets = NTMinerRoot.Instance.WorkerEventSet.GetEvents(data.EventTypeId, "test").ToList();
            Assert.IsTrue(workerEvnets.Count == 1);
            workerEvnets = NTMinerRoot.Instance.WorkerEventSet.GetEvents(Guid.Empty, "aaaaaa").ToList();
            Assert.IsTrue(workerEvnets.Count == 0);
        }
    }
}
