using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Core.Gpus.Impl {
    public class AMDClockDeltaSet : IGpuClockDeltaSet {
        private readonly Dictionary<int, IGpuClockDelta> _dicByGpuIndex = new Dictionary<int, IGpuClockDelta>();

        private readonly INTMinerRoot _root;

        public AMDClockDeltaSet(INTMinerRoot root) {
            _root = root;
        }

        public bool TryGetValue(int gpuIndex, out IGpuClockDelta data) {
            data = null;
            return false;
        }

        public IEnumerator<IGpuClockDelta> GetEnumerator() {
            return _dicByGpuIndex.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _dicByGpuIndex.Values.GetEnumerator();
        }
    }
}
