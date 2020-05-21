using NTMiner.Gpus.Adl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Gpus {
    public class AdlHelper : IGpuHelper {
        public struct ATIGPU {
            public static readonly ATIGPU Empty = new ATIGPU {
                AdapterIndex = -1,
                BusNumber = -1,
                AdapterName = string.Empty,
                DeviceNumber = -1
            };

            public int AdapterIndex { get; set; }
            public int BusNumber { get; set; }
            public int DeviceNumber { get; set; }
            public string AdapterName { get; set; }

            public override string ToString() {
                return $"AdapterIndex={AdapterIndex.ToString()},BusNumber={BusNumber.ToString()},DeviceNumber={DeviceNumber.ToString()},AdapterName={AdapterName}";
            }
        }

        public AdlHelper() { }

        private IntPtr context;
        private List<ATIGPU> _gpuNames = new List<ATIGPU>();
        public bool Init() {
            try {
                var r = AdlNativeMethods.ADL_Main_Control_Create(1);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"ADL_Main_Control_Create {r.ToString()}");
                }
                if (r >= AdlStatus.ADL_OK) {
                    int numberOfAdapters = 0;
                    r = AdlNativeMethods.ADL_Adapter_NumberOfAdapters_Get(ref numberOfAdapters);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL_Adapter_NumberOfAdapters_Get)} {r.ToString()}");
                    }
                    if (numberOfAdapters > 0) {
                        ADLAdapterInfo[] adapterInfo = new ADLAdapterInfo[numberOfAdapters];
                        if (AdlNativeMethods.ADL_Adapter_AdapterInfo_Get(adapterInfo) >= AdlStatus.ADL_OK) {
                            for (int i = 0; i < numberOfAdapters; i++) {
                                NTMinerConsole.DevDebug(() => adapterInfo[i].ToString());
                                if (!string.IsNullOrEmpty(adapterInfo[i].UDID) && adapterInfo[i].VendorID == AdlConst.ATI_VENDOR_ID) {
                                    bool found = false;
                                    foreach (ATIGPU gpu in _gpuNames) {
                                        if (gpu.BusNumber == adapterInfo[i].BusNumber && gpu.DeviceNumber == adapterInfo[i].DeviceNumber) {
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found)
                                        _gpuNames.Add(new ATIGPU {
                                            AdapterName = adapterInfo[i].AdapterName.Trim(),
                                            AdapterIndex = adapterInfo[i].AdapterIndex,
                                            BusNumber = adapterInfo[i].BusNumber,
                                            DeviceNumber = adapterInfo[i].DeviceNumber
                                        });
                                }
                            }
                        }
                    }
                    r = AdlNativeMethods.ADL2_Main_Control_Create(AdlNativeMethods.Main_Memory_Alloc, 1, ref context);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_Main_Control_Create)} {r.ToString()}");
                    }
                }
                _gpuNames = _gpuNames.OrderBy(a => a.BusNumber).ToList();
                NTMinerConsole.DevDebug(() => string.Join(",", _gpuNames.Select(a => a.AdapterIndex)));
            }
            catch {
                return false;
            }

            return true;
        }

        public int GpuCount {
            get { return _gpuNames.Count; }
        }

        public Version GetDriverVersion() {
            ADLVersionsInfoX2 info = new ADLVersionsInfoX2();
            try {
                var r = AdlNativeMethods.ADL2_Graphics_VersionsX2_Get(context, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_Graphics_VersionsX2_Get)} {r.ToString()}");
                }
                if (string.IsNullOrEmpty(info.strCrimsonVersion) || !Version.TryParse(info.strCrimsonVersion, out Version v)) {
                    return new Version();
                }
                return v;
            }
            catch {
                return new Version();
            }
        }

        // 将GPUIndex转换为AdapterIndex
        private bool TryGpuAdapterIndex(int gpuIndex, out int adapterIndex) {
            adapterIndex = 0;
            if (gpuIndex < 0 || gpuIndex >= _gpuNames.Count) {
                return false;
            }
            adapterIndex = _gpuNames[gpuIndex].AdapterIndex;
            return true;
        }

        public ATIGPU GetGpuName(int gpuIndex) {
            try {
                if (gpuIndex >= _gpuNames.Count) {
                    return ATIGPU.Empty;
                }
                return _gpuNames[gpuIndex];
            }
            catch {
                return ATIGPU.Empty;
            }
        }

        public OverClockRange GetClockRange(int gpuIndex) {
            OverClockRange result = new OverClockRange(gpuIndex);
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return result;
                }
                ADLODNCapabilitiesX2 info = new ADLODNCapabilitiesX2();
                var r = AdlNativeMethods.ADL2_OverdriveN_CapabilitiesX2_Get(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_CapabilitiesX2_Get)} {r.ToString()}");
                    return result;
                }
                result.PowerCurr = GetPowerLimit(gpuIndex);
                result.TempCurr = GetTempLimit(gpuIndex);
                if (GetMemoryClock(gpuIndex, out int memoryClock, out int iVddc)) {
                    result.MemoryClockDelta = memoryClock;
                    result.MemoryVoltage = iVddc;
                }
                if (GetCoreClock(gpuIndex, out int coreClock, out iVddc)) {
                    result.CoreClockDelta = coreClock;
                    result.CoreVoltage = iVddc;
                }
                result.CoreClockMin = info.sEngineClockRange.iMin * 10;
                result.CoreClockMax = info.sEngineClockRange.iMax * 10;
                result.MemoryClockMin = info.sMemoryClockRange.iMin * 10;
                result.MemoryClockMax = info.sMemoryClockRange.iMax * 10;
                result.PowerMin = info.power.iMin + 100;
                result.PowerMax = info.power.iMax + 100;
                result.PowerDefault = info.power.iDefault + 100;
                result.TempLimitMin = info.powerTuneTemperature.iMin;
                result.TempLimitMax = info.powerTuneTemperature.iMax;
                result.TempLimitDefault = info.powerTuneTemperature.iDefault;
                result.VoltMin = info.svddcRange.iMin;
                result.VoltMax = info.svddcRange.iMax;
                result.VoltDefault = info.svddcRange.iDefault;
                if (info.fanSpeed.iMax == 0) {
                    result.FanSpeedMin = 0;
                }
                else {
                    result.FanSpeedMin = info.fanSpeed.iMin * 100 / info.fanSpeed.iMax;
                }
                result.FanSpeedMax = 100;
