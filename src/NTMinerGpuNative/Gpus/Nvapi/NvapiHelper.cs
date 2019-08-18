using System;
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
                handles = new NvPhysicalGpuHandle[NvConst.MAX_PHYSICAL_GPUS];
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

        public void GetClockRangeByIndex(
            int busId,
            out int coreClockMin, out int coreClockMax, out int coreClockDelta,
            out int memoryClockMin, out int memoryClockMax, out int memoryClockDelta,
            out int powerMin, out int powerMax, out int powerDefault, out int powerLimit,
            out int tempLimitMin, out int tempLimitMax, out int tempLimitDefault, out int tempLimit,
            out int fanSpeedMin, out int fanSpeedMax, out int fanSpeedDefault) {
            coreClockMin = 0;
            coreClockMax = 0;
            coreClockDelta = 0;
            memoryClockMin = 0;
            memoryClockMax = 0;
            memoryClockDelta = 0;
            powerMin = 0;
            powerMax = 0;
            powerDefault = 0;
            powerLimit = 0;
            tempLimitMin = 0;
            tempLimitMax = 0;
            tempLimitDefault = 0;
            tempLimit = 0;
            fanSpeedMin = 0;
            fanSpeedMax = 0;
            fanSpeedDefault = 0;
            try {
                if (GetClockDelta(busId, isMemClock: false, out int outCurrFreqDelta, out int outMinFreqDelta, out int outMaxFreqDelta)) {
                    coreClockMin = outMinFreqDelta;
                    coreClockMax = outMaxFreqDelta;
                    coreClockDelta = outCurrFreqDelta;
                }
                if (GetClockDelta(busId, isMemClock: true, out outCurrFreqDelta, out outMinFreqDelta, out outMaxFreqDelta)) {
                    memoryClockMin = outMinFreqDelta;
                    memoryClockMax = outMaxFreqDelta;
                    memoryClockDelta = outCurrFreqDelta;
                }
                if (getPowerLimit(busId, out uint outCurrPower, out uint outMinPower, out uint outDefPower, out uint outMaxPower)) {
                    powerMin = (int)outMinPower;
                    powerMax = (int)outMaxPower;
                    powerDefault = (int)outDefPower;
                    powerLimit = (int)outCurrPower;
                }
                if (getTempLimit(busId, out int outCurrTemp, out int outMinTemp, out int outDefTemp, out int outMaxTemp)) {
                    tempLimitMin = outMinTemp;
                    tempLimitMax = outMaxTemp;
                    tempLimitDefault = outDefTemp;
                    tempLimit = outCurrTemp;
                }
                if (getCooler(busId, out uint currCooler, out uint minCooler, out uint defCooler, out uint maxCooler)) {
                    fanSpeedMin = (int)minCooler;
                    fanSpeedMax = (int)maxCooler;
                    fanSpeedDefault = (int)defCooler;
                }
#if DEBUG
                Write.DevWarn($"GetClockRangeByIndex coreClockMin={coreClockMin},coreClockMax={coreClockMax},coreClockDelta={coreClockDelta},memoryClockMin={memoryClockMin},memoryClockMax={memoryClockMax},memoryClockDelta={memoryClockDelta},powerMin={powerMin},powerMax={powerMax},powerDefault={powerDefault},powerLimit={powerLimit},tempLimitMin={tempLimitMin},tempLimitMax={tempLimitMax},tempLimitDefault={tempLimitDefault},tempLimit={tempLimit},fanSpeedMin={fanSpeedMin},fanSpeedMax={fanSpeedMax},fanSpeedDefault={fanSpeedDefault}");
#endif
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
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
        private bool GetClockDelta(int busId, bool isMemClock, out int outCurrFreqDelta, out int outMinFreqDelta, out int outMaxFreqDelta) {
            outCurrFreqDelta = 0;
            outMinFreqDelta = 0;
            outMaxFreqDelta = 0;
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
            NV_GPU_PERF_PSTATES20_INFO_V1 info = new NV_GPU_PERF_PSTATES20_INFO_V1();
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
            NV_GPU_PERF_PSTATES20_INFO_V2 info = new NV_GPU_PERF_PSTATES20_INFO_V2();
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
            NV_GPU_CLOCK_FREQUENCIES_V2 info = new NV_GPU_CLOCK_FREQUENCIES_V2();
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
        private NVAPI_GPU_THERMAL_INFO nvapi_ClientThermalPoliciesGetInfo(int busId) {
            NVAPI_GPU_THERMAL_INFO info = new NVAPI_GPU_THERMAL_INFO();
            try {
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NVAPI_GPU_THERMAL_INFO))));
                if (NvapiNativeMethods.NvApiClientThermalPoliciesGetInfo(HandlesByBusId[busId], ref info) == NvStatus.OK) {
                    return info;
                }
            }
            catch {
            }
            return info;
        }

        private NVAPI_GPU_THERMAL_LIMIT nvapi_ClientThermalPoliciesGetLimit(int busId) {
            NVAPI_GPU_THERMAL_LIMIT info = new NVAPI_GPU_THERMAL_LIMIT();
            try {
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NVAPI_GPU_THERMAL_LIMIT))));
                if (NvapiNativeMethods.NvApiClientThermalPoliciesGetSetLimit(_handlesByBusId[busId], ref info) == NvStatus.OK) {
                    return info;
                }
            }
            catch {
            }
            return info;
        }

        private bool nvapi_ClientThermalPoliciesSetLimit(int busId, ref NVAPI_GPU_THERMAL_LIMIT info) {
            try {
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NVAPI_GPU_THERMAL_LIMIT))));
                if (NvapiNativeMethods.NvApiClientThermalPoliciesGetSetLimit(HandlesByBusId[busId], ref info) == NvStatus.OK) {
                    return true;
                }
            }
            catch {
            }
            return false;
        }

        public bool nvapi_SetThermalValue(int busId, int value) {
            value = value << 8;
            try {
                NVAPI_GPU_THERMAL_INFO info = nvapi_ClientThermalPoliciesGetInfo(busId);
                if (value == 0)
                    value = info.entries[0].def_temp;
                else if (value > info.entries[0].max_temp)
                    value = info.entries[0].max_temp;
                else if (value < info.entries[0].min_temp)
                    value = info.entries[0].min_temp;

                NVAPI_GPU_THERMAL_LIMIT limit = nvapi_ClientThermalPoliciesGetLimit(busId);
                limit.flags = 1;
                limit.entries[0].value = (uint)value;

                return nvapi_ClientThermalPoliciesSetLimit(busId, ref limit);
            }
            catch {
            }
            return false;
        }

        public bool nvapi_GetThermalInfo(int busId, out double minThermal, out double defThermal, out double maxThermal) {
            minThermal = 0;
            defThermal = 0;
            maxThermal = 0;
            try {
                NVAPI_GPU_THERMAL_INFO info = nvapi_ClientThermalPoliciesGetInfo(busId);
                minThermal = info.entries[0].min_temp / 256.0;
                defThermal = info.entries[0].def_temp / 256.0;
                maxThermal = info.entries[0].max_temp / 256.0;
            }
            catch {
                return false;
            }
            return false;
        }

        public bool nvapi_GetThermalInfo(int busId, out int minThermal, out int defThermal, out int maxThermal) {
            minThermal = 0;
            defThermal = 0;
            maxThermal = 0;
            try {
                NVAPI_GPU_THERMAL_INFO info = nvapi_ClientThermalPoliciesGetInfo(busId);
                minThermal = info.entries[0].min_temp / (1 << 8);
                defThermal = info.entries[0].def_temp / (1 << 8);
                maxThermal = info.entries[0].max_temp / (1 << 8);
            }
            catch {
                return false;
            }
            return false;
        }
        private bool getTempLimit(int busId, out int outCurrTemp, out int outMinTemp, out int outDefTemp, out int outMaxTemp) {
            outCurrTemp = 0;
            outMinTemp = 0;
            outDefTemp = 0;
            outMaxTemp = 0;
            try {
                nvapi_GetThermalInfo(busId, out outMinTemp, out outDefTemp, out outMaxTemp);

                NVAPI_GPU_THERMAL_LIMIT limit = nvapi_ClientThermalPoliciesGetLimit(busId);
                outCurrTemp = (int)(limit.entries[0].value / 256);

                return true;
            }
            catch {
            }
            return false;
        }
        public bool setTempLimit(int busId, int temp) {
            return nvapi_SetThermalValue(busId, temp);
        }
        public bool setDefaultTempLimit(int busId) {
            int currValue = 0;
            int minValue = 0;
            int defValue = 0;
            int maxValue = 0;
            if (getTempLimit(busId, out currValue, out minValue, out defValue, out maxValue)) {
                return setTempLimit(busId, defValue);
            }
            return false;
        }
        private NVAPI_GPU_POWER_STATUS nvapi_ClientPowerPoliciesGetStatus(int busId) {
            NVAPI_GPU_POWER_STATUS info = new NVAPI_GPU_POWER_STATUS();
            try {
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NVAPI_GPU_POWER_STATUS))));
                if (NvapiNativeMethods.NvApiClientPowerPoliciesGetSetStatus(HandlesByBusId[busId], ref info) == NvStatus.OK) {
                    return info;
                }
            }
            catch {
            }
            return info;
        }

        public double nvapi_GetPowerPercent(int busId) {
            try {
                NVAPI_GPU_POWER_STATUS info = nvapi_ClientPowerPoliciesGetStatus(busId);
                return (info.entries[0].power / 1000) / 100.0;
            }
            catch {
            }
            return 1.0;
        }

        public NVAPI_GPU_POWER_INFO nvapi_ClientPowerPoliciesGetInfo(int busId) {
            NVAPI_GPU_POWER_INFO info = new NVAPI_GPU_POWER_INFO();
            try {
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NVAPI_GPU_POWER_INFO))));
                if (NvapiNativeMethods.NvApiClientPowerPoliciesGetInfo(HandlesByBusId[busId], ref info) == NvStatus.OK) {
                    return info;
                }
            }
            catch {
            }
            return info;
        }

        public bool nvapi_ClientPowerPoliciesGetInfo(int busId, out uint minPower, out uint defPower, out uint maxPower) {
            minPower = 0;
            defPower = 0;
            maxPower = 0;
            NVAPI_GPU_POWER_INFO info = nvapi_ClientPowerPoliciesGetInfo(busId);
            if (info.valid == 1 && info.count > 0) {
                minPower = info.entries[0].min_power;
                defPower = info.entries[0].def_power;
                maxPower = info.entries[0].max_power;
                return true;
            }
            return false;
        }

        public bool nvapi_ClientPowerPoliciesSetStatus(int busId, ref NVAPI_GPU_POWER_STATUS info) {
            try {
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NVAPI_GPU_POWER_STATUS))));
                if (NvapiNativeMethods.NvApiClientPowerPoliciesGetSetStatus(HandlesByBusId[busId], ref info) == NvStatus.OK) {
                    return true;
                }
            }
            catch {
            }
            return false;
        }

        public bool nvapi_SetPowerPercent(int busId, double percent) {
            uint percentInt = (uint)(percent * 100) * 1000;
            return nvapi_SetPowerValue(busId, percentInt);
        }

        public bool nvapi_SetPowerValue(int busId, uint percentInt) {
            uint minPower = 0, defPower = 0, maxPower = 0;

            try {
                if (nvapi_ClientPowerPoliciesGetInfo(busId, out minPower, out defPower, out maxPower)) {
                    if (percentInt == 0)
                        percentInt = defPower;
                    else if (percentInt < minPower)
                        percentInt = minPower;
                    else if (percentInt > maxPower)
                        percentInt = maxPower;

                    NVAPI_GPU_POWER_STATUS info = nvapi_ClientPowerPoliciesGetStatus(busId);
                    info.flags = 1;
                    info.entries[0].power = percentInt;
                    return nvapi_ClientPowerPoliciesSetStatus(busId, ref info);
                }
            }
            catch {
            }
            return false;
        }

        public bool getPowerLimit(int busId, out uint outCurrPower, out uint outMinPower, out uint outDefPower, out uint outMaxPower) {
            outCurrPower = 0;
            outMinPower = 0;
            outDefPower = 0;
            outMaxPower = 0;
            try {
                nvapi_ClientPowerPoliciesGetInfo(busId, out outMinPower, out outDefPower, out outMaxPower);

                NVAPI_GPU_POWER_STATUS info = nvapi_ClientPowerPoliciesGetStatus(busId);
                outCurrPower = info.entries[0].power;

                outCurrPower = outCurrPower / 1000;
                outMinPower = outMinPower / 1000;
                outDefPower = outDefPower / 1000;
                outMaxPower = outMaxPower / 1000;

                return true;
            }
            catch {
            }
            return false;
        }
        public bool setPowerLimit(int busId, uint powerValue) {
            powerValue *= 1000;
            return nvapi_SetPowerValue(busId, powerValue);
        }
        public bool setDefaultPowerLimit(int busId) {
            uint currPower;
            uint minPower;
            uint defPower;
            uint maxPower;
            if (getPowerLimit(busId, out currPower, out minPower, out defPower, out maxPower)) {
                return nvapi_SetPowerValue(busId, defPower * 1000);
            }
            return false;
        }
        public NVAPI_COOLER_SETTINGS nvapi_GetCoolerSettings(int busId) {
            NVAPI_COOLER_SETTINGS info = new NVAPI_COOLER_SETTINGS();
            try {
                NV_COOLER_TARGET cmd = NV_COOLER_TARGET.NVAPI_COOLER_TARGET_ALL;
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NVAPI_COOLER_SETTINGS))));
                if (NvapiNativeMethods.NvApiGetCoolerSettings(HandlesByBusId[busId], cmd, ref info) == NvStatus.OK) {
                    return info;
                }
            }
            catch {
            }
            return info;
        }

        public bool nvapi_GetCoolerSettings(int busId, ref uint minCooler, ref uint currCooler, ref uint maxCooler) {
            try {
                NVAPI_COOLER_SETTINGS info = nvapi_GetCoolerSettings(busId);
                if (info.count > 0) {
                    minCooler = info.cooler[0].currentMinLevel;
                    currCooler = info.cooler[0].currentLevel;
                    maxCooler = info.cooler[0].currentMaxLevel;
                    return true;
                }
            }
            catch {
            }
            return false;
        }

        public bool nvapi_IsCoolerAuto(int busId) {
            try {
                NVAPI_COOLER_SETTINGS info = nvapi_GetCoolerSettings(busId);
                if (info.count > 0) {
                    return info.cooler[0].currentPolicy != NV_COOLER_POLICY.NVAPI_COOLER_POLICY_MANUAL;
                }
            }
            catch {
            }
            return false;
        }

        public bool nvapi_SetCoolerLevels(int busId, NV_COOLER_TARGET coolerIndex, ref NVAPI_COOLER_LEVEL level) {
            try {
                level.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NVAPI_COOLER_LEVEL))));
                if (NvapiNativeMethods.NvApiSetCoolerLevels(HandlesByBusId[busId], coolerIndex, ref level) == NvStatus.OK) {
                    return true;
                }
            }
            catch {
            }
            return false;
        }
        private static T allocStruct<T>() {
            int size = Marshal.SizeOf(typeof(T));
            byte[] bytes = new byte[size];
            Array.Clear(bytes, 0, size);
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            object obj = Marshal.PtrToStructure(structPtr, typeof(T));
            Marshal.FreeHGlobal(structPtr);
            return (T)obj;
        }

        public bool nvapi_SetCoolerLevels(int busId, uint value) {
            NV_COOLER_TARGET coolerIndex = NV_COOLER_TARGET.NVAPI_COOLER_TARGET_ALL;
            try {
                NVAPI_COOLER_LEVEL level = allocStruct<NVAPI_COOLER_LEVEL>();
                level.coolers[0].currentLevel = value;
                level.coolers[0].currentPolicy = NV_COOLER_POLICY.NVAPI_COOLER_POLICY_MANUAL;

                return nvapi_SetCoolerLevels(busId, coolerIndex, ref level);
            }
            catch {
            }
            return false;
        }

        public bool nvapi_SetCoolerLevelsAuto(int busId) {
            NV_COOLER_TARGET coolerIndex = NV_COOLER_TARGET.NVAPI_COOLER_TARGET_ALL;
            try {
                NVAPI_COOLER_LEVEL level = allocStruct<NVAPI_COOLER_LEVEL>();
                level.coolers[0].currentLevel = 0;
                level.coolers[0].currentPolicy = NV_COOLER_POLICY.NVAPI_COOLER_POLICY_AUTO;
                return nvapi_SetCoolerLevels(busId, coolerIndex, ref level);
            }
            catch {
            }
            return false;
        }

        public bool getCooler(int busId, out uint currCooler, out uint minCooler, out uint defCooler, out uint maxCooler) {
            currCooler = 0;
            minCooler = 0;
            defCooler = 0;
            maxCooler = 0;
            try {
                nvapi_GetCoolerSettings(busId, ref minCooler, ref currCooler, ref maxCooler);
                defCooler = minCooler;
                return true;
            }
            catch {
            }
            return false;
        }


        public bool isCoolerAuto(int busId) {
            try {
                return nvapi_IsCoolerAuto(busId);
            }
            catch {
            }
            return false;
        }

        public bool setCooler(int busId, uint value, bool isAutoMode) {
            if (isAutoMode) {
                return nvapi_SetCoolerLevelsAuto(busId);
            }
            else {
                return nvapi_SetCoolerLevels(busId, value);
            }
        }
        public bool setDefaultCooler(int busId) {
            return setCooler(busId, 0, true);
        }
        #endregion
    }
}
