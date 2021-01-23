using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner {
    [TestClass]
    public class ShardingHasherTests {
        [TestMethod]
        public void Test1() {
            var nodes = new string[] { "1.1.1.1", "2.2.2.2", "3.3.3.3", "4.4.4.4", "5.5.5.5" };
            ShardingHasher shardingHasher = new ShardingHasher(nodes);
            Dictionary<string, int> counts = new Dictionary<string, int>();
            for (int i = 0; i < 10000; i++) {
                string nodeAddress = shardingHasher.GetTargetNode(Guid.NewGuid());
                if (counts.TryGetValue(nodeAddress, out int count)) {
                    counts[nodeAddress]++;
                }
                else {
                    counts.Add(nodeAddress, 1);
                }
            }
            foreach (var item in counts) {
                Console.WriteLine($"key={item.Key}, value={item.Value.ToString()}");
            }
        }
    }
}
