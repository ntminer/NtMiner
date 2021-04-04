using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NTMiner.ServerNode;
using NTMiner.Ws;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public class ClassA {
        public ClassA() { }

        public string Property1 { get; set; }
        // 必须标记上JsonIgnore才能避免序列化
        [JsonIgnore]
        public string ReadOnlyProperty1 {
            get {
                return this.Property1;
            }
        }
    }

    [TestClass]
    public class JsonSerializerTests {
        [TestMethod]
        public void ReadOnlyPropertyTest() {
            ClassA a = new ClassA {
                Property1 = "this is a test"
            };
            string json = VirtualRoot.JsonSerializer.Serialize(a);
            Console.WriteLine(json);
        }

        [TestMethod]
        public void DictonaryTest() {
            Dictionary<string, object> dic = new Dictionary<string, object> {
                ["A"] = 1,
                ["B"] = DateTime.Now,
                ["C"] = "this is a test"
            };
            Console.WriteLine(VirtualRoot.JsonSerializer.Serialize(dic));
        }

        [TestMethod]
        public void ConsoleOutLineTest() {
            ConsoleOutLine consoleOutLine = new ConsoleOutLine {
                Timestamp = Timestamp.GetTimestamp(),
                Line = "this is a test"
            };
            string json = VirtualRoot.JsonSerializer.Serialize(consoleOutLine);
            Console.WriteLine(json);
        }

        [TestMethod]
        public void DeserializeTest() {
            var dic = VirtualRoot.JsonSerializer.Deserialize<Dictionary<string, object>>(string.Empty);
            Assert.IsNull(dic);
            dic = VirtualRoot.JsonSerializer.Deserialize<Dictionary<string, object>>("{}");
            Assert.IsNotNull(dic);
            bool raiseException = false;
            try {
                dic = VirtualRoot.JsonSerializer.Deserialize<Dictionary<string, object>>("aa");
            }
            catch {
                raiseException = true;
            }
            finally {
                Assert.IsFalse(raiseException);
            }
        }

        [TestMethod]
        public void ObjectTest() {
            object obj = new object();
            string json = VirtualRoot.JsonSerializer.Serialize(obj);
            Assert.AreEqual("{}", json);
        }

        [TestMethod]
        public void IntPtrTest() {
            IntPtr p = new IntPtr(1234567890);
            var json = VirtualRoot.JsonSerializer.Serialize(p);
            Console.WriteLine(json);
            var p1 = VirtualRoot.JsonSerializer.Deserialize<IntPtr>(json);
            Console.WriteLine(p1);
        }

        [TestMethod]
        public void ListTest() {
            List<string> minerIds = new List<string> {
                "aaaaaaa",
                "bbbbbbb",
                "ccccccc"
            };
            var json = VirtualRoot.JsonSerializer.Serialize(minerIds);
            Assert.AreEqual("[\"aaaaaaa\",\"bbbbbbb\",\"ccccccc\"]", json);
        }

        [TestMethod]
        public void ArrayTest() {
            MqCountData[] data = new MqCountData[] {
                new MqCountData(),
                new MqCountData()
            };
            string json = VirtualRoot.JsonSerializer.Serialize(data);
            Console.WriteLine(json);
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
                        ["AAAAAAAA"] = 1,
                        ["BBBBBBBB"] = DateTime.Now,
                        ["CCCCCCCC"] = "hello world this is a test",
                        ["DDDDDDDD"] = Guid.NewGuid()
                    }
                });
            }
            NTStopwatch.Start();
            for (int i = 0; i < messages.Count; i++) {
                var message = messages[i];
                string json = VirtualRoot.JsonSerializer.Serialize(message);
                VirtualRoot.JsonSerializer.Deserialize<WsMessage>(json);
            }
            var elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);
        }
    }
}
