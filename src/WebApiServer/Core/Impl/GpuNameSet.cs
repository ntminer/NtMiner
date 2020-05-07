using NTMiner.Core.Gpus;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class GpuNameSet : IGpuNameSet {
        private readonly Dictionary<string, GpuNameCount> _dic = new Dictionary<string, GpuNameCount>(StringComparer.OrdinalIgnoreCase);

        public GpuNameSet() {
        }

        public void Add(GpuType gpuType, string gpuName, ulong gpuTotalMemory) {
            if (gpuType == GpuType.Empty || string.IsNullOrEmpty(gpuName) || !GpuName.IsValidTotalMemory(gpuTotalMemory)) {
                return;
            }
            string key = GpuName.Format(gpuType, gpuName, gpuTotalMemory);
            if (_dic.TryGetValue(key, out GpuNameCount gpuNameCount)) {
                gpuNameCount.Count++;
            }
            else {
                gpuNameCount = GpuNameCount.Create(new GpuName {
                    GpuType = gpuType,
                    Name = gpuName,
                    TotalMemory = gpuTotalMemory
                });
                _dic.Add(key, gpuNameCount);
            }
        }

        public IEnumerable<GpuNameCount> AsEnumerable() {
            return _dic.Values;
        }
    }
}
