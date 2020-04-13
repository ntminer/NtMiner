using NTMiner.Gpus.Nvapi;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NTMiner.Gpus {
    public class NvapiHelper : IGpuHelper {
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
                    if (r != NvStatus.NVAPI_OK) {
                        Write.DevError(() => $"{nameof(NvapiNativeMethods.NvEnumPhysicalGPUs)} {r.ToString()}");
                    }
                    for (int i = 0; i < gpuCount; i++) {
                        r = NvapiNativeMethods.NvGetBusID(handles[i], out int busId);
                        if (r != NvStatus.NVAPI_OK) {
                            Write.DevError(() => $"{nameof(NvapiNativeMethods.NvGetBusID)} {r.ToString()}");
                        }
                        if (!_handlesByBusId.ContainsKey(busId)) {
                            _handlesByBusId.Add(busId, handles[i]);
                        }
                    }
                    handles = new NvPhysicalGpuHandle[NvapiConst.MAX_PHYSICAL_GPUS];
                    r = NvapiNativeMethods.NvEnumTCCPhysicalGPUs(handles, out gpuCount);
                    if (r != NvStatus.NVAPI_OK) {
                        Write.DevError(() => $"{nameof(NvapiNativeMethods.NvEnumTCCPhysicalGPUs)} {r.ToString()}");
                    }
                    for (int i = 0; i < gpuCount; i++) {
                        r = NvapiNativeMethods.NvGetBusID(handles[i], out int busId);
                        if (r != NvStatus.NVAPI_OK) {
                            Write.DevError(() => $"{nameof(NvapiNativeMethods.NvGetBusID)} {r.ToString()}");
                        }
                        if (!_handlesByBusId.ContainsKey(busId)) {
                            _handlesByBusId.Add(busId, handles[i]);
                        }
                    }
                    return _handlesByBusId;
                }
            }
        }

        public OverClockRange GetClockRange(int busId) {
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
                Write.DevWarn(() => $"GetClockRange {result.ToString()}");
#endif
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            return result;
        }

        public bool SetCoreClock(int busId, int mHz, int voltage) {
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
                        Write.DevError(() => $"{nameof(SetCoreClock)} {r.ToString()}");
                    }
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

                    var r = NvSetPStateV2(busId, ref info);
                    if (!r) {
                        Write.DevError(() => $"{nameof(SetMemoryClock)} {r.ToString()}");
                    }
                }
                return false;
            }
            catch {
            }
            return false;
        }

        public bool SetTempLimit(int busId, int value) {
            value <<= 8;
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

        public bool SetPowerLimit(int busId, int powerValue) {
            powerValue *= 1000;
            return SetPowerValue(busId, (uint)powerValue);
        }

        public bool GetFanSpeed(int busId, out uint currCooler) {
            if (GetCooler(busId, out _, out currCooler, out _)) {
                return true;
            }
            return false;
        }

        public bool SetFanSpeed(int busId, int value, bool isAutoMode) {
            return SetCooler(busId, (uint)value, isAutoMode);
        }

        #region private methods
        private bool NvPowerPoliciesSetStatus(int busId, ref NvGpuPowerStatus info) {
            if (NvapiNativeMethods.NvPowerPoliciesSetStatus == null) {
                return false;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return false;
                }
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NvGpuPowerStatus))));
                var r = NvapiNativeMethods.NvPowerPoliciesSetStatus(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    Write.DevError(() => $"{nameof(NvapiNativeMethods.NvPowerPoliciesSetStatus)} {r.ToString()}");
                }
                if (r == NvStatus.NVAPI_OK) {
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

        private bool NvGetPStateV1(int busId, out NvGpuPerfPStates20InfoV1 info) {
            info = new NvGpuPerfPStates20InfoV1();
            if (NvapiNativeMethods.NvGetPStateV1 == null) {
                return false;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return false;
                }
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NvGpuPerfPStates20InfoV1))));
                var r = NvapiNativeMethods.NvGetPStateV1(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    Write.DevError(() => $"{nameof(NvapiNativeMethods.NvGetPStateV1)} {r.ToString()}");
                    return false;
                }
                return true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return false;
            }
        }

        private bool NvGetPStateV2(int busId, out NvGpuPerfPStates20InfoV2 info) {
            info = new NvGpuPerfPStates20InfoV2();
            if (NvapiNativeMethods.NvGetPStateV2 == null) {
                return false;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return false;
                }
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NvGpuPerfPStates20InfoV2))));
                var r = NvapiNativeMethods.NvGetPStateV2(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    Write.DevError(() => $"{nameof(NvapiNativeMethods.NvGetPStateV2)} {r.ToString()}");
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
            if (NvapiNativeMethods.NvSetPStateV2 == null) {
                return false;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return false;
                }
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NvGpuPerfPStates20InfoV2))));
                var r = NvapiNativeMethods.NvSetPStateV2(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    Write.DevError(() => $"{nameof(NvapiNativeMethods.NvSetPStateV2)} {r.ToString()}");
                }
                if (r == NvStatus.NVAPI_OK) {
                    return true;
                }
            }
            catch {
            }
            return false;
        }

        private bool NvSetPStateV1(int busId, ref NvGpuPerfPStates20InfoV1 info) {
            if (NvapiNativeMethods.NvSetPStateV1 == null) {
                return false;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return false;
                }
                int len = Marshal.SizeOf(typeof(NvGpuPerfPStates20InfoV1));
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NvGpuPerfPStates20InfoV1))));
                var r = NvapiNativeMethods.NvSetPStateV1(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    Write.DevError(() => $"{nameof(NvapiNativeMethods.NvSetPStateV1)} {r.ToString()}");
                }
                if (r == NvStatus.NVAPI_OK) {
                    return true;
                }
            }
            catch {
            }
            return false;
        }

        private NvGpuClockFrequenciesV2 NvGetAllClockFrequenciesV2(int busId, uint type) {
            NvGpuClockFrequenciesV2 info = new NvGpuClockFrequenciesV2();
            if (NvapiNativeMethods.NvGetAllClockFrequenciesV2 == null) {
                return info;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return info;
                }
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NvGpuClockFrequenciesV2))));
                info.ClockType = type;
                var r = NvapiNativeMethods.NvGetAllClockFrequenciesV2(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    Write.DevError(() => $"{nameof(NvapiNativeMethods.NvGetAllClockFrequenciesV2)} {r.ToString()}");
                }
                if (r == NvStatus.NVAPI_OK) {
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
            if (NvapiNativeMethods.NvThermalPoliciesGetInfo == null) {
                return false;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return false;
                }
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NvGpuThermalInfo))));
                var r = NvapiNativeMethods.NvThermalPoliciesGetInfo(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    Write.DevError(() => $"{nameof(NvapiNativeMethods.NvThermalPoliciesGetInfo)} {r.ToString()}");
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
            if (NvapiNativeMethods.NvThermalPoliciesGetLimit == null) {
                return info;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return info;
                }
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NvGpuThermalLimit))));
                var r = NvapiNativeMethods.NvThermalPoliciesGetLimit(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    Write.DevError(() => $"{nameof(NvapiNativeMethods.NvThermalPoliciesGetLimit)} {r.ToString()}");
                }
                if (r == NvStatus.NVAPI_OK) {
                    return info;
                }
            }
            catch {
            }
            return info;
        }

        private bool NvThermalPoliciesSetLimit(int busId, ref NvGpuThermalLimit info) {
            if (NvapiNativeMethods.NvThermalPoliciesSetLimit == null) {
                return false;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return false;
                }
                info.version = (uint)(VERSION2 | (Marshal.SizeOf(typeof(NvGpuThermalLimit))));
                var r = NvapiNativeMethods.NvThermalPoliciesSetLimit(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    Write.DevError(() => $"{nameof(NvapiNativeMethods.NvThermalPoliciesSetLimit)} {r.ToString()}");
                }
                if (r == NvStatus.NVAPI_OK) {
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
            if (GetTempLimit(busId, out _, out _, out int defValue, out _)) {
                SetTempLimit(busId, defValue);
            }
        }

        private bool NvPowerPoliciesGetStatus(int busId, out NvGpuPowerStatus info) {
            info = new NvGpuPowerStatus();
            if (NvapiNativeMethods.NvPowerPoliciesGetStatus == null) {
                return false;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return false;
                }
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NvGpuPowerStatus))));
                var r = NvapiNativeMethods.NvPowerPoliciesGetStatus(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    Write.DevError(() => $"{nameof(NvapiNativeMethods.NvPowerPoliciesGetStatus)} {r.ToString()}");
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
            if (NvapiNativeMethods.NvPowerPoliciesGetInfo == null) {
                return false;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return false;
                }
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NvGpuPowerInfo))));
                var r = NvapiNativeMethods.NvPowerPoliciesGetInfo(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    Write.DevError(() => $"{nameof(NvapiNativeMethods.NvPowerPoliciesGetInfo)} {r.ToString()}");
                    return false;
                }
                return true;
            }
            catch {
                return false;
            }
        }

        private bool SetDefaultPowerLimit(int busId) {
            if (GetPowerLimit(busId, out _, out _, out uint defPower, out _)) {
                return SetPowerValue(busId, defPower * 1000);
            }
            return false;
        }

        private readonly HashSet<int> _nvFanCoolersGetStatusNotSupporteds = new HashSet<int>();
        private bool GetFanCoolersGetStatus(int busId, out PrivateFanCoolersStatusV1 info) {
            info = new PrivateFanCoolersStatusV1();
            if (NvapiNativeMethods.NvFanCoolersGetStatus == null) {
                return false;
            }
            if (_nvFanCoolersGetStatusNotSupporteds.Contains(busId)) {
                return false;
            }
            info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(PrivateFanCoolersStatusV1))));
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return false;
                }
                var r = NvapiNativeMethods.NvFanCoolersGetStatus(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    if (r == NvStatus.NVAPI_NOT_SUPPORTED || r == NvStatus.NVAPI_FIRMWARE_REVISION_NOT_SUPPORTED) {
                        _nvFanCoolersGetStatusNotSupporteds.Add(busId);
                    }
                    Write.DevError(() => $"{nameof(NvapiNativeMethods.NvFanCoolersGetStatus)} {r.ToString()}");
                    return false;
                }
                return true;
            }
            catch {
            }
            return false;
        }

        private readonly HashSet<int> _nvGetCoolerSettingsNotSupporteds = new HashSet<int>();
        private bool GetCoolerSettings(int busId, out NvCoolerSettings info) {
            info = new NvCoolerSettings();
            if (NvapiNativeMethods.NvGetCoolerSettings == null) {
                return false;
            }
            if (_nvGetCoolerSettingsNotSupporteds.Contains(busId)) {
                return false;
            }
            info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NvCoolerSettings))));
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
                    Write.DevError(() => $"{nameof(NvapiNativeMethods.NvGetCoolerSettings)} {r.ToString()}");
                    return false;
                }
                return true;
            }
            catch {
            }
            return false;
        }

        private bool NvFanCoolersGetControl(int busId, out PrivateFanCoolersControlV1 info) {
            info = new PrivateFanCoolersControlV1();
            if (NvapiNativeMethods.NvFanCoolersGetControl == null) {
                return false;
            }
            try {
                if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                    return false;
                }
                info.version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(PrivateFanCoolersControlV1))));
                var r = NvapiNativeMethods.NvFanCoolersGetControl(handle, ref info);
                if (r != NvStatus.NVAPI_OK) {
                    Write.DevError(() => $"{nameof(NvapiNativeMethods.NvFanCoolersGetControl)} {r.ToString()}");
                    return false;
                }
                return true;
            }
            catch {
            }
            return false;
        }

        private bool SetCooler(int busId, uint value, bool isAutoMode) {
            if (!HandlesByBusId.TryGetValue(busId, out NvPhysicalGpuHandle handle)) {
                return false;
            }
            #region GTX
            if (NvapiNativeMethods.NvSetCoolerLevels != null) {
                try {
                    NvCoolerTarget coolerIndex = NvCoolerTarget.NVAPI_COOLER_TARGET_ALL;
                    NvCoolerLevel info = new NvCoolerLevel() {
                        version = (uint)(VERSION1 | (Marshal.SizeOf(typeof(NvCoolerLevel)))),
                        coolers = new NvCoolerLevelItem[NvapiConst.NVAPI_MAX_COOLERS_PER_GPU]
                    };
                    info.coolers[0].currentLevel = isAutoMode ? 0 : value;
                    info.coolers[0].currentPolicy = isAutoMode ? NvCoolerPolicy.NVAPI_COOLER_POLICY_AUTO : NvCoolerPolicy.NVAPI_COOLER_POLICY_MANUAL;
                    var r = NvapiNativeMethods.NvSetCoolerLevels(handle, coolerIndex, ref info);
                    if (r != NvStatus.NVAPI_OK) {
                        Write.DevError(() => $"{nameof(NvapiNativeMethods.NvSetCoolerLevels)} {r.ToString()}");
                    }
                    else {
                        return true;
                    }
                }
                catch {
                }
            }
            #endregion

            #region RTX
            if (NvapiNativeMethods.NvFanCoolersSetControl == null) {
                return false;
            }
            try {
                if (NvFanCoolersGetControl(busId, out PrivateFanCoolersControlV1 info)) {
                    for (int i = 0; i < info.FanCoolersControlCount; i++) {
                        info.FanCoolersControlEntries[i].ControlMode = isAutoMode ? FanCoolersControlMode.Auto : FanCoolersControlMode.Manual;
                        info.FanCoolersControlEntries[i].Level = isAutoMode ? 0u : (uint)value;
                    }
                    var r = NvapiNativeMethods.NvFanCoolersSetControl(handle, ref info);
                    if (r != NvStatus.NVAPI_OK) {
                        Write.DevError(() => $"{nameof(NvapiNativeMethods.NvFanCoolersSetControl)} {r.ToString()}");
                        return false;
                    }
                    return true;
                }
                return false;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return false;
            }
            #endregion
        }

        private bool GetCooler(int busId, out uint minCooler, out uint currCooler, out uint maxCooler) {
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
