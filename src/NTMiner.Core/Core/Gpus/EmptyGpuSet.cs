using NTMiner.Core.Gpus.Impl;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Core.Gpus {
    public class EmptyGpuSet : IGpuSet {
        public static readonly EmptyGpuSet Instance = new EmptyGpuSet();

        private List<IGpu> _list = new List<IGpu> {
            new Gpu {
                Index = NTMinerRoot.GpuAllId,
                Name = "全部显卡",
                Temperature = 0,
                FanSpeed = 0,
                PowerUsage = 0,
                CoreClockDelta = 0,
                MemoryClockDelta = 0,
                GpuClockDelta = new GpuClockDelta(0, 0, 0, 0),
                OverClock = new EmptyOverClock()
            }
        };

        private EmptyGpuSet() {

        }

        private IGpuClockDeltaSet _gpuClockDeltaSet;
        public IGpuClockDeltaSet GpuClockDeltaSet {
            get {
                if (_gpuClockDeltaSet == null) {
                    _gpuClockDeltaSet = new EmptyClockDeltaSet(NTMinerRoot.Current);
                }
                return _gpuClockDeltaSet;
            }
        }

        public IGpu this[int index] {
            get {
                return _list[0];
            }
        }

        public GpuType GpuType {
            get {
                return GpuType.Empty;
            }
        }

        public int Count {
            get {
                return 0;
            }
        }

        public bool TryGetGpu(int index, out IGpu gpu) {
            gpu = null;
            return false;
        }

        public List<GpuSetProperty> Properties { get; private set; } = new List<GpuSetProperty>();

        public IEnumerator<IGpu> GetEnumerator() {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _list.GetEnumerator();
        }
    }
}
