using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NTMiner.Gpus.Nvapi {
    public class NvapiHelper {
        public NvapiHelper() { }

        private static Dictionary<int, NvPhysicalGpuHandle> _handlesByBusId = null;
        internal Dictionary<int, NvPhysicalGpuHandle> HandlesByBusId {
            get {
                if (_handlesByBusId != null) {
                    return _handlesByBusId;
                }
                _handlesByBusId = new Dictionary<int, NvPhysicalGpuHandle>();
                var handles = new NvPhysicalGpuHandle[NvConst.MAX_PHYSICAL_GPUS];
                NvapiNativeMethods.NvAPI_EnumPhysicalGPUs(handles, out int gpuCount);
                for (int i = 0; i < gpuCount; i++) {
                    NvapiNativeMethods.NvAPI_GPU_GetBusID(handles[i], out int busId);
                    if (!_handlesByBusId.ContainsKey(busId)) {
                        _handlesByBusId.Add(busId, handles[i]);
                    }
                }
                NvapiNativeMethods.NvAPI_EnumTCCPhysicalGPUs(handles, out gpuCount);
                for (int i = 0; i < gpuCount; i++) {
                    NvapiNativeMethods.NvAPI_GPU_GetBusID(handles[i], out int busId);
                    if (!_handlesByBusId.ContainsKey(busId)) {
                        _handlesByBusId.Add(busId, handles[i]);
                    }
                }
                return _handlesByBusId;
            }
        }

        public uint GetCoreClockBaseFreq(int busId) {
            return nvapi_NvGetCoreAndMemClockFreq(busId, (uint)NV_GPU_PUBLIC_CLOCK_ID.NVAPI_GPU_PUBLIC_CLOCK_GRAPHICS, NvConst.NV_GPU_CLOCK_FREQUENCIES_BASE_CLOCK);
        }
        public uint GetMemClockBaseFreq(int busId) {
            return nvapi_NvGetCoreAndMemClockFreq(busId, (uint)NV_GPU_PUBLIC_CLOCK_ID.NVAPI_GPU_PUBLIC_CLOCK_MEMORY, NvConst.NV_GPU_CLOCK_FREQUENCIES_BASE_CLOCK);
        }
        public uint GetCoreClockBoostFreq(int busId) {
            return nvapi_NvGetCoreAndMemClockFreq(busId, (uint)NV_GPU_PUBLIC_CLOCK_ID.NVAPI_GPU_PUBLIC_CLOCK_GRAPHICS, NvConst.NV_GPU_CLOCK_FREQUENCIES_BOOST_CLOCK);
        }
        public uint GetMemClockBoostFreq(int busId) {
            return nvapi_NvGetCoreAndMemClockFreq(busId, (uint)NV_GPU_PUBLIC_CLOCK_ID.NVAPI_GPU_PUBLIC_CLOCK_MEMORY, NvConst.NV_GPU_CLOCK_FREQUENCIES_BOOST_CLOCK);
        }


        public uint GetCoreClockFreq(int busId) {
            return nvapi_NvGetCoreAndMemClockFreq(busId, (uint)NV_GPU_PUBLIC_CLOCK_ID.NVAPI_GPU_PUBLIC_CLOCK_GRAPHICS, NvConst.NV_GPU_CLOCK_FREQUENCIES_CURRENT_FREQ);
        }
        public uint GetMemClockFreq(int busId) {
            return nvapi_NvGetCoreAndMemClockFreq(busId, (uint)NV_GPU_PUBLIC_CLOCK_ID.NVAPI_GPU_PUBLIC_CLOCK_MEMORY, NvConst.NV_GPU_CLOCK_FREQUENCIES_CURRENT_FREQ);
        }

        public bool SetPstatesV2_CoreClock(int busId, int kHz) {
            try {
                NV_GPU_PERF_PSTATES20_INFO_V2 info = nvapi_NvGetPstatesV2(busId);
                info.numPstates = 1;
                info.numClocks = 1;
                info.pstates[0].clocks[0].domainId = NV_GPU_PUBLIC_CLOCK_ID.NVAPI_GPU_PUBLIC_CLOCK_GRAPHICS;
                info.pstates[0].clocks[0].freqDelta_kHz.value = kHz;
                return nvapi_NvSetPstatesV2(busId, ref info);
            }
            catch {
            }
            return false;
        }

        public bool SetPstatesV2_MemClock(int busId, int kHz) {
            try {
                NV_GPU_PERF_PSTATES20_INFO_V2 info = nvapi_NvGetPstatesV2(busId);
                info.numPstates = 1;
                info.numClocks = 1;
                info.numBaseVoltages = 0;

                info.pstates[0].clocks[0].domainId = NV_GPU_PUBLIC_CLOCK_ID.NVAPI_GPU_PUBLIC_CLOCK_MEMORY;
                info.pstates[0].clocks[0].typeId = NV_GPU_PERF_PSTATE20_CLOCK_TYPE_ID.NVAPI_GPU_PERF_PSTATE20_CLOCK_TYPE_SINGLE;
                info.pstates[0].clocks[0].freqDelta_kHz.value = kHz;

                return nvapi_NvSetPstatesV2(busId, ref info);
            }
            catch {
            }
            return false;
        }

        #region private methods
        private bool GetClockDelta(int busId, bool isMemClock, ref int outCurrFreqDelta, ref int outMinFreqDelta, ref int outMaxFreqDelta) {
            bool hasSetValue = false;
            NV_GPU_PUBLIC_CLOCK_ID clockType;
            if (isMemClock) {
                clockType = NV_GPU_PUBLIC_CLOCK_ID.NVAPI_GPU_PUBLIC_CLOCK_MEMORY;
            }
            else {
                clockType = NV_GPU_PUBLIC_CLOCK_ID.NVAPI_GPU_PUBLIC_CLOCK_GRAPHICS;
            }

            try {
                NV_GPU_PERF_PSTATES20_INFO_V2 info = nvapi_NvGetPstatesV2(busId);

                for (int i = 0; i < info.numPstates; i++) {
                    for (int j = 0; j < info.numClocks; j++) {
                        uint min = info.pstates[i].clocks[j].data.minFreq_kHz;
                        uint max = info.pstates[i].clocks[j].data.maxFreq_kHz;
                        if (info.pstates[i].clocks[j].domainId == clockType && min > 0 && max > 0) {

                            outCurrFreqDelta = info.pstates[i].clocks[j].freqDelta_kHz.value;
                            outMinFreqDelta = info.pstates[i].clocks[j].freqDelta_kHz.mindelta;
                            outMaxFreqDelta = info.pstates[i].clocks[j].freqDelta_kHz.maxdelta;
                            hasSetValue = true;
                            return hasSetValue;
                        }
                    }
                }
            }
            catch {
            }

            return hasSetValue;
        }

        private const int VERSION1 = 1 << 16;
        private const int VERSION2 = 2 << 16;
        private NV_GPU_PERF_PSTATES20_INFO_V1 nvapi_NvGetPstatesV1(int busId) {
            NV_GPU_PERF_PSTATES20_INFO_V1 info = default;
            try {
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NV_GPU_PERF_PSTATES20_INFO_V1))));
                if (NvapiNativeMethods.NvSetGetPStateV1(HandlesByBusId[busId], ref info) == NvStatus.OK) {
                    return info;
                }
            }
            catch {
            }
            return info;
        }

        private NV_GPU_PERF_PSTATES20_INFO_V2 nvapi_NvGetPstatesV2(int busId) {
            NV_GPU_PERF_PSTATES20_INFO_V2 info = default;
            try {
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NV_GPU_PERF_PSTATES20_INFO_V2))));
                if (NvapiNativeMethods.NvSetGetPStateV2(_handlesByBusId[busId], ref info) == NvStatus.OK) {
                    return info;
                }
            }
            catch {
            }
            return info;
        }

        private bool nvapi_NvSetPstatesV2(int busId, ref NV_GPU_PERF_PSTATES20_INFO_V2 info) {
            try {
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NV_GPU_PERF_PSTATES20_INFO_V2))));
                if (NvapiNativeMethods.NvSetGetPStateV2(_handlesByBusId[busId], ref info) == NvStatus.OK) {
                    return true;
                }
            }
            catch {
            }
            return false;
        }
        private bool nvapi_NvSetPstatesV1(int busId, ref NV_GPU_PERF_PSTATES20_INFO_V1 info) {
            try {
                int len = Marshal.SizeOf(typeof(NV_GPU_PERF_PSTATES20_INFO_V1));
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NV_GPU_PERF_PSTATES20_INFO_V1))));
                if (NvapiNativeMethods.NvSetGetPStateV1(_handlesByBusId[busId], ref info) == NvStatus.OK) {
                    return true;
                }
            }
            catch {
            }
            return false;
        }
        
        private NV_GPU_CLOCK_FREQUENCIES_V2 nvapi_NvGetAllClockFrequenciesV2(int busId, uint type) {
            NV_GPU_CLOCK_FREQUENCIES_V2 info = default;
            try {
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NV_GPU_CLOCK_FREQUENCIES_V2))));
                info.ClockType = type;
                if (NvapiNativeMethods.NvGetAllClockFrequenciesV2(HandlesByBusId[busId], ref info) == NvStatus.OK) {
                    return info;
                }
            }
            catch {
            }
            return info;
        }

        private uint nvapi_NvGetCoreAndMemClockFreq(int busId, uint clockId, uint clockType) {
            try {
                NV_GPU_CLOCK_FREQUENCIES_V2 info = nvapi_NvGetAllClockFrequenciesV2(busId, clockType);
                if (clockId < info.domain.Length) {
                    NV_GPU_CLOCK_FREQUENCIES_DOMAIN domain = info.domain[clockId];
                    if (domain.bIsPresent == 1 && info.ClockType == clockType) {
                        return domain.frequency;
                    }
                }
            }
            catch {
            }
            return 0;
        }
        #endregion
    }
}
