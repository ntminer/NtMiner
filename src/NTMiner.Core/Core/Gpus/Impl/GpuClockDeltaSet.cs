using System.Collections.Generic;

namespace NTMiner.Core.Gpus.Impl {
    public class GpuClockDeltaSet : IGpuClockDeltaSet {
        private readonly Dictionary<int, GpuClockDelta> _dicByGpuIndex = new Dictionary<int, GpuClockDelta>();

        private readonly INTMinerRoot _root;

        public GpuClockDeltaSet(INTMinerRoot root) {
            _root = root;
        }

        private bool _isInited = false;
        private object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    
                    _isInited = true;
                }
            }
        }

        public bool TryGetValue(int gpuIndex, out IGpuClockDelta data) {
            InitOnece();
            GpuClockDelta value;
            bool result = _dicByGpuIndex.TryGetValue(gpuIndex, out value);
            data = value;
            return result;
        }
    }
}
