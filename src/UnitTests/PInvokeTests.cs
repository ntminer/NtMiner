using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.Gpus;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NTMiner {
    [TestClass]
    public class PInvokeTests {
        [TestMethod]
        public void PairTest() {
            IDictionary<Pair<DllImportAttribute, Type>, Type> wrapperTypes = new Dictionary<Pair<DllImportAttribute, Type>, Type>();
            Pair<DllImportAttribute, Type> key1 = new Pair<DllImportAttribute, Type> {
                First = new DllImportAttribute("test"),
                Second = typeof(int)
            };
            Pair<DllImportAttribute, Type> key2 = new Pair<DllImportAttribute, Type> {
                First = new DllImportAttribute("test"),
                Second = typeof(int)
            };
            wrapperTypes.Add(key1, typeof(int));
            Assert.IsTrue(wrapperTypes.ContainsKey(key2));
        }
    }
}
