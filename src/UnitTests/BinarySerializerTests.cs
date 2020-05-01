using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.Ws;
using System;
using System.Collections.Generic;

namespace NTMiner {
    [TestClass]
    public class BinarySerializerTests {
        [TestMethod]
        public void Test1() {
            Dictionary<string, object> dic1 = new Dictionary<string, object> {
                {"A", 1 },
                {"B", DateTime.Now },
                {"C", "this is a test" }
            };
            byte[] data = VirtualRoot.BinarySerializer.Serialize(dic1);
            Dictionary<string, object> dic2 = VirtualRoot.BinarySerializer.Deserialize<Dictionary<string, object>>(data);
            foreach (var key in dic1.Keys) {
                Assert.AreEqual(dic1[key], dic2[key]);
            }
        }

        [TestMethod]
        public void BenchmarkTest() {
            int n = 10000;
            List<WsMessage> messages = new List<WsMessage>();
            for (int i = 0; i < n; i++) {
                messages.Add(new WsMessage(Guid.NewGuid(), "test") {
                    Id = Guid.NewGuid(),
                    Timestamp = Timestamp.GetTimestamp(),
                    Sign = Guid.NewGuid().ToString(),
                    Data = new Dictionary<string, object> {
                        {"AAAAAAAA", 1 },
                        {"BBBBBBBB", DateTime.Now },
                        {"CCCCCCCC", "hello world this is a test" },
                        {"DDDDDDDD", Guid.NewGuid() }
                    }
                });
            }
            NTStopwatch.Start();
            for (int i = 0; i < messages.Count; i++) {
                var message = messages[i];
                byte[] data = VirtualRoot.BinarySerializer.Serialize(message);
                VirtualRoot.BinarySerializer.Deserialize<WsMessage>(data);
            }
            var elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);
        }
    }
}