#if DEBUG
                NTMinerConsole.DevWarn(() => $"GetClockRange {result.ToString()}");
#endif
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            return result;
        }

        public bool GetCoreClock(int gpuIndex, out int coreClock, out int iVddc) {
            coreClock = 0;
            iVddc = 0;
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return false;
                }
                ADLODNPerformanceLevelsX2 info = ADLODNPerformanceLevelsX2.Create();
                var r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get)} {r.ToString()}");
                    return false;
                }
                int index = 0;
                for (int i = 0; i < info.aLevels.Length; i++) {
                    if (info.aLevels[i].iEnabled != 0) {
                        index = i;
                    }
                    NTMinerConsole.DevWarn(() => "GetCoreClock " + info.aLevels[i].ToString());
                }
                coreClock = info.aLevels[index].iClock * 10;
                iVddc = info.aLevels[index].iVddc;
                return true;
            }
            catch {
                return false;
            }
        }

        public bool SetCoreClock(int gpuIndex, int value, int voltage) {
            if (value < 0) {
                value = 0;
            }
            if (voltage < 0) {
                voltage = 0;
            }
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return false;
                }
                ADLODNPerformanceLevelsX2 info = ADLODNPerformanceLevelsX2.Create();
                var r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get)} {r.ToString()}");
                    return false;
                }
                info.iMode = AdlConst.ODNControlType_Default;
                r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Set(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Set)} {r.ToString()}");
                    return false;
                }
                bool isReset = value == 0 && voltage == 0;
                if (isReset) {
                    return true;
                }
                r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get)} {r.ToString()}");
                    return false;
                }
                info.iMode = AdlConst.ODNControlType_Manual;
                int index = 0;
                for (int i = 0; i < info.aLevels.Length; i++) {
                    if (info.aLevels[i].iEnabled != 0) {
                        index = i;
                    }
                }
                NTMinerConsole.DevDebug(() => $"SetCoreClock PState {index.ToString()} value={value.ToString()} voltage={voltage.ToString()}");
                if (value != 0) {
                    info.aLevels[index].iClock = value * 100;
                }
                if (voltage != 0) {
                    info.aLevels[index].iVddc = voltage;
                }
                r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Set(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Set)} {r.ToString()}");
                    return false;
                }
                return true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return false;
            }
        }

        public bool GetMemoryClock(int gpuIndex, out int memoryClock, out int iVddc) {
            memoryClock = 0;
            iVddc = 0;
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return false;
                }
                ADLODNPerformanceLevelsX2 info = ADLODNPerformanceLevelsX2.Create();
                var r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get)} {r.ToString()}");
                    return false;
                }
                int index = 0;
                for (int i = 0; i < info.aLevels.Length; i++) {
                    if (info.aLevels[i].iEnabled != 0) {
                        index = i;
                    }
                    NTMinerConsole.DevWarn(() => "GetMemoryClock " + info.aLevels[i].ToString());
                }
                memoryClock = info.aLevels[index].iClock * 10;
                iVddc = info.aLevels[index].iVddc;
                return true;
            }
            catch {
                return false;
            }
        }

        public bool SetMemoryClock(int gpuIndex, int value, int voltage) {
            if (value < 0) {
                value = 0;
            }
            if (voltage < 0) {
                voltage = 0;
            }
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return false;
                }
                ADLODNPerformanceLevelsX2 info = ADLODNPerformanceLevelsX2.Create();
                var r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get)} {r.ToString()}");
                    return false;
                }
                info.iMode = AdlConst.ODNControlType_Default;
                r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Set(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Set)} {r.ToString()}");
                    return false;
                }
                bool isReset = value == 0 && voltage == 0;
                if (isReset) {
                    return true;
                }
                r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get)} {r.ToString()}");
                    return false;
                }
                info.iMode = AdlConst.ODNControlType_Manual;
                int index = 0;
                for (int i = 0; i < info.aLevels.Length; i++) {
                    if (info.aLevels[i].iEnabled != 0) {
                        index = i;
                    }
                }
                NTMinerConsole.DevDebug(() => $"SetMemoryClock PState {index.ToString()} value={value.ToString()} voltage={voltage.ToString()}");
                if (value != 0) {
                    info.aLevels[index].iClock = value * 100;
                }
                if (voltage != 0) {
                    info.aLevels[index].iVddc = voltage;
                }
                r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Set(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Set)} {r.ToString()}");
                    return false;
                }
                return true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return false;
            }
        }

        public ulong GetTotalMemory(int gpuIndex) {
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return 0;
                }
                ADLMemoryInfo info = new ADLMemoryInfo();
                var r = AdlNativeMethods.ADL_Adapter_MemoryInfo_Get(adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL_Adapter_MemoryInfo_Get)} {r.ToString()}");
                    return 0;
                }
                return info.MemorySize;
            }
            catch {
                return 0;
            }
        }

        public int GetTemperature(int gpuIndex) {
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return 0;
                }
                ADLTemperature info = new ADLTemperature();
                var r = AdlNativeMethods.ADL_Overdrive5_Temperature_Get(adapterIndex, 0, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL_Overdrive5_Temperature_Get)} {r.ToString()}");
                    return 0;
                }
                return (int)(0.001f * info.Temperature);
            }
            catch {
                return 0;
            }
        }

        public uint GetFanSpeed(int gpuIndex) {
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return 0;
                }
                ADLFanSpeedValue info = new ADLFanSpeedValue {
                    SpeedType = AdlConst.ADL_DL_FANCTRL_SPEED_TYPE_PERCENT
                };
                var r = AdlNativeMethods.ADL_Overdrive5_FanSpeed_Get(adapterIndex, 0, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL_Overdrive5_FanSpeed_Get)} {r.ToString()}");
                    return 0;
                }
                return (uint)info.FanSpeed;
            }
            catch {
                return 0;
            }
        }

        public bool SetFanSpeed(int gpuIndex, int value, bool isAutoMode) {
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return false;
                }
                AdlStatus r;
                if (isAutoMode) {
                    r = AdlNativeMethods.ADL2_Overdrive5_FanSpeedToDefault_Set(context, adapterIndex, 0);
                    if (r != AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_Overdrive5_FanSpeedToDefault_Set)} {r.ToString()}");
                        r = AdlNativeMethods.ADL2_Overdrive6_FanSpeed_Reset(context, adapterIndex);
                        if (r < AdlStatus.ADL_OK) {
                            NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_Overdrive6_FanSpeed_Reset)} {r.ToString()}");
                        }
                    }
                    return true;
                }
                ADLFanSpeedValue info = new ADLFanSpeedValue {
                    SpeedType = AdlConst.ADL_DL_FANCTRL_SPEED_TYPE_PERCENT
                };
                r = AdlNativeMethods.ADL_Overdrive5_FanSpeed_Get(adapterIndex, 0, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL_Overdrive5_FanSpeed_Get)} {r.ToString()}");
                    return false;
                }
                info.FanSpeed = value;
                r = AdlNativeMethods.ADL_Overdrive5_FanSpeed_Set(adapterIndex, 0, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL_Overdrive5_FanSpeed_Set)} {r.ToString()}");
                    return false;
                }
                return true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return false;
            }
        }

        public int GetPowerLimit(int gpuIndex) {
            if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                return 0;
            }
            ADLODNPowerLimitSetting info = new ADLODNPowerLimitSetting();
            try {
                var r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get)} {r.ToString()}");
                    return 0;
                }
                return 100 + info.iTDPLimit;
            }
            catch {
                return 0;
            }
        }

        public bool SetPowerLimit(int gpuIndex, int value) {
            if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                return false;
            }
            ADLODNPowerLimitSetting info = new ADLODNPowerLimitSetting();
            try {
                var r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get)} {r.ToString()}");
                    return false;
                }
                NTMinerConsole.DevWarn(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get)} result={r.ToString()},iMode={info.iMode.ToString()},iTDPLimit={info.iTDPLimit.ToString()},iMaxOperatingTemperature={info.iMaxOperatingTemperature.ToString()}");
                info.iMode = AdlConst.ODNControlType_Manual;
                info.iTDPLimit = value - 100;
                r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Set(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Set)} {r.ToString()}");
                    return false;
                }
                return true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return false;
            }
        }

        public int GetTempLimit(int gpuIndex) {
            if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                return 0;
            }
            ADLODNPowerLimitSetting info = new ADLODNPowerLimitSetting();
            try {
                var r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get)} {r.ToString()}");
                    return 0;
                }
                return info.iMaxOperatingTemperature;
            }
            catch {
                return 0;
            }
        }

        public bool SetTempLimit(int gpuIndex, int value) {
            if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                return false;
            }
            ADLODNPowerLimitSetting info = new ADLODNPowerLimitSetting();
            try {
                var r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get)} {r.ToString()}");
                    return false;
                }
                bool isAutoModel = value == 0;
                info.iMode = isAutoModel ? AdlConst.ODNControlType_Auto : AdlConst.ODNControlType_Manual;
                info.iMaxOperatingTemperature = isAutoModel ? -1 : value;
                r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Set(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Set)} {r.ToString()}");
                    return false;
                }
                return true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return false;
            }
        }

        public uint GetPowerUsage(int gpuIndex) {
            if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                return 0;
            }
            int power = 0;
            try {
                var r = AdlNativeMethods.ADL2_Overdrive6_CurrentPower_Get(context, adapterIndex, 0, ref power);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_Overdrive6_CurrentPower_Get)} {r.ToString()}");
                    return 0;
                }
                return (uint)(power / 256.0);
            }
            catch {
            }
            return 0;
        }
    }
}
