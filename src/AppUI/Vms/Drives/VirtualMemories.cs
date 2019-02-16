using System;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Vms {
    public class VirtualMemories : IEnumerable<VirtualMemory> {
        public static readonly VirtualMemories Instance = new VirtualMemories();

        private readonly Dictionary<string, VirtualMemory> _dic = new Dictionary<string, VirtualMemory>(StringComparer.OrdinalIgnoreCase);

        private VirtualMemories() { }

        public void Clear() {
            _dic.Clear();
        }

        public void Add(string driveName, VirtualMemory item) {
            if (!_dic.ContainsKey(driveName)) {
                _dic.Add(driveName, item);
            }
        }

        public VirtualMemory this[string driveName] {
            get {
                return _dic[driveName];
            }
        }

        public bool Contains(string driveName) {
            return _dic.ContainsKey(driveName);
        }

        public IEnumerator<VirtualMemory> GetEnumerator() {
            return _dic.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _dic.Values.GetEnumerator();
        }
    }
}
