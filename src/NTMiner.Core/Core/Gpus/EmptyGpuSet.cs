using NTMiner.Core.Gpus.Impl;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Core.Gpus {
    public class EmptyGpuSet : IGpuSet {
        public static readonly EmptyGpuSet Instance = new EmptyGpuSet();

        private List<IGpu> _list = new List<IGpu> {
            Gpu.GpuAll
        };

        private EmptyGpuSet() {
            this.Properties = new List<GpuSetProperty>();
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

        public IOverClock OverClock { get; private set; } = new EmptyOverClock();

        public string DriverVersion {
            get { return "0.0"; }
        }

        public string GetProperty(string key) {
            return string.Empty;
        }

        public void LoadGpuState() {
            // nothing need todo
        }

        public IEnumerator<IGpu> GetEnumerator() {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _list.GetEnumerator();
        }
    }
}
