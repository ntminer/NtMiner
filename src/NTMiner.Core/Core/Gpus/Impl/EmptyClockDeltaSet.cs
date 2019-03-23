using NTMiner.MinerClient;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Core.Gpus.Impl {
    public class EmptyClockDeltaSet : IGpuClockDeltaSet {
        private readonly List<IGpuClockDelta> _list = new List<IGpuClockDelta>();

        private readonly INTMinerRoot _root;

        public EmptyClockDeltaSet(INTMinerRoot root) {
            _root = root;
        }

        public bool TryGetValue(int gpuIndex, out IGpuClockDelta data) {
            data = null;
            return false;
        }

        public IEnumerator<IGpuClockDelta> GetEnumerator() {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _list.GetEnumerator();
        }
    }
}
