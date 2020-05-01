using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.Ws;
using System;
using System.Collections.Generic;
using System.Text;

namespace NTMiner {
    [TestClass]
    public class BinarySerializerTests {
        [TestMethod]
        public void Test() {
            byte[] data1 = Encoding.UTF8.GetBytes("gzipped");
            byte[] data2 = Encoding.UTF8.GetBytes("gzipped");
            Assert.AreNotEqual(data1, data2);
            Assert.IsFalse(data1 == data2);
            Assert.IsFalse(data1.Equals(data2));
        }

        [TestMethod]
        public void Test1() {
            Dictionary<string, object> dic1 = new Dictionary<string, object> {
                {"A", 1 },
                {"B", DateTime.Now },
                {"C", "this is a test" }
            };
            byte[] data = VirtualRoot.BinarySerializer.Serialize(dic1);
            Assert.IsFalse(VirtualRoot.BinarySerializer.IsGZipped(data));
            Dictionary<string, object> dic2 = VirtualRoot.BinarySerializer.Deserialize<Dictionary<string, object>>(data);
            foreach (var key in dic1.Keys) {
                if (key == "A") {
                    // object int 反序列化后会是long
                    Assert.AreNotEqual(dic1[key], dic2[key]);
                    Assert.AreEqual(Convert.ToInt64(dic1[key]), dic2[key]);
                }
                else {
                    Assert.AreEqual(dic1[key], dic2[key]);
                }
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
