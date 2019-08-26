using NTMiner.Gpus.Nvapi;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NTMiner.Gpus {
    public class NvapiHelper {
        private const int VERSION1 = 1 << 16;
        private const int VERSION2 = 2 << 16;

        public NvapiHelper() { }

        private static Dictionary<int, NvPhysicalGpuHandle> _handlesByBusId = null;
        private static readonly object _locker = new object();
        private static Dictionary<int, NvPhysicalGpuHandle> HandlesByBusId {
            get {
                if (_handlesByBusId != null) {
                    return _handlesByBusId;
                }
                lock (_locker) {
                    if (_handlesByBusId != null) {
                        return _handlesByBusId;
                    }
                    _handlesByBusId = new Dictionary<int, NvPhysicalGpuHandle>();
                    var handles = new NvPhysicalGpuHandle[NvapiConst.MAX_PHYSICAL_GPUS];
                    var r = NvapiNativeMethods.NvEnumPhysicalGPUs(handles, out int gpuCount);
                    if (r != NvStatus.OK) {
                        Write.DevError($"{nameof(NvapiNativeMethods.NvEnumPhysicalGPUs)} {r}");
                    }
                    for (int i = 0; i < gpuCount; i++) {
                        r = NvapiNativeMethods.NvGetBusID(handles[i], out int busId);
                        if (r != NvStatus.OK) {
                            Write.DevError($"{nameof(NvapiNativeMethods.NvGetBusID)} {r}");
                        }
                        if (!_handlesByBusId.ContainsKey(busId)) {
                            _handlesByBusId.Add(busId, handles[i]);
                        }
                    }
                    handles = new NvPhysicalGpuHandle[NvapiConst.MAX_PHYSICAL_GPUS];
                    r = NvapiNativeMethods.NvEnumTCCPhysicalGPUs(handles, out gpuCount);
                    if (r != NvStatus.OK) {
                        Write.DevError($"{nameof(NvapiNativeMethods.NvEnumTCCPhysicalGPUs)} {r}");
                    }
                    for (int i = 0; i < gpuCount; i++) {
                        r = NvapiNativeMethods.NvGetBusID(handles[i], out int busId);
                        if (r != NvStatus.OK) {
                            Write.DevError($"{nameof(NvapiNativeMethods.NvGetBusID)} {r}");
                        }
                        if (!_handlesByBusId.ContainsKey(busId)) {
                            _handlesByBusId.Add(busId, handles[i]);
                        }
                    }
                    return _handlesByBusId;
                }
            }
        }

        public void GetClockRange(
            int busId,
            out int coreClockMin, out int coreClockMax, out int coreClockDelta,
            out int memoryClockMin, out int memoryClockMax, out int memoryClockDelta,
            out int powerMin, out int powerMax, out int powerDefault, out int powerLimit,
            out int tempLimitMin, out int tempLimitMax, out int tempLimitDefault, out int tempLimit,
            out uint fanSpeedCurr, out int fanSpeedMin, out int fanSpeedMax, out int fanSpeedDefault) {
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
            fanSpeedCurr = 0;
            fanSpeedMin = 0;
            fanSpeedMax = 100;
            fanSpeedDefault = 0;
            try {
                if (GetClockDelta(busId, 
                    out int outCoreCurrFreqDelta, out int outCoreMinFreqDelta, out int outCoreMaxFreqDelta,
                    out int outMemoryCurrFreqDelta, out int outMemoryMinFreqDelta, out int outMemoryMaxFreqDelta)) {
                    coreClockMin = outCoreMinFreqDelta;
                    coreClockMax = outCoreMaxFreqDelta;
                    coreClockDelta = outCoreCurrFreqDelta;
                    memoryClockMin = outMemoryMinFreqDelta;
                    memoryClockMax = outMemoryMaxFreqDelta;
                    memoryClockDelta = outMemoryCurrFreqDelta;
#if DEBUG
                Write.DevWarn($"{nameof(GetClockDelta)} coreClockMin={coreClockMin},coreClockMax={coreClockMax},coreClockDelta={coreClockDelta},memoryClockMin={memoryClockMin},memoryClockMax={memoryClockMax},memoryClockDelta={memoryClockDelta}");
#endif
                }

                if (GetPowerLimit(busId, out uint outCurrPower, out uint outMinPower, out uint outDefPower, out uint outMaxPower)) {
                    powerMin = (int)outMinPower;
                    powerMax = (int)outMaxPower;
                    powerDefault = (int)outDefPower;
                    powerLimit = (int)outCurrPower;
#if DEBUG
                    Write.DevWarn($"{nameof(GetPowerLimit)} powerMin={powerMin},powerMax={powerMax},powerDefault={powerDefault},powerLimit={powerLimit}");
#endif
                }

                if (GetTempLimit(busId, out int outCurrTemp, out int outMinTemp, out int outDefTemp, out int outMaxTemp)) {
                    tempLimitMin = outMinTemp;
                    tempLimitMax = outMaxTemp;
                    tempLimitDefault = outDefTemp;
                    tempLimit = outCurrTemp;
#if DEBUG
                    Write.DevWarn($"{nameof(GetTempLimit)} tempLimitMin={tempLimitMin},tempLimitMax={tempLimitMax},tempLimitDefault={tempLimitDefault},tempLimit={tempLimit}");
#endif
                }

                if (GetCooler(busId, out uint currCooler, out uint minCooler, out uint defCooler, out uint maxCooler)) {
                    fanSpeedCurr = currCooler;
                    fanSpeedMin = (int)minCooler;
                    fanSpeedMax = (int)maxCooler;
                    fanSpeedDefault = (int)defCooler;
#if DEBUG
                    Write.DevWarn($"{nameof(GetCooler)} fanSpeedCurr={fanSpeedCurr},fanSpeedMin={fanSpeedMin},fanSpeedMax={fanSpeedMax},fanSpeedDefault={fanSpeedDefault}");
#endif
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        public uint GetCoreClockBaseFreq(int busId) {
            return NvGetAllClockFrequenciesV2(busId, (uint)NvGpuPublicClockId.NVAPI_GPU_PUBLIC_CLOCK_GRAPHICS, NvapiConst.NV_GPU_CLOCK_FREQUENCIES_BASE_CLOCK);
        }
        public uint GetMemClockBaseFreq(int busId) {
            return NvGetAllClockFrequenciesV2(busId, (uint)NvGpuPublicClockId.NVAPI_GPU_PUBLIC_CLOCK_MEMORY, NvapiConst.NV_GPU_CLOCK_FREQUENCIES_BASE_CLOCK);
        }
        public uint GetCoreClockBoostFreq(int busId) {
            return NvGetAllClockFrequenciesV2(busId, (uint)NvGpuPublicClockId.NVAPI_GPU_PUBLIC_CLOCK_GRAPHICS, NvapiConst.NV_GPU_CLOCK_FREQUENCIES_BOOST_CLOCK);
        }
        public uint GetMemClockBoostFreq(int busId) {
            return NvGetAllClockFrequenciesV2(busId, (uint)NvGpuPublicClockId.NVAPI_GPU_PUBLIC_CLOCK_MEMORY, NvapiConst.NV_GPU_CLOCK_FREQUENCIES_BOOST_CLOCK);
        }

        public uint GetCoreClockFreq(int busId) {
            return NvGetAllClockFrequenciesV2(busId, (uint)NvGpuPublicClockId.NVAPI_GPU_PUBLIC_CLOCK_GRAPHICS, NvapiConst.NV_GPU_CLOCK_FREQUENCIES_CURRENT_FREQ);
        }
        public uint GetMemClockFreq(int busId) {
            return NvGetAllClockFrequenciesV2(busId, (uint)NvGpuPublicClockId.NVAPI_GPU_PUBLIC_CLOCK_MEMORY, NvapiConst.NV_GPU_CLOCK_FREQUENCIES_CURRENT_FREQ);
        }

        public bool SetCoreClock(int busId, int mHz, int voltage) {
            int kHz = mHz * 1000;
            try {
                if (NvGetPStateV2(busId, out NvGpuPerfPStates20InfoV2 info)) {
                    info.numPStates = 1;
                    info.numClocks = 1;
                    info.pstates[0].clocks[0].domainId = NvGpuPublicClockId.NVAPI_GPU_PUBLIC_CLOCK_GRAPHICS;
                    info.pstates[0].clocks[0].freqDelta_kHz.value = kHz;
                    return NvSetPStateV2(busId, ref info);
                }
                return false;
            }
            catch {
            }
            return false;
        }

        public bool SetMemoryClock(int busId, int mHz, int voltage) {
            int kHz = mHz * 1000;
            try {
                if (NvGetPStateV2(busId, out NvGpuPerfPStates20InfoV2 info)) {
                    info.numPStates = 1;
                    info.numClocks = 1;
                    info.numBaseVoltages = 0;

                    info.pstates[0].clocks[0].domainId = NvGpuPublicClockId.NVAPI_GPU_PUBLIC_CLOCK_MEMORY;
                    info.pstates[0].clocks[0].typeId = NvGpuPerfPState20ClockTypeId.NVAPI_GPU_PERF_PSTATE20_CLOCK_TYPE_SINGLE;
                    info.pstates[0].clocks[0].freqDelta_kHz.value = kHz;

                    return NvSetPStateV2(busId, ref info);
                }
                return false;
            }
            catch {
            }
            return false;
        }

        public bool SetTempLimit(int busId, int value) {
            value = value << 8;
            try {
                if (!NvThermalPoliciesGetInfo(busId, out NvGpuThermalInfo info)) {
                    return false;
                }
                if (value == 0) {
                    value = info.entries[0].def_temp;
                }
                else if (value > info.entries[0].max_temp) {
                    value = info.entries[0].max_temp;
                }
                else if (value < info.entries[0].min_temp) {
                    value = info.entries[0].min_temp;
                }

                NvGpuThermalLimit limit = NvThermalPoliciesGetLimit(busId);
                limit.flags = 1;
                limit.entries[0].value = (uint)value;

                return NvThermalPoliciesSetLimit(busId, ref limit);
            }
            catch {
            }
            return false;
        }

        public bool GetPowerPoliciesInfo(int busId, out uint minPower, out uint defPower, out uint maxPower) {
            minPower = 0;
            defPower = 0;
            maxPower = 0;
            if (NvPowerPoliciesGetInfo(busId, out NvGpuPowerInfo info)) {
                if (info.valid == 1 && info.count > 0) {
                    minPower = info.entries[0].min_power;
                    defPower = info.entries[0].def_power;
                    maxPower = info.entries[0].max_power;
                    return true;
                }
            }
            return false;
        }

        public bool SetPowerValue(int busId, uint percentInt) {
            uint minPower = 0, defPower = 0, maxPower = 0;

            try {
                if (GetPowerPoliciesInfo(busId, out minPower, out defPower, out maxPower)) {
                    if (percentInt == 0) {
                        percentInt = defPower;
                    }
                    else if (percentInt < minPower) {
                        percentInt = minPower;
                    }
                    else if (percentInt > maxPower) {
                        percentInt = maxPower;
                    }

                    if (NvPowerPoliciesGetStatus(busId, out NvGpuPowerStatus info)) {
                        info.flags = 1;
                        info.entries[0].power = percentInt;
                        return NvPowerPoliciesSetStatus(busId, ref info);
                    }
                    return false;
                }
            }
            catch {
            }
            return false;
        }

        public bool GetPowerLimit(int busId, out uint outCurrPower, out uint outMinPower, out uint outDefPower, out uint outMaxPower) {
            outCurrPower = 0;
            outMinPower = 0;
            outDefPower = 0;
            outMaxPower = 0;
            try {
                if (GetPowerPoliciesInfo(busId, out outMinPower, out outDefPower, out outMaxPower)) {
                    if (NvPowerPoliciesGetStatus(busId, out NvGpuPowerStatus info)) {
                        outCurrPower = info.entries[0].power;
                        outCurrPower = outCurrPower / 1000;
                        outMinPower = outMinPower / 1000;
                        outDefPower = outDefPower / 1000;
                        outMaxPower = outMaxPower / 1000;
                        return true;
                    }
                }
                return false;
            }
            catch {
                return false;
            }
        }

        public bool SetPowerLimit(int busId, int powerValue) {
            powerValue *= 1000;
            return SetPowerValue(busId, (uint)powerValue);
        }

        public bool GetFanSpeed(int busId, out uint currCooler) {
            if (GetCooler(busId, out currCooler, out uint minCooler, out uint defCooler, out uint maxCooler)) {
                return true;
            }
            return false;
        }

        public bool SetFanSpeed(int busId, int value, bool isAutoMode) {
            return SetCooler(busId, (uint)value, isAutoMode);
        }

        #region private methods
        private bool NvPowerPoliciesSetStatus(int busId, ref NvGpuPowerStatus info) {
            try {
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NvGpuPowerStatus))));
                var r = NvapiNativeMethods.NvPowerPoliciesSetStatus(HandlesByBusId[busId], ref info);
                if (r != NvStatus.OK) {
                    Write.DevError($"{nameof(NvapiNativeMethods.NvPowerPoliciesSetStatus)} {r}");
                }
                if (r == NvStatus.OK) {
                    return true;
                }
            }
            catch {
            }
            return false;
        }

        private bool GetClockDelta(int busId,
            out int outCoreCurrFreqDelta, out int outCoreMinFreqDelta, out int outCoreMaxFreqDelta,
            out int outMemoryCurrFreqDelta, out int outMemoryMinFreqDelta, out int outMemoryMaxFreqDelta) {
            outCoreCurrFreqDelta = 0;
            outCoreMinFreqDelta = 0;
            outCoreMaxFreqDelta = 0;
            outMemoryCurrFreqDelta = 0;
            outMemoryMinFreqDelta = 0;
            outMemoryMaxFreqDelta = 0;
            try {
                bool isCoreClockPicked = false;
                bool isMemoryClockPicked = false;
                if (NvGetPStateV2(busId, out NvGpuPerfPStates20InfoV2 info)) {
                    for (int i = 0; i < info.numPStates; i++) {
                        for (int j = 0; j < info.numClocks; j++) {
                            uint min = info.pstates[i].clocks[j].data.minFreq_kHz;
                            uint max = info.pstates[i].clocks[j].data.maxFreq_kHz;
                            var domainId = info.pstates[i].clocks[j].domainId;
                            if (!isCoreClockPicked && domainId == NvGpuPublicClockId.NVAPI_GPU_PUBLIC_CLOCK_GRAPHICS && min > 0 && max > 0) {
                                outCoreCurrFreqDelta = info.pstates[i].clocks[j].freqDelta_kHz.value;
                                outCoreMinFreqDelta = info.pstates[i].clocks[j].freqDelta_kHz.mindelta;
                                outCoreMaxFreqDelta = info.pstates[i].clocks[j].freqDelta_kHz.maxdelta;
                                isCoreClockPicked = true;
                            }
                            if (!isMemoryClockPicked && domainId == NvGpuPublicClockId.NVAPI_GPU_PUBLIC_CLOCK_MEMORY && min > 0 && max > 0) {
                                outMemoryCurrFreqDelta = info.pstates[i].clocks[j].freqDelta_kHz.value;
                                outMemoryMinFreqDelta = info.pstates[i].clocks[j].freqDelta_kHz.mindelta;
                                outMemoryMaxFreqDelta = info.pstates[i].clocks[j].freqDelta_kHz.maxdelta;
                                isMemoryClockPicked = true;
                            }
                            if (isCoreClockPicked && isMemoryClockPicked) {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch {
                return false;
            }
        }

        private NvGpuPerfPStates20InfoV1 NvGetPStateV1(int busId) {
            NvGpuPerfPStates20InfoV1 info = new NvGpuPerfPStates20InfoV1();
            try {
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NvGpuPerfPStates20InfoV1))));
                var r = NvapiNativeMethods.NvGetPStateV1(HandlesByBusId[busId], ref info);
                if (r != NvStatus.OK) {
                    Write.DevError($"{nameof(NvapiNativeMethods.NvGetPStateV1)} {r}");
                }
                if (r == NvStatus.OK) {
                    return info;
                }
            }
            catch {
            }
            return info;
        }

        private bool NvGetPStateV2(int busId, out NvGpuPerfPStates20InfoV2 info) {
            info = new NvGpuPerfPStates20InfoV2();
            try {
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NvGpuPerfPStates20InfoV2))));
                var r = NvapiNativeMethods.NvGetPStateV2(HandlesByBusId[busId], ref info);
                if (r != NvStatus.OK) {
                    Write.DevError($"{nameof(NvapiNativeMethods.NvGetPStateV2)} {r}");
                    return false;
                }
                return true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return false;
            }
        }

        private bool NvSetPStateV2(int busId, ref NvGpuPerfPStates20InfoV2 info) {
            try {
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NvGpuPerfPStates20InfoV2))));
                var r = NvapiNativeMethods.NvSetPStateV2(HandlesByBusId[busId], ref info);
                if (r != NvStatus.OK) {
                    Write.DevError($"{nameof(NvapiNativeMethods.NvSetPStateV2)} {r}");
                }
                if (r == NvStatus.OK) {
                    return true;
                }
            }
            catch {
            }
            return false;
        }

        private bool NvSetPStateV1(int busId, ref NvGpuPerfPStates20InfoV1 info) {
            try {
                int len = Marshal.SizeOf(typeof(NvGpuPerfPStates20InfoV1));
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NvGpuPerfPStates20InfoV1))));
                var r = NvapiNativeMethods.NvSetPStateV1(HandlesByBusId[busId], ref info);
                if (r != NvStatus.OK) {
                    Write.DevError($"{nameof(NvapiNativeMethods.NvSetPStateV1)} {r}");
                }
                if (r == NvStatus.OK) {
                    return true;
                }
            }
            catch {
            }
            return false;
        }

        private NvGpuClockFrequenciesV2 NvGetAllClockFrequenciesV2(int busId, uint type) {
            NvGpuClockFrequenciesV2 info = new NvGpuClockFrequenciesV2();
            try {
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NvGpuClockFrequenciesV2))));
                info.ClockType = type;
                var r = NvapiNativeMethods.NvGetAllClockFrequenciesV2(HandlesByBusId[busId], ref info);
                if (r != NvStatus.OK) {
                    Write.DevError($"{nameof(NvapiNativeMethods.NvGetAllClockFrequenciesV2)} {r}");
                }
                if (r == NvStatus.OK) {
                    return info;
                }
            }
            catch {
            }
            return info;
        }

        private uint NvGetAllClockFrequenciesV2(int busId, uint clockId, uint clockType) {
            try {
                NvGpuClockFrequenciesV2 info = NvGetAllClockFrequenciesV2(busId, clockType);
                if (clockId < info.domain.Length) {
                    NvGpuClockRrequenciesDomain domain = info.domain[clockId];
                    if (domain.bIsPresent == 1 && info.ClockType == clockType) {
                        return domain.frequency;
                    }
                }
            }
            catch {
            }
            return 0;
        }

        private bool NvThermalPoliciesGetInfo(int busId, out NvGpuThermalInfo info) {
            info = new NvGpuThermalInfo();
            try {
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NvGpuThermalInfo))));
                var r = NvapiNativeMethods.NvThermalPoliciesGetInfo(HandlesByBusId[busId], ref info);
                if (r != NvStatus.OK) {
                    Write.DevError($"{nameof(NvapiNativeMethods.NvThermalPoliciesGetInfo)} {r}");
                    return false;
                }
                return true;
            }
            catch {
                return false;
            }
        }

        private NvGpuThermalLimit NvThermalPoliciesGetLimit(int busId) {
            NvGpuThermalLimit info = new NvGpuThermalLimit();
            try {
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NvGpuThermalLimit))));
                var r = NvapiNativeMethods.NvThermalPoliciesGetLimit(HandlesByBusId[busId], ref info);
                if (r != NvStatus.OK) {
                    Write.DevError($"{nameof(NvapiNativeMethods.NvThermalPoliciesGetLimit)} {r}");
                }
                if (r == NvStatus.OK) {
                    return info;
                }
            }
            catch {
            }
            return info;
        }

        private bool NvThermalPoliciesSetLimit(int busId, ref NvGpuThermalLimit info) {
            try {
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NvGpuThermalLimit))));
                var r = NvapiNativeMethods.NvThermalPoliciesSetLimit(HandlesByBusId[busId], ref info);
                if (r != NvStatus.OK) {
                    Write.DevError($"{nameof(NvapiNativeMethods.NvThermalPoliciesSetLimit)} {r}");
                }
                if (r == NvStatus.OK) {
                    return true;
                }
            }
            catch {
            }
            return false;
        }

        private bool GetThermalInfo(int busId, out int minThermal, out int defThermal, out int maxThermal) {
            minThermal = 0;
            defThermal = 0;
            maxThermal = 0;
            try {
                var r = NvThermalPoliciesGetInfo(busId, out NvGpuThermalInfo info);
                minThermal = info.entries[0].min_temp / (1 << 8);
                defThermal = info.entries[0].def_temp / (1 << 8);
                maxThermal = info.entries[0].max_temp / (1 << 8);
                return r;
            }
            catch {
                return false;
            }
        }

        private bool GetTempLimit(int busId, out int outCurrTemp, out int outMinTemp, out int outDefTemp, out int outMaxTemp) {
            outCurrTemp = 0;
            outMinTemp = 0;
            outDefTemp = 0;
            outMaxTemp = 0;
            try {
                var r = GetThermalInfo(busId, out outMinTemp, out outDefTemp, out outMaxTemp);
                NvGpuThermalLimit limit = NvThermalPoliciesGetLimit(busId);
                outCurrTemp = (int)(limit.entries[0].value / 256);
                return r;
            }
            catch {
                return false;
            }
        }

        private void SetDefaultTempLimit(int busId) {
            int currValue = 0;
            int minValue = 0;
            int defValue = 0;
            int maxValue = 0;
            if (GetTempLimit(busId, out currValue, out minValue, out defValue, out maxValue)) {
                SetTempLimit(busId, defValue);
            }
        }

        private bool NvPowerPoliciesGetStatus(int busId, out NvGpuPowerStatus info) {
            info = new NvGpuPowerStatus();
            try {
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NvGpuPowerStatus))));
                var r = NvapiNativeMethods.NvPowerPoliciesGetStatus(HandlesByBusId[busId], ref info);
                if (r != NvStatus.OK) {
                    Write.DevError($"{nameof(NvapiNativeMethods.NvPowerPoliciesGetStatus)} {r}");
                    return false;
                }
                return true;
            }
            catch {
            }
            return false;
        }

        private double GetPowerPercent(int busId) {
            try {
                if (NvPowerPoliciesGetStatus(busId, out NvGpuPowerStatus info)) {
                    return (info.entries[0].power / 1000) / 100.0;
                }
                return 1.0;
            }
            catch {
            }
            return 1.0;
        }

        private bool NvPowerPoliciesGetInfo(int busId, out NvGpuPowerInfo info) {
            info = new NvGpuPowerInfo();
            try {
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NvGpuPowerInfo))));
                var r = NvapiNativeMethods.NvPowerPoliciesGetInfo(HandlesByBusId[busId], ref info);
                if (r != NvStatus.OK) {
                    Write.DevError($"{nameof(NvapiNativeMethods.NvPowerPoliciesGetInfo)} {r}");
                    return false;
                }
                return true;
            }
            catch {
                return false;
            }
        }

        private bool SetDefaultPowerLimit(int busId) {
            uint currPower;
            uint minPower;
            uint defPower;
            uint maxPower;
            if (GetPowerLimit(busId, out currPower, out minPower, out defPower, out maxPower)) {
                return SetPowerValue(busId, defPower * 1000);
            }
            return false;
        }

        private Dictionary<int, int> _nvGetCoolerSettingsErrorCountByBusId = new Dictionary<int, int>();
        // 20卡不支持该方法，所以尝试两次返回值不正确不再尝试
        private bool GetCoolerSettings(int busId, out NvCoolerSettings info) {
            info = new NvCoolerSettings();
            int count;
            if (_nvGetCoolerSettingsErrorCountByBusId.TryGetValue(busId, out count)) {
                if (count > 1) {
                    return false;
                }
            }
            else {
                _nvGetCoolerSettingsErrorCountByBusId.Add(busId, 1);
            }
            info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NvCoolerSettings))));
            try {
                NvCoolerTarget coolerIndex = NvCoolerTarget.NVAPI_COOLER_TARGET_ALL;
                var r = NvapiNativeMethods.NvGetCoolerSettings(HandlesByBusId[busId], coolerIndex, ref info);
                if (r != NvStatus.OK) {
                    _nvGetCoolerSettingsErrorCountByBusId[busId] = count + 1;
                    Write.DevError($"{nameof(NvapiNativeMethods.NvGetCoolerSettings)} {r}");
                    return false;
                }
                return true;
            }
            catch {
            }
            return false;
        }

        private bool GetCoolerSettings(int busId, ref uint minCooler, ref uint currCooler, ref uint maxCooler) {
            try {
                if (!GetCoolerSettings(busId, out NvCoolerSettings info)) {
                    return false;
                }
                if (info.count > 0) {
                    minCooler = info.cooler[0].currentMinLevel;
                    currCooler = info.cooler[0].currentLevel;
                    maxCooler = info.cooler[0].currentMaxLevel;
                }
                return true;
            }
            catch {
                return false;
            }
        }

        private bool NvFanCoolersGetControl(int busId, out PrivateFanCoolersControlV1 info) {
            info = new PrivateFanCoolersControlV1();
            info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(PrivateFanCoolersControlV1))));
            var r = NvapiNativeMethods.NvFanCoolersGetControl(HandlesByBusId[busId], ref info);
            if (r != NvStatus.OK) {
                Write.DevError($"{nameof(NvapiNativeMethods.NvFanCoolersGetControl)} {r}");
                return false;
            }
            return true;
        }

        private bool SetCooler(int busId, uint value, bool isAutoMode) {
            #region GTX
            try {
                NvCoolerTarget coolerIndex = NvCoolerTarget.NVAPI_COOLER_TARGET_ALL;
                NvCoolerLevel info = new NvCoolerLevel() {
                    version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NvCoolerLevel)))),
                    coolers = new NvCoolerLevelItem[NvapiConst.NVAPI_MAX_COOLERS_PER_GPU]
                };
                info.coolers[0].currentLevel = isAutoMode ? 0 : value;
                info.coolers[0].currentPolicy = isAutoMode ? NvCoolerPolicy.NVAPI_COOLER_POLICY_AUTO : NvCoolerPolicy.NVAPI_COOLER_POLICY_MANUAL;
                var r = NvapiNativeMethods.NvSetCoolerLevels(HandlesByBusId[busId], coolerIndex, ref info);
                if (r != NvStatus.OK) {
                    Write.DevError($"{nameof(NvapiNativeMethods.NvSetCoolerLevels)} {r}");
                }
                else {
                    return true;
                }
            }
            catch {
            }
            #endregion

            #region RTX
            try {
                if (NvFanCoolersGetControl(busId, out PrivateFanCoolersControlV1 info)) {
                    for (int i = 0; i < info.FanCoolersControlCount; i++) {
                        info.FanCoolersControlEntries[i].ControlMode = isAutoMode ? FanCoolersControlMode.Auto : FanCoolersControlMode.Manual;
                        info.FanCoolersControlEntries[i].Level = isAutoMode ? 0u : (uint)value;
                    }
                    var r = NvapiNativeMethods.NvFanCoolersSetControl(HandlesByBusId[busId], ref info);
                    if (r != NvStatus.OK) {
                        Write.DevError($"{nameof(NvapiNativeMethods.NvFanCoolersSetControl)} {r}");
                        return false;
                    }
                    return true;
                }
                return false;
            }
            catch(Exception e) {
                Logger.ErrorDebugLine(e);
                return false;
            }
            #endregion
        }

        private bool GetCooler(int busId, out uint currCooler, out uint minCooler, out uint defCooler, out uint maxCooler) {
            currCooler = 0;
            minCooler = 0;
            defCooler = 0;
            maxCooler = 0;
            try {
                bool r = GetCoolerSettings(busId, ref minCooler, ref currCooler, ref maxCooler);
                if (maxCooler == 0) {
                    maxCooler = 100;
                }
                return r;
            }
            catch {
                return false;
            }
        }
        #endregion
    }
}
