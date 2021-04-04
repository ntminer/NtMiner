using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NTMiner {
    [TestClass]
    public class DictionaryTests {
        [TestMethod]
        public void DictionarySetTest() {
            var dic = new Dictionary<string, string>();
            // 与指定的键相关联的值。 如果指定键未找到，则 Get 操作引发 System.Collections.Generic.KeyNotFoundException，而
            // Set 操作创建一个带指定键的新元素。
            dic["test1"] = "value1";
            dic.Remove("testaaa");
        }

        [TestMethod]
        public void KeyValuePairToString() {
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>("key", "value");
            Assert.AreEqual("[key, value]", kv.ToString());
        }

        [TestMethod]
        public void ToStringTest() {
            var dic = new Dictionary<string, string> {
                {"key1", "value1" }
            };
            Assert.AreEqual("System.Collections.Generic.Dictionary`2[System.String,System.String]", dic.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void DictionaryGetTest() {
            var dic = new Dictionary<string, string>();
            // 与指定的键相关联的值。 如果指定键未找到，则 Get 操作引发 System.Collections.Generic.KeyNotFoundException，而
            // Set 操作创建一个带指定键的新元素。
            var v = dic["test1"];
        }

        [TestMethod]
        public void TryRemoveTest() {
            Guid guid = Guid.NewGuid();
            ConcurrentDictionary<Guid, DateTime> dic = new ConcurrentDictionary<Guid, DateTime> {
                [guid] = DateTime.Now
            };
            // 移除不存在的键应返回false
            Assert.IsFalse(dic.TryRemove(Guid.NewGuid(), out _));
            Assert.IsTrue(dic.TryRemove(guid, out _));
        }

        [TestMethod]
        public void MutiThreadTest() {
            Dictionary<Guid, object> dic = new Dictionary<Guid, object>();
            Task.Factory.StartNew(() => {
                for (int i = 0; i < 1000; i++) {
                    dic.Add(Guid.NewGuid(), string.Empty);
                    if (i > 100) {
                        Thread.Sleep(5);
                    }
                }
            });
            Thread.Sleep(100);
            bool isSetModified = false;
            try {
                foreach (var item in dic.Values) {
                    Thread.Sleep(3);
                }
            }
            catch (InvalidOperationException) {
                isSetModified = true;
            }
            Assert.IsTrue(isSetModified);
        }

        [TestMethod]
        public void BenchmarkTest() {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            int n = 100000;
            NTStopwatch.Start();
            for (int i = 0; i < n; i++) {
                string key = Guid.NewGuid().ToString();
                if (!dic.ContainsKey(key)) {
                    dic.Add(key, key);
                }
            }
            var elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);

            dic = new Dictionary<string, string>();
            NTStopwatch.Start();
            for (int i = 0; i < n; i++) {
                string key = Guid.NewGuid().ToString();
                dic[key] = key;
            }
            elapsedMilliseconds = NTStopwatch.Stop();
            Console.WriteLine(elapsedMilliseconds);
            // 没啥区别
        }
    }
}
