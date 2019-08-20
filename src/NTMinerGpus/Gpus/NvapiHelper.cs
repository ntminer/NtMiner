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
                        Write.DevWarn($"{nameof(NvapiNativeMethods.NvEnumPhysicalGPUs)} {r}");
                    }
                    for (int i = 0; i < gpuCount; i++) {
                        r = NvapiNativeMethods.NvGetBusID(handles[i], out int busId);
                        if (r != NvStatus.OK) {
                            Write.DevWarn($"{nameof(NvapiNativeMethods.NvGetBusID)} {r}");
                        }
                        if (!_handlesByBusId.ContainsKey(busId)) {
                            _handlesByBusId.Add(busId, handles[i]);
                        }
                    }
                    handles = new NvPhysicalGpuHandle[NvapiConst.MAX_PHYSICAL_GPUS];
                    r = NvapiNativeMethods.NvEnumTCCPhysicalGPUs(handles, out gpuCount);
                    if (r != NvStatus.OK) {
                        Write.DevWarn($"{nameof(NvapiNativeMethods.NvEnumTCCPhysicalGPUs)} {r}");
                    }
                    for (int i = 0; i < gpuCount; i++) {
                        r = NvapiNativeMethods.NvGetBusID(handles[i], out int busId);
                        if (r != NvStatus.OK) {
                            Write.DevWarn($"{nameof(NvapiNativeMethods.NvGetBusID)} {r}");
                        }
                        if (!_handlesByBusId.ContainsKey(busId)) {
                            _handlesByBusId.Add(busId, handles[i]);
                        }
                    }
                    return _handlesByBusId;
                }
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
                if (GetPowerLimit(busId, out uint outCurrPower, out uint outMinPower, out uint outDefPower, out uint outMaxPower)) {
                    powerMin = (int)outMinPower;
                    powerMax = (int)outMaxPower;
                    powerDefault = (int)outDefPower;
                    powerLimit = (int)outCurrPower;
                }
                if (GetTempLimit(busId, out int outCurrTemp, out int outMinTemp, out int outDefTemp, out int outMaxTemp)) {
                    tempLimitMin = outMinTemp;
                    tempLimitMax = outMaxTemp;
                    tempLimitDefault = outDefTemp;
                    tempLimit = outCurrTemp;
                }
                if (GetCooler(busId, out uint currCooler, out uint minCooler, out uint defCooler, out uint maxCooler)) {
                    fanSpeedMin = (int)minCooler;
                    fanSpeedMax = (int)maxCooler;
                    fanSpeedDefault = (int)defCooler;
                }
#if DEBUG
                Write.DevWarn($"{nameof(GetClockRangeByIndex)} coreClockMin={coreClockMin},coreClockMax={coreClockMax},coreClockDelta={coreClockDelta},memoryClockMin={memoryClockMin},memoryClockMax={memoryClockMax},memoryClockDelta={memoryClockDelta},powerMin={powerMin},powerMax={powerMax},powerDefault={powerDefault},powerLimit={powerLimit},tempLimitMin={tempLimitMin},tempLimitMax={tempLimitMax},tempLimitDefault={tempLimitDefault},tempLimit={tempLimit},fanSpeedMin={fanSpeedMin},fanSpeedMax={fanSpeedMax},fanSpeedDefault={fanSpeedDefault}");
