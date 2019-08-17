using System.Collections.Generic;

namespace NTMiner.Gpus.Nvapi {
    public class NvapiHelper {
        public NvapiHelper() { }

        public Dictionary<int, NvPhysicalGpuHandle> GetNvPhysicalGpuHandles() {
            Dictionary<int, NvPhysicalGpuHandle> dic = new Dictionary<int, NvPhysicalGpuHandle>();
            var gpuHandles = new NvPhysicalGpuHandle[NvapiNativeMethods.MAX_PHYSICAL_GPUS];
            int gpuCount;
            NvapiNativeMethods.NvAPI_EnumPhysicalGPUs(gpuHandles, out gpuCount);
            Write.DevDebug($"NvAPI_EnumPhysicalGPUs:{gpuCount}");
            for (int i = 0; i < gpuCount; i++) {
                NvapiNativeMethods.NvAPI_GPU_GetBusID(gpuHandles[i], out int busId);
                if (!dic.ContainsKey(busId)) {
                    dic.Add(busId, gpuHandles[i]);
                }
            }
            NvapiNativeMethods.NvAPI_EnumTCCPhysicalGPUs(gpuHandles, out gpuCount);
            Write.DevDebug($"NvAPI_EnumTCCPhysicalGPUs:{gpuCount}");
            for (int i = 0; i < gpuCount; i++) {
                NvapiNativeMethods.NvAPI_GPU_GetBusID(gpuHandles[i], out int busId);
                if (!dic.ContainsKey(busId)) {
                    dic.Add(busId, gpuHandles[i]);
                }
            }
            return dic;
        }
    }
}
