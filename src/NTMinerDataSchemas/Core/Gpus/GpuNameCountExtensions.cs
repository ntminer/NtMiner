using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Gpus {
    public static class GpuNameCountExtensions {
        public static IGpuName GetMatchGpuName(this IGpuNameCount gpuNameCount, IEnumerable<IGpuName> gpuNames) {
            if (gpuNames == null) {
                return null;
            }
            gpuNames = gpuNames.OrderByDescending(a => a.Name);
            foreach (var gpuName in gpuNames) {
                if (GpuName.ConvertToGb(gpuName.TotalMemory) == GpuName.ConvertToGb(gpuNameCount.TotalMemory) 
                    && gpuNameCount.Name.IndexOf(gpuName.Name, StringComparison.OrdinalIgnoreCase) != -1) {
                    return gpuName;
                }
            }
            return null;
        }
    }
}