#endif
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        public uint GetCoreClockBaseFreq(int busId) {
            return GetClockFreq(busId, (uint)NV_GPU_PUBLIC_CLOCK_ID.NVAPI_GPU_PUBLIC_CLOCK_GRAPHICS, NvapiConst.NV_GPU_CLOCK_FREQUENCIES_BASE_CLOCK);
        }
        public uint GetMemClockBaseFreq(int busId) {
            return GetClockFreq(busId, (uint)NV_GPU_PUBLIC_CLOCK_ID.NVAPI_GPU_PUBLIC_CLOCK_MEMORY, NvapiConst.NV_GPU_CLOCK_FREQUENCIES_BASE_CLOCK);
        }
        public uint GetCoreClockBoostFreq(int busId) {
            return GetClockFreq(busId, (uint)NV_GPU_PUBLIC_CLOCK_ID.NVAPI_GPU_PUBLIC_CLOCK_GRAPHICS, NvapiConst.NV_GPU_CLOCK_FREQUENCIES_BOOST_CLOCK);
        }
        public uint GetMemClockBoostFreq(int busId) {
            return GetClockFreq(busId, (uint)NV_GPU_PUBLIC_CLOCK_ID.NVAPI_GPU_PUBLIC_CLOCK_MEMORY, NvapiConst.NV_GPU_CLOCK_FREQUENCIES_BOOST_CLOCK);
        }

        public uint GetCoreClockFreq(int busId) {
            return GetClockFreq(busId, (uint)NV_GPU_PUBLIC_CLOCK_ID.NVAPI_GPU_PUBLIC_CLOCK_GRAPHICS, NvapiConst.NV_GPU_CLOCK_FREQUENCIES_CURRENT_FREQ);
        }
        public uint GetMemClockFreq(int busId) {
            return GetClockFreq(busId, (uint)NV_GPU_PUBLIC_CLOCK_ID.NVAPI_GPU_PUBLIC_CLOCK_MEMORY, NvapiConst.NV_GPU_CLOCK_FREQUENCIES_CURRENT_FREQ);
        }

        public bool SetCoreClockV2(int busId, int kHz) {
            try {
                NvGpuPerfPstates20InfoV2 info = NvGetPStateV2(busId);
                info.numPstates = 1;
                info.numClocks = 1;
                info.pstates[0].clocks[0].domainId = NV_GPU_PUBLIC_CLOCK_ID.NVAPI_GPU_PUBLIC_CLOCK_GRAPHICS;
                info.pstates[0].clocks[0].freqDelta_kHz.value = kHz;
                return NvSetPStateV2(busId, ref info);
            }
            catch {
            }
            return false;
        }

        public bool SetMemClockV2(int busId, int kHz) {
            try {
                NvGpuPerfPstates20InfoV2 info = NvGetPStateV2(busId);
                info.numPstates = 1;
                info.numClocks = 1;
                info.numBaseVoltages = 0;

                info.pstates[0].clocks[0].domainId = NV_GPU_PUBLIC_CLOCK_ID.NVAPI_GPU_PUBLIC_CLOCK_MEMORY;
                info.pstates[0].clocks[0].typeId = NV_GPU_PERF_PSTATE20_CLOCK_TYPE_ID.NVAPI_GPU_PERF_PSTATE20_CLOCK_TYPE_SINGLE;
                info.pstates[0].clocks[0].freqDelta_kHz.value = kHz;

                return NvSetPStateV2(busId, ref info);
            }
            catch {
            }
            return false;
        }

        public bool SetThermalValue(int busId, int value) {
            value = value << 8;
            try {
                NvGpuThermalInfo info = NvApiClientThermalPoliciesGetInfo(busId);
                if (value == 0)
                    value = info.entries[0].def_temp;
                else if (value > info.entries[0].max_temp)
                    value = info.entries[0].max_temp;
                else if (value < info.entries[0].min_temp)
                    value = info.entries[0].min_temp;

                NvGpuThermalLimit limit = NvApiClientThermalPoliciesGetLimit(busId);
                limit.flags = 1;
                limit.entries[0].value = (uint)value;

                return ClientThermalPoliciesSetLimit(busId, ref limit);
            }
            catch {
            }
            return false;
        }

        public bool ClientPowerPoliciesGetInfo(int busId, out uint minPower, out uint defPower, out uint maxPower) {
            minPower = 0;
            defPower = 0;
            maxPower = 0;
            NvGpuPowerInfo info = NvApiClientPowerPoliciesGetInfo(busId);
            if (info.valid == 1 && info.count > 0) {
                minPower = info.entries[0].min_power;
                defPower = info.entries[0].def_power;
                maxPower = info.entries[0].max_power;
                return true;
            }
            return false;
        }

        public bool SetPowerPercent(int busId, double percent) {
            uint percentInt = (uint)(percent * 100) * 1000;
            return SetPowerValue(busId, percentInt);
        }

        public bool SetPowerValue(int busId, uint percentInt) {
            uint minPower = 0, defPower = 0, maxPower = 0;

            try {
                if (ClientPowerPoliciesGetInfo(busId, out minPower, out defPower, out maxPower)) {
                    if (percentInt == 0)
                        percentInt = defPower;
                    else if (percentInt < minPower)
                        percentInt = minPower;
                    else if (percentInt > maxPower)
                        percentInt = maxPower;

                    NvGpuPowerStatus info = ClientPowerPoliciesGetStatus(busId);
                    info.flags = 1;
                    info.entries[0].power = percentInt;
                    return ClientPowerPoliciesSetStatus(busId, ref info);
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
                ClientPowerPoliciesGetInfo(busId, out outMinPower, out outDefPower, out outMaxPower);

                NvGpuPowerStatus info = ClientPowerPoliciesGetStatus(busId);
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

        public bool SetPowerLimit(int busId, uint powerValue) {
            powerValue *= 1000;
            return SetPowerValue(busId, powerValue);
        }

        public bool SetCooler(int busId, uint value, bool isAutoMode) {
            if (isAutoMode) {
                return SetCoolerLevelsAuto(busId);
            }
            else {
                return SetCoolerLevels(busId, value);
            }
        }

        #region private methods
        private bool ClientPowerPoliciesSetStatus(int busId, ref NvGpuPowerStatus info) {
            try {
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NvGpuPowerStatus))));
                var r = NvapiNativeMethods.NvPowerPoliciesSetStatus(HandlesByBusId[busId], ref info);
                if (r != NvStatus.OK) {
                    Write.DevWarn($"{nameof(NvapiNativeMethods.NvPowerPoliciesSetStatus)} {r}");
                }
                if (r == NvStatus.OK) {
                    return true;
                }
            }
            catch {
            }
            return false;
        }

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
                NvGpuPerfPstates20InfoV2 info = NvGetPStateV2(busId);

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

        private NvGpuPerfPstates20InfoV1 NvGetPStateV1(int busId) {
            NvGpuPerfPstates20InfoV1 info = new NvGpuPerfPstates20InfoV1();
            try {
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NvGpuPerfPstates20InfoV1))));
                var r = NvapiNativeMethods.NvGetPStateV1(HandlesByBusId[busId], ref info);
                if (r != NvStatus.OK) {
                    Write.DevWarn($"{nameof(NvapiNativeMethods.NvGetPStateV1)} {r}");
                }
                if (r == NvStatus.OK) {
                    return info;
                }
            }
            catch {
            }
            return info;
        }

        private NvGpuPerfPstates20InfoV2 NvGetPStateV2(int busId) {
            NvGpuPerfPstates20InfoV2 info = new NvGpuPerfPstates20InfoV2();
            try {
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NvGpuPerfPstates20InfoV2))));
                var r = NvapiNativeMethods.NvGetPStateV2(_handlesByBusId[busId], ref info);
                if (r != NvStatus.OK) {
                    Write.DevWarn($"{nameof(NvapiNativeMethods.NvGetPStateV2)} {r}");
                }
                if (r == NvStatus.OK) {
                    return info;
                }
            }
            catch {
            }
            return info;
        }

        private bool NvSetPStateV2(int busId, ref NvGpuPerfPstates20InfoV2 info) {
            try {
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NvGpuPerfPstates20InfoV2))));
                var r = NvapiNativeMethods.NvSetPStateV2(_handlesByBusId[busId], ref info);
                if (r != NvStatus.OK) {
                    Write.DevWarn($"{nameof(NvapiNativeMethods.NvSetPStateV2)} {r}");
                }
                if (r == NvStatus.OK) {
                    return true;
                }
            }
            catch {
            }
            return false;
        }

        private bool NvSetPStateV1(int busId, ref NvGpuPerfPstates20InfoV1 info) {
            try {
                int len = Marshal.SizeOf(typeof(NvGpuPerfPstates20InfoV1));
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NvGpuPerfPstates20InfoV1))));
                var r = NvapiNativeMethods.NvSetPStateV1(_handlesByBusId[busId], ref info);
                if (r != NvStatus.OK) {
                    Write.DevWarn($"{nameof(NvapiNativeMethods.NvSetPStateV1)} {r}");
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
                    Write.DevWarn($"{nameof(NvapiNativeMethods.NvGetAllClockFrequenciesV2)} {r}");
                }
                if (r == NvStatus.OK) {
                    return info;
                }
            }
            catch {
            }
            return info;
        }

        private uint GetClockFreq(int busId, uint clockId, uint clockType) {
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

        private NvGpuThermalInfo NvApiClientThermalPoliciesGetInfo(int busId) {
            NvGpuThermalInfo info = new NvGpuThermalInfo();
            try {
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NvGpuThermalInfo))));
                var r = NvapiNativeMethods.NvThermalPoliciesGetInfo(HandlesByBusId[busId], ref info);
                if (r != NvStatus.OK) {
                    Write.DevWarn($"{nameof(NvapiNativeMethods.NvThermalPoliciesGetInfo)} {r}");
                }
                if (r == NvStatus.OK) {
                    return info;
                }
            }
            catch {
            }
            return info;
        }

        private NvGpuThermalLimit NvApiClientThermalPoliciesGetLimit(int busId) {
            NvGpuThermalLimit info = new NvGpuThermalLimit();
            try {
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NvGpuThermalLimit))));
                var r = NvapiNativeMethods.NvThermalPoliciesGetLimit(_handlesByBusId[busId], ref info);
                if (r != NvStatus.OK) {
                    Write.DevWarn($"{nameof(NvapiNativeMethods.NvThermalPoliciesGetLimit)} {r}");
                }
                if (r == NvStatus.OK) {
                    return info;
                }
            }
            catch {
            }
            return info;
        }

        private bool ClientThermalPoliciesSetLimit(int busId, ref NvGpuThermalLimit info) {
            try {
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NvGpuThermalLimit))));
                var r = NvapiNativeMethods.NvThermalPoliciesSetLimit(HandlesByBusId[busId], ref info);
                if (r != NvStatus.OK) {
                    Write.DevWarn($"{nameof(NvapiNativeMethods.NvThermalPoliciesSetLimit)} {r}");
                }
                if (r == NvStatus.OK) {
                    return true;
                }
            }
            catch {
            }
            return false;
        }

        private bool GetThermalInfo(int busId, out double minThermal, out double defThermal, out double maxThermal) {
            minThermal = 0;
            defThermal = 0;
            maxThermal = 0;
            try {
                NvGpuThermalInfo info = NvApiClientThermalPoliciesGetInfo(busId);
                minThermal = info.entries[0].min_temp / 256.0;
                defThermal = info.entries[0].def_temp / 256.0;
                maxThermal = info.entries[0].max_temp / 256.0;
            }
            catch {
                return false;
            }
            return false;
        }

        private bool GetThermalInfo(int busId, out int minThermal, out int defThermal, out int maxThermal) {
            minThermal = 0;
            defThermal = 0;
            maxThermal = 0;
            try {
                NvGpuThermalInfo info = NvApiClientThermalPoliciesGetInfo(busId);
                minThermal = info.entries[0].min_temp / (1 << 8);
                defThermal = info.entries[0].def_temp / (1 << 8);
                maxThermal = info.entries[0].max_temp / (1 << 8);
            }
            catch {
                return false;
            }
            return false;
        }

        private bool GetTempLimit(int busId, out int outCurrTemp, out int outMinTemp, out int outDefTemp, out int outMaxTemp) {
            outCurrTemp = 0;
            outMinTemp = 0;
            outDefTemp = 0;
            outMaxTemp = 0;
            try {
                GetThermalInfo(busId, out outMinTemp, out outDefTemp, out outMaxTemp);

                NvGpuThermalLimit limit = NvApiClientThermalPoliciesGetLimit(busId);
                outCurrTemp = (int)(limit.entries[0].value / 256);

                return true;
            }
            catch {
            }
            return false;
        }

        private bool SetDefaultTempLimit(int busId) {
            int currValue = 0;
            int minValue = 0;
            int defValue = 0;
            int maxValue = 0;
            if (GetTempLimit(busId, out currValue, out minValue, out defValue, out maxValue)) {
                return SetThermalValue(busId, defValue);
            }
            return false;
        }

        private NvGpuPowerStatus ClientPowerPoliciesGetStatus(int busId) {
            NvGpuPowerStatus info = new NvGpuPowerStatus();
            try {
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NvGpuPowerStatus))));
                var r = NvapiNativeMethods.NvPowerPoliciesGetStatus(HandlesByBusId[busId], ref info);
                if (r != NvStatus.OK) {
                    Write.DevWarn($"{nameof(NvapiNativeMethods.NvPowerPoliciesGetStatus)} {r}");
                }
                if (r == NvStatus.OK) {
                    return info;
                }
            }
            catch {
            }
            return info;
        }

        private double GetPowerPercent(int busId) {
            try {
                NvGpuPowerStatus info = ClientPowerPoliciesGetStatus(busId);
                return (info.entries[0].power / 1000) / 100.0;
            }
            catch {
            }
            return 1.0;
        }

        private NvGpuPowerInfo NvApiClientPowerPoliciesGetInfo(int busId) {
            NvGpuPowerInfo info = new NvGpuPowerInfo();
            try {
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NvGpuPowerInfo))));
                var r = NvapiNativeMethods.NvPowerPoliciesGetInfo(HandlesByBusId[busId], ref info);
                if (r != NvStatus.OK) {
                    Write.DevWarn($"{nameof(NvapiNativeMethods.NvPowerPoliciesGetInfo)} {r}");
                }
                if (r == NvStatus.OK) {
                    return info;
                }
            }
            catch {
            }
            return info;
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

        private NvCoolerSettings GetCoolerSettings(int busId) {
            NvCoolerSettings info = new NvCoolerSettings();
            try {
                NvCoolerTarget cmd = NvCoolerTarget.NVAPI_COOLER_TARGET_ALL;
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NvCoolerSettings))));
                var r = NvapiNativeMethods.NvGetCoolerSettings(HandlesByBusId[busId], cmd, ref info);
                if (r != NvStatus.OK) {
                    Write.DevWarn($"{nameof(NvapiNativeMethods.NvGetCoolerSettings)} {r}");
                }
                if (r == NvStatus.OK) {
                    return info;
                }
            }
            catch {
            }
            return info;
        }

        private bool GetCoolerSettings(int busId, ref uint minCooler, ref uint currCooler, ref uint maxCooler) {
            try {
                NvCoolerSettings info = GetCoolerSettings(busId);
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

        private bool IsCoolerAuto(int busId) {
            try {
                NvCoolerSettings info = GetCoolerSettings(busId);
                if (info.count > 0) {
                    return info.cooler[0].currentPolicy != NvCoolerPolicy.NVAPI_COOLER_POLICY_MANUAL;
                }
            }
            catch {
            }
            return false;
        }

        private bool SetCoolerLevels(int busId, NvCoolerTarget coolerIndex, ref NvCoolerLevel level) {
            try {
                level.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NvCoolerLevel))));
                var r = NvapiNativeMethods.NvSetCoolerLevels(HandlesByBusId[busId], coolerIndex, ref level);
                if (r != NvStatus.OK) {
                    Write.DevWarn($"{nameof(NvapiNativeMethods.NvSetCoolerLevels)} {r}");
                }
                if (r == NvStatus.OK) {
                    return true;
                }
            }
            catch {
            }
            return false;
        }

        private bool SetCoolerLevels(int busId, uint value) {
            NvCoolerTarget coolerIndex = NvCoolerTarget.NVAPI_COOLER_TARGET_ALL;
            try {
                NvCoolerLevel level = new NvCoolerLevel();
                level.coolers[0].currentLevel = value;
                level.coolers[0].currentPolicy = NvCoolerPolicy.NVAPI_COOLER_POLICY_MANUAL;

                return SetCoolerLevels(busId, coolerIndex, ref level);
            }
            catch {
            }
            return false;
        }

        private bool SetCoolerLevelsAuto(int busId) {
            NvCoolerTarget coolerIndex = NvCoolerTarget.NVAPI_COOLER_TARGET_ALL;
            try {
                NvCoolerLevel level = new NvCoolerLevel();
                level.coolers[0].currentLevel = 0;
                level.coolers[0].currentPolicy = NvCoolerPolicy.NVAPI_COOLER_POLICY_AUTO;
                return SetCoolerLevels(busId, coolerIndex, ref level);
            }
            catch {
            }
            return false;
        }

        private bool GetCooler(int busId, out uint currCooler, out uint minCooler, out uint defCooler, out uint maxCooler) {
            currCooler = 0;
            minCooler = 0;
            defCooler = 0;
            maxCooler = 0;
            try {
                GetCoolerSettings(busId, ref minCooler, ref currCooler, ref maxCooler);
                defCooler = minCooler;
                return true;
            }
            catch {
            }
            return false;
        }
        #endregion
    }
}
