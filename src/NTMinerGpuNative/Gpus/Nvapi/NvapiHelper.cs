using System.Collections.Generic;

namespace NTMiner.Gpus.Nvapi {
    public class NvapiHelper {
        public NvapiHelper() { }

        public NvPhysicalGpuHandle[] GetNvPhysicalGpuHandles() {
            List<NvPhysicalGpuHandle> list = new List<NvPhysicalGpuHandle>();
            var gpuHandles = new NvPhysicalGpuHandle[NvapiNativeMethods.MAX_PHYSICAL_GPUS];
            int gpuCount;
            NvapiNativeMethods.NvAPI_EnumPhysicalGPUs(gpuHandles, out gpuCount);
            Write.DevDebug($"NvAPI_EnumPhysicalGPUs:{gpuCount}");
            for (int i = 0; i < gpuCount; i++) {
                list.Add(gpuHandles[i]);
            }
            NvapiNativeMethods.NvAPI_EnumTCCPhysicalGPUs(gpuHandles, out gpuCount);
            Write.DevDebug($"NvAPI_EnumTCCPhysicalGPUs:{gpuCount}");
            for (int i = 0; i < gpuCount; i++) {
                list.Add(gpuHandles[i]);
            }
            return list.ToArray();
        }
    }
}
