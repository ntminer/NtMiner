using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
        [ExpectedException(typeof(KeyNotFoundException))]
        public void DictionaryGetTest() {
            var dic = new Dictionary<string, string>();
            // 与指定的键相关联的值。 如果指定键未找到，则 Get 操作引发 System.Collections.Generic.KeyNotFoundException，而
            // Set 操作创建一个带指定键的新元素。
            var v = dic["test1"];
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
            bool 集合已修改 = false;
            try {
                foreach (var item in dic.Values) {
                    Thread.Sleep(3);
                }
            }
            catch (InvalidOperationException) {
                集合已修改 = true;
            }
            Assert.IsTrue(集合已修改);
        }
    }
}
