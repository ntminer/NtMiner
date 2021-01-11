using NTMiner.Gpus.Nvapi;
using System;
using System.Collections.Generic;

namespace NTMiner.Gpus {
    public class NvapiHelper : IGpuHelper {
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
                    if (r != NvStatus.NVAPI_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(NvapiNativeMethods.NvEnumPhysicalGPUs)} {r.ToString()}");
                    }
                    for (int i = 0; i < gpuCount; i++) {
                        r = NvapiNativeMethods.NvGetBusID(handles[i], out int busId);
                        if (r != NvStatus.NVAPI_OK) {
                            NTMinerConsole.DevError(() => $"{nameof(NvapiNativeMethods.NvGetBusID)} {r.ToString()}");
                        }
                        if (!_handlesByBusId.ContainsKey(busId)) {
                            _handlesByBusId.Add(busId, handles[i]);
                        }
                    }
                    handles = new NvPhysicalGpuHandle[NvapiConst.MAX_PHYSICAL_GPUS];
                    r = NvapiNativeMethods.NvEnumTCCPhysicalGPUs(handles, out gpuCount);
                    if (r != NvStatus.NVAPI_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(NvapiNativeMethods.NvEnumTCCPhysicalGPUs)} {r.ToString()}");
                    }
                    for (int i = 0; i < gpuCount; i++) {
                        r = NvapiNativeMethods.NvGetBusID(handles[i], out int busId);
                        if (r != NvStatus.NVAPI_OK) {
                            NTMinerConsole.DevError(() => $"{nameof(NvapiNativeMethods.NvGetBusID)} {r.ToString()}");
                        }
                        if (!_handlesByBusId.ContainsKey(busId)) {
                            _handlesByBusId.Add(busId, handles[i]);
                        }
                    }
                    return _handlesByBusId;
                }
            }
        }

        #region IGpuHelper成员
        #region OverClock
        public void OverClock(
            IGpu gpu, 
            int coreClockMHz, 
            int coreClockVoltage, 
            int memoryClockMHz,
            int memoryClockVoltage, 
            int powerLimit, 
            int tempLimit, 
            int fanSpeed) {
            if (coreClockVoltage < 0) {
                coreClockVoltage = 0;
            }
            if (memoryClockVoltage < 0) {
                memoryClockVoltage = 0;
            }
            bool isSetCoreClock = coreClockMHz == 0 || coreClockMHz != gpu.CoreClockDelta || coreClockVoltage != gpu.CoreVoltage;
            bool isSetMemoryClock = memoryClockMHz == 0 || memoryClockMHz != gpu.MemoryClockDelta || memoryClockVoltage != gpu.MemoryVoltage;
            bool isSetPowerLimit = powerLimit == 0 || powerLimit != gpu.PowerCapacity;
            bool isSetTempLimit = tempLimit == 0 || tempLimit != gpu.TempLimit;
            int busId = gpu.GetOverClockId();
            if (isSetCoreClock) {
                SetCoreClock(busId, coreClockMHz, coreClockVoltage);
            }
            if (isSetMemoryClock) {
                SetMemoryClock(busId, memoryClockMHz, memoryClockVoltage);
            }
            if (isSetPowerLimit) {
                SetPowerLimit(busId, powerLimit);
            }
            if (isSetTempLimit) {
                SetTempLimit(busId, tempLimit);
            }
            // fanSpeed == -1表示开源自动温控
            if (fanSpeed >= 0) {
                SetFanSpeed(gpu, fanSpeed);
            }
        }
        #endregion

        #region GetClockRange
        public OverClockRange GetClockRange(IGpu gpu) {
            int busId = gpu.GetOverClockId();
            OverClockRange result = new OverClockRange(busId);
            try {
                if (GetClockDelta(busId,
                    out int outCoreCurrFreqDelta, out int outCoreMinFreqDelta, out int outCoreMaxFreqDelta,
                    out int outMemoryCurrFreqDelta, out int outMemoryMinFreqDelta, out int outMemoryMaxFreqDelta)) {
                    result.CoreClockMin = outCoreMinFreqDelta;
                    result.CoreClockMax = outCoreMaxFreqDelta;
                    result.CoreClockDelta = outCoreCurrFreqDelta;
                    result.MemoryClockMin = outMemoryMinFreqDelta;
                    result.MemoryClockMax = outMemoryMaxFreqDelta;
                    result.MemoryClockDelta = outMemoryCurrFreqDelta;
                }

                if (GetPowerLimit(busId, out uint outCurrPower, out uint outMinPower, out uint outDefPower, out uint outMaxPower)) {
                    result.PowerMin = (int)outMinPower;
                    result.PowerMax = (int)outMaxPower;
                    result.PowerDefault = (int)outDefPower;
                    result.PowerCurr = (int)outCurrPower;
                }

                if (GetTempLimit(busId, out int outCurrTemp, out int outMinTemp, out int outDefTemp, out int outMaxTemp)) {
                    result.TempLimitMin = outMinTemp;
                    result.TempLimitMax = outMaxTemp;
                    result.TempLimitDefault = outDefTemp;
                    result.TempCurr = outCurrTemp;
                }

                if (GetCooler(busId, out uint minCooler, out uint currCooler, out uint maxCooler)) {
                    result.FanSpeedCurr = (int)currCooler;
                    result.FanSpeedMin = (int)minCooler;
                    result.FanSpeedMax = (int)maxCooler;
                }
#if DEBUG
                NTMinerConsole.DevWarn(() => $"GetClockRange {result.ToString()}");
#endif
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            return result;
        }
        #endregion

        #region SetFanSpeed
        public void SetFanSpeed(IGpu gpu, int fanSpeed) {
            bool isAutoMode = fanSpeed == 0;
            uint value = (uint)fanSpeed;
            int busId = gpu.GetOverClockId();
            if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                return;
            }
            #region GTX
            if (NvapiNativeMethods.NvSetCoolerLevels != null) {
                try {
                    NvCoolerTarget coolerIndex = NvCoolerTarget.NVAPI_COOLER_TARGET_ALL;
                    NvCoolerLevel info = NvCoolerLevel.Create();
                    info.coolers[0].currentLevel = isAutoMode ? 0 : value;
                    info.coolers[0].currentPolicy = isAutoMode ? NvCoolerPolicy.NVAPI_COOLER_POLICY_AUTO : NvCoolerPolicy.NVAPI_COOLER_POLICY_MANUAL;
                    var r = NvapiNativeMethods.NvSetCoolerLevels(handle, coolerIndex, ref info);
                    if (r != NvStatus.NVAPI_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(NvapiNativeMethods.NvSetCoolerLevels)} {r.ToString()}");
                    }
                    else {
                        return;
                    }
                }
                catch {
                }
            }
            #endregion

            #region RTX
            if (NvapiNativeMethods.NvFanCoolersSetControl == null) {
                return;
            }
            try {
                if (NvFanCoolersGetControl(busId, out PrivateFanCoolersControlV1 info)) {
                    for (int i = 0; i < info.FanCoolersControlCount; i++) {
                        info.FanCoolersControlEntries[i].ControlMode = isAutoMode ? FanCoolersControlMode.Auto : FanCoolersControlMode.Manual;
                        info.FanCoolersControlEntries[i].Level = isAutoMode ? 0u : (uint)value;
                    }
                    var r = NvapiNativeMethods.NvFanCoolersSetControl(handle, ref info);
                    if (r != NvStatus.NVAPI_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(NvapiNativeMethods.NvFanCoolersSetControl)} {r.ToString()}");
                        return;
                    }
                    return;
                }
                return;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return;
            }
            #endregion
        }
        #endregion

        private static void SetCoreClock(int busId, int mHz, int voltage) {
            int kHz = mHz * 1000;
            try {
                if (NvGetPStateV2(busId, out NvGpuPerfPStates20InfoV2 info)) {
                    info.numPStates = 1;
                    info.numClocks = 1;
                    info.pstates[0].clocks[0].domainId = NvGpuPublicClockId.NVAPI_GPU_PUBLIC_CLOCK_GRAPHICS;
                    info.pstates[0].clocks[0].typeId = NvGpuPerfPState20ClockTypeId.NVAPI_GPU_PERF_PSTATE20_CLOCK_TYPE_SINGLE;
                    info.pstates[0].clocks[0].freqDelta_kHz.value = kHz;
                    var r = NvSetPStateV2(busId, ref info);
                    if (!r) {
                        NTMinerConsole.DevError(() => $"{nameof(SetCoreClock)} {r.ToString()}");
                    }
                }
            }
            catch {
            }
        }

        private static void SetMemoryClock(int busId, int mHz, int voltage) {
            int kHz = mHz * 1000;
            try {
                if (NvGetPStateV2(busId, out NvGpuPerfPStates20InfoV2 info)) {
                    info.numPStates = 1;
                    info.numClocks = 1;
                    info.numBaseVoltages = 0;

                    info.pstates[0].clocks[0].domainId = NvGpuPublicClockId.NVAPI_GPU_PUBLIC_CLOCK_MEMORY;
                    info.pstates[0].clocks[0].typeId = NvGpuPerfPState20ClockTypeId.NVAPI_GPU_PERF_PSTATE20_CLOCK_TYPE_SINGLE;
                    info.pstates[0].clocks[0].freqDelta_kHz.value = kHz;

                    var r = NvSetPStateV2(busId, ref info);
                    if (!r) {
                        NTMinerConsole.DevError(() => $"{nameof(SetMemoryClock)} {r.ToString()}");
                    }
                }
            }
            catch {
            }
        }

        private static void SetTempLimit(int busId, int value) {
            value <<= 8;
            try {
                if (!NvThermalPoliciesGetInfo(busId, out NvGpuThermalInfo info)) {
                    return;
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

                NvThermalPoliciesSetLimit(busId, ref limit);
            }
            catch {
            }
        }

        private static void SetPowerLimit(int busId, int powerValue) {
            powerValue *= 1000;
            uint percentInt = (uint)powerValue;
            try {
                if (GetPowerPoliciesInfo(busId, out uint minPower, out uint defPower, out uint maxPower)) {
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
                        NvPowerPoliciesSetStatus(busId, ref info);
                    }
                }
            }
            catch {
            }
        }
        #endregion

        public bool GetPowerLimit(
            int busId, 
            out uint outCurrPower, 
            out uint outMinPower, 
            out uint outDefPower, 
            out uint outMaxPower) {
            outCurrPower = 0;
            outMinPower = 0;
            outDefPower = 0;
            outMaxPower = 0;
            try {
                if (GetPowerPoliciesInfo(busId, out outMinPower, out outDefPower, out outMaxPower)) {
                    if (NvPowerPoliciesGetStatus(busId, out NvGpuPowerStatus info)) {
                        outCurrPower = info.entries[0].power;
                        outCurrPower /= 1000;
                        outMinPower /= 1000;
                        outDefPower /= 1000;
                        outMaxPower /= 1000;
                        return true;
                    }
                }
                return false;
            }
            catch {
                return false;
            }
        }

        public bool GetFanSpeed(int busId, out uint currCooler) {
            if (GetCooler(busId, out _, out currCooler, out _)) {
                return true;
            }
            return false;
        }

        #region private static methods
        private static bool GetPowerPoliciesInfo(int busId, out uint minPower, out uint defPower, out uint maxPower) {
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

        private static bool NvPowerPoliciesSetStatus(int busId, ref NvGpuPowerStatus info) {
            if (NvapiNativeMethods.NvPowerPoliciesSetStatus == null) {
                return false;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return false;
                }
                var r = NvapiNativeMethods.NvPowerPoliciesSetStatus(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(NvapiNativeMethods.NvPowerPoliciesSetStatus)} {r.ToString()}");
                }
                if (r == NvStatus.NVAPI_OK) {
                    return true;
                }
            }
            catch {
            }
            return false;
        }

        private static bool GetClockDelta(int busId,
            out int outCoreCurrFreqDelta, 
            out int outCoreMinFreqDelta, 
            out int outCoreMaxFreqDelta,
            out int outMemoryCurrFreqDelta, 
            out int outMemoryMinFreqDelta, 
            out int outMemoryMaxFreqDelta) {
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
            catch (Exception e){
                Logger.ErrorDebugLine(e);
                return false;
            }
        }

        private static bool NvGetPStateV2(int busId, out NvGpuPerfPStates20InfoV2 info) {
            info = NvGpuPerfPStates20InfoV2.Create();
            if (NvapiNativeMethods.NvGetPStateV2 == null) {
                return false;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return false;
                }
                var r = NvapiNativeMethods.NvGetPStateV2(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(NvapiNativeMethods.NvGetPStateV2)} {r.ToString()}");
                    return false;
                }
                return true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return false;
            }
        }

        private static bool NvSetPStateV2(int busId, ref NvGpuPerfPStates20InfoV2 info) {
            if (NvapiNativeMethods.NvSetPStateV2 == null) {
                return false;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return false;
                }
                var r = NvapiNativeMethods.NvSetPStateV2(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(NvapiNativeMethods.NvSetPStateV2)} {r.ToString()}");
                }
                if (r == NvStatus.NVAPI_OK) {
                    return true;
                }
            }
            catch {
            }
            return false;
        }

        private static bool NvThermalPoliciesGetInfo(int busId, out NvGpuThermalInfo info) {
            info = NvGpuThermalInfo.Create();
            if (NvapiNativeMethods.NvThermalPoliciesGetInfo == null) {
                return false;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return false;
                }
                var r = NvapiNativeMethods.NvThermalPoliciesGetInfo(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(NvapiNativeMethods.NvThermalPoliciesGetInfo)} {r.ToString()}");
                    return false;
                }
                return true;
            }
            catch {
                return false;
            }
        }

        private static NvGpuThermalLimit NvThermalPoliciesGetLimit(int busId) { 
            NvGpuThermalLimit info = NvGpuThermalLimit.Create();
            if (NvapiNativeMethods.NvThermalPoliciesGetLimit == null) {
                return info;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return info;
                }
                var r = NvapiNativeMethods.NvThermalPoliciesGetLimit(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(NvapiNativeMethods.NvThermalPoliciesGetLimit)} {r.ToString()}");
                }
                if (r == NvStatus.NVAPI_OK) {
                    return info;
                }
            }
            catch {
            }
            return info;
        }

        private static bool NvThermalPoliciesSetLimit(int busId, ref NvGpuThermalLimit info) {
            if (NvapiNativeMethods.NvThermalPoliciesSetLimit == null) {
                return false;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return false;
                }
                var r = NvapiNativeMethods.NvThermalPoliciesSetLimit(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(NvapiNativeMethods.NvThermalPoliciesSetLimit)} {r.ToString()}");
                }
                if (r == NvStatus.NVAPI_OK) {
                    return true;
                }
            }
            catch {
            }
            return false;
        }

        private static bool GetThermalInfo(int busId, out int minThermal, out int defThermal, out int maxThermal) {
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

        private static bool GetTempLimit(
            int busId, 
            out int outCurrTemp, 
            out int outMinTemp, 
            out int outDefTemp, 
            out int outMaxTemp) {
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

        private static bool NvPowerPoliciesGetStatus(int busId, out NvGpuPowerStatus info) {
            info = NvGpuPowerStatus.Create();
            if (NvapiNativeMethods.NvPowerPoliciesGetStatus == null) {
                return false;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return false;
                }
                var r = NvapiNativeMethods.NvPowerPoliciesGetStatus(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(NvapiNativeMethods.NvPowerPoliciesGetStatus)} {r.ToString()}");
                    return false;
                }
                return true;
            }
            catch {
            }
            return false;
        }

        private static bool NvPowerPoliciesGetInfo(int busId, out NvGpuPowerInfo info) {
            info = NvGpuPowerInfo.Create();
            if (NvapiNativeMethods.NvPowerPoliciesGetInfo == null) {
                return false;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return false;
                }
                var r = NvapiNativeMethods.NvPowerPoliciesGetInfo(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(NvapiNativeMethods.NvPowerPoliciesGetInfo)} {r.ToString()}");
                    return false;
                }
                return true;
            }
            catch {
                return false;
            }
        }

        private static readonly HashSet<int> _nvFanCoolersGetStatusNotSupporteds = new HashSet<int>();
        private static bool GetFanCoolersGetStatus(int busId, out PrivateFanCoolersStatusV1 info) {
            info = PrivateFanCoolersStatusV1.Create();
            if (NvapiNativeMethods.NvFanCoolersGetStatus == null) {
                return false;
            }
            if (_nvFanCoolersGetStatusNotSupporteds.Contains(busId)) {
                return false;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return false;
                }
                var r = NvapiNativeMethods.NvFanCoolersGetStatus(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    if (r == NvStatus.NVAPI_NOT_SUPPORTED || r == NvStatus.NVAPI_FIRMWARE_REVISION_NOT_SUPPORTED) {
                        _nvFanCoolersGetStatusNotSupporteds.Add(busId);
                    }
                    NTMinerConsole.DevError(() => $"{nameof(NvapiNativeMethods.NvFanCoolersGetStatus)} {r.ToString()}");
                    return false;
                }
                return true;
            }
            catch {
            }
            return false;
        }

        private static readonly HashSet<int> _nvGetCoolerSettingsNotSupporteds = new HashSet<int>();
        private static bool GetCoolerSettings(int busId, out NvCoolerSettings info) {
            info = NvCoolerSettings.Create();
            if (NvapiNativeMethods.NvGetCoolerSettings == null) {
                return false;
            }
            if (_nvGetCoolerSettingsNotSupporteds.Contains(busId)) {
                return false;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return false;
                }
                NvCoolerTarget coolerIndex = NvCoolerTarget.NVAPI_COOLER_TARGET_ALL;
                var r = NvapiNativeMethods.NvGetCoolerSettings(handle, coolerIndex, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    if (r == NvStatus.NVAPI_NOT_SUPPORTED || r == NvStatus.NVAPI_FIRMWARE_REVISION_NOT_SUPPORTED || r == NvStatus.NVAPI_GPU_NOT_POWERED) {
                        _nvGetCoolerSettingsNotSupporteds.Add(busId);
                    }
                    NTMinerConsole.DevError(() => $"{nameof(NvapiNativeMethods.NvGetCoolerSettings)} {r.ToString()}");
                    return false;
                }
                return true;
            }
            catch {
            }
            return false;
        }

        private static bool NvFanCoolersGetControl(int busId, out PrivateFanCoolersControlV1 info) {
            info = PrivateFanCoolersControlV1.Create();
            if (NvapiNativeMethods.NvFanCoolersGetControl == null) {
                return false;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return false;
                }
                var r = NvapiNativeMethods.NvFanCoolersGetControl(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(NvapiNativeMethods.NvFanCoolersGetControl)} {r.ToString()}");
                    return false;
                }
                return true;
            }
            catch {
            }
            return false;
        }

        private static bool GetCooler(int busId, out uint minCooler, out uint currCooler, out uint maxCooler) {
            currCooler = 0;
            minCooler = 0;
            maxCooler = 0;
            try {
                if (GetFanCoolersGetStatus(busId, out PrivateFanCoolersStatusV1 v1)) {
                    if (v1.FanCoolersStatusCount > 0) {
                        minCooler = v1.FanCoolersStatusEntries[0].CurrentMinimumLevel;
                        currCooler = v1.FanCoolersStatusEntries[0].CurrentLevel;
                        maxCooler = v1.FanCoolersStatusEntries[0].CurrentMaximumLevel;
                        return true;
                    }
                }
                if (GetCoolerSettings(busId, out NvCoolerSettings info)) {
                    if (info.count > 0) {
                        minCooler = info.cooler[0].currentMinLevel;
                        currCooler = info.cooler[0].currentLevel;
                        maxCooler = info.cooler[0].currentMaxLevel;
                        return true;
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            return false;
        }
        #endregion
    }
}
