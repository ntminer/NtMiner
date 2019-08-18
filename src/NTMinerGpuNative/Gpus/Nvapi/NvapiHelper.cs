using System.Collections.Generic;

namespace NTMiner.Gpus.Nvapi {
    public class NvapiHelper {
        public NvapiHelper() { }

        private static Dictionary<int, NvPhysicalGpuHandle> _nvapiHandleDicByBusId = null;
        internal Dictionary<int, NvPhysicalGpuHandle> NvapiHandleDicByBusId {
            get {
                if (_nvapiHandleDicByBusId != null) {
                    return _nvapiHandleDicByBusId;
                }
                _nvapiHandleDicByBusId = new Dictionary<int, NvPhysicalGpuHandle>();
                var handles = new NvPhysicalGpuHandle[NvapiNativeMethods.MAX_PHYSICAL_GPUS];
                NvapiNativeMethods.NvAPI_EnumPhysicalGPUs(handles, out int gpuCount);
                for (int i = 0; i < gpuCount; i++) {
                    NvapiNativeMethods.NvAPI_GPU_GetBusID(handles[i], out int busId);
                    if (!_nvapiHandleDicByBusId.ContainsKey(busId)) {
                        _nvapiHandleDicByBusId.Add(busId, handles[i]);
                    }
                }
                NvapiNativeMethods.NvAPI_EnumTCCPhysicalGPUs(handles, out gpuCount);
                for (int i = 0; i < gpuCount; i++) {
                    NvapiNativeMethods.NvAPI_GPU_GetBusID(handles[i], out int busId);
                    if (!_nvapiHandleDicByBusId.ContainsKey(busId)) {
                        _nvapiHandleDicByBusId.Add(busId, handles[i]);
                    }
                }
                return _nvapiHandleDicByBusId;
            }
        }
    }
}
