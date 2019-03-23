using NTMiner.MinerClient;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Core.Gpus.Impl {
    public class AMDClockDeltaSet : IGpuClockDeltaSet {
        private readonly Dictionary<int, GpuClockDelta> _dicByGpuIndex = new Dictionary<int, GpuClockDelta>();

        private readonly IGpuSet _gpuSet;

        public AMDClockDeltaSet(IGpuSet gpuSet) {
            _gpuSet = gpuSet;
        }

        private bool _isInited = false;
        private readonly object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    foreach (var gpu in _gpuSet) {
                        if (gpu.Index == NTMinerRoot.GpuAllId) {
                            continue;
                        }
                        _dicByGpuIndex.Add(gpu.Index, GpuClockDelta.Empty);
                    }
                    _isInited = true;
                }
            }
        }

        public bool TryGetValue(int gpuIndex, out IGpuClockDelta data) {
            InitOnece();
            bool result = _dicByGpuIndex.TryGetValue(gpuIndex, out GpuClockDelta value);
            data = value;
            return result;
        }

        public IEnumerator<IGpuClockDelta> GetEnumerator() {
            InitOnece();
            return _dicByGpuIndex.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicByGpuIndex.Values.GetEnumerator();
        }
    }
}
