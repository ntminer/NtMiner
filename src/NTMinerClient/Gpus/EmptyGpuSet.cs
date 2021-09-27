using NTMiner.Gpus.Impl;
using System;
using System.Collections.Generic;

namespace NTMiner.Gpus {
    public class EmptyGpuSet : IGpuSet {
        public static EmptyGpuSet Instance { get; private set; } = new EmptyGpuSet();

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

        public bool IsLowDriverVersion {
            get {
                return true;
            }
        }

        public DateTime HighTemperatureOn { get; set; }

        public DateTime LowTemperatureOn { get; set; }

        public void LoadGpuState() {
            // nothing need todo
        }

        public void LoadGpuState(int gpuIndex) {
            // nothing need todo
        }

        public IEnumerable<IGpu> AsEnumerable() {
            return _list;
        }
    }
}
