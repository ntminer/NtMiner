using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTMiner.Core {
    public class ShardingHasher {
        public static readonly ShardingHasher Empty = new ShardingHasher(null);

        private readonly SortedDictionary<ulong, string> _circle = new SortedDictionary<ulong, string>();

        public ShardingHasher(string[] nodes) {
            if (nodes != null && nodes.Length != 0) {
                // 每个实节点对应100个节点，其中99个节点是虚节点
                int repeat = 100;
                foreach (var node in nodes) {
                    for (int i = 0; i < repeat; i++) {
                        string identifier = node.GetHashCode().ToString() + "-" + i.ToString();
                        ulong hashCode = Md5Hash(identifier);
                        if (!_circle.ContainsKey(hashCode)) {
                            _circle.Add(hashCode, node);
                        }
                    }
                }
            }
        }

        public string GetTargetNode(Guid guid) {
            return GetTargetNode(guid.ToString());
        }

        public string GetTargetNode(string key) {
            if (_circle.Count == 0) {
                return string.Empty;
            }
            ulong hash = Md5Hash(key);
            ulong firstNode = ModifiedBinarySearch(_circle.Keys.ToArray(), hash);
            return _circle[firstNode];
        }

        /// <summary>
        /// 计算key的数值，得出空间归属。
        /// </summary>
        /// <param name="sortedArray"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private static ulong ModifiedBinarySearch(ulong[] sortedArray, ulong val) {
            int min = 0;
            int max = sortedArray.Length - 1;

            if (val < sortedArray[min] || val > sortedArray[max])
                return sortedArray[0];

            while (max - min > 1) {
                int mid = (max + min) / 2;
                if (sortedArray[mid] >= val) {
                    max = mid;
                }
                else {
                    min = mid;
                }
            }

            return sortedArray[max];
        }

        private static ulong Md5Hash(string key) {
            using (var hash = System.Security.Cryptography.MD5.Create()) {
                byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(key));
                var a = BitConverter.ToUInt64(data, 0);
                var b = BitConverter.ToUInt64(data, 8);
                ulong hashCode = a ^ b;
                return hashCode;
            }
        }
    }
}
