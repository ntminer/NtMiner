using NTMiner.Core.Gpus;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class GpuNameSet : IGpuNameSet {
        private readonly HashSet<GpuName> _hashSet = new HashSet<GpuName>();

        public GpuNameSet() {
        }

        public void Add(GpuName gpuName) {
            if (gpuName == null) {
                return;
            }
            _hashSet.Add(gpuName);
        }

        public IEnumerable<IGpuName> AsEnumerable() {
            return _hashSet;
        }
    }
}
