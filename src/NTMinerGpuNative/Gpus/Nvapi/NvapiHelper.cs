using NTMiner.Gpus.Nvapi.Native;
using NTMiner.Gpus.Nvapi.Native.GPU.Structures;
using System.Collections.Generic;

namespace NTMiner.Gpus.Nvapi {
    public class NvapiHelper {
        public NvapiHelper() { }

        public Dictionary<int, PhysicalGPUHandle> GetNvPhysicalGpuHandles() {
            Dictionary<int, PhysicalGPUHandle> dic = new Dictionary<int, PhysicalGPUHandle>();
            var handles = GPUApi.EnumPhysicalGPUs();
            Write.DevDebug($"EnumPhysicalGPUs:{handles.Length}");
            foreach (var handle in handles) {
                int busId = GPUApi.GetBusId(handle);
                if (!dic.ContainsKey(busId)) {
                    dic.Add(busId, handle);
                }
            }
            handles = GPUApi.EnumTCCPhysicalGPUs();
            Write.DevDebug($"EnumTCCPhysicalGPUs:{handles.Length}");
            foreach (var handle in handles) {
                int busId = GPUApi.GetBusId(handle);
                if (!dic.ContainsKey(busId)) {
                    dic.Add(busId, handle);
                }
            }
            return dic;
        }
    }
}
