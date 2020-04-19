using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace NTMiner {
    [TestClass]
    public class DictionaryTests {
        [TestMethod]
        public void DictionarySetTest() {
            var dic = new Dictionary<string, string>();
            // 与指定的键相关联的值。 如果指定键未找到，则 Get 操作引发 System.Collections.Generic.KeyNotFoundException，而
            // Set 操作创建一个带指定键的新元素。
            dic["test1"] = "value1";
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void DictionaryGetTest() {
            var dic = new Dictionary<string, string>();
            // 与指定的键相关联的值。 如果指定键未找到，则 Get 操作引发 System.Collections.Generic.KeyNotFoundException，而
            // Set 操作创建一个带指定键的新元素。
            var v = dic["test1"];
        }
    }
}
