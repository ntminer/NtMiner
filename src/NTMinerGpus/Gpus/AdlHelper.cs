using NTMiner.Gpus.Adl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Gpus {
    public class AdlHelper {
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
                return $"AdapterIndex={AdapterIndex},BusNumber={BusNumber},DeviceNumber={DeviceNumber},AdapterName={AdapterName}";
            }
        }

        public AdlHelper() { }

        private IntPtr context;
        private List<ATIGPU> _gpuNames = new List<ATIGPU>();
        public bool Init() {
            try {
                var r = AdlNativeMethods.ADL_Main_Control_Create(1);
                if (r < AdlStatus.ADL_OK) {
                    Write.DevError($"ADL_Main_Control_Create {r}");
                }
                if (r >= AdlStatus.ADL_OK) {
                    int numberOfAdapters = 0;
                    r = AdlNativeMethods.ADL_Adapter_NumberOfAdapters_Get(ref numberOfAdapters);
                    if (r < AdlStatus.ADL_OK) {
                        Write.DevError($"{nameof(AdlNativeMethods.ADL_Adapter_NumberOfAdapters_Get)} {r}");
                    }
                    if (numberOfAdapters > 0) {
                        ADLAdapterInfo[] adapterInfo = new ADLAdapterInfo[numberOfAdapters];
                        if (AdlNativeMethods.ADL_Adapter_AdapterInfo_Get(adapterInfo) >= AdlStatus.ADL_OK) {
                            for (int i = 0; i < numberOfAdapters; i++) {
#if DEBUG
                                Write.DevDebug(adapterInfo[i].ToString());
#endif
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
                        Write.DevError($"{nameof(AdlNativeMethods.ADL2_Main_Control_Create)} {r}");
                    }
                }
                _gpuNames = _gpuNames.OrderBy(a => a.BusNumber).ToList();
#if DEBUG
                Write.DevDebug(string.Join(",", _gpuNames.Select(a => a.AdapterIndex)));
#endif
            }
            catch {
                return false;
            }

            return true;
        }

        public int GpuCount {
            get { return _gpuNames.Count; }
        }

        public string GetDriverVersion() {
            ADLVersionsInfoX2 info = new ADLVersionsInfoX2();
            try {
                var r = AdlNativeMethods.ADL2_Graphics_VersionsX2_Get(context, ref info);
                if (r < AdlStatus.ADL_OK) {
                    Write.DevError($"{nameof(AdlNativeMethods.ADL2_Graphics_VersionsX2_Get)} {r}");
                }
                return info.strCrimsonVersion;
            }
            catch {
                return "0.0";
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

        public void GetClockRange(
            int gpuIndex,
            out int coreClockMin, out int coreClockMax,
            out int memoryClockMin, out int memoryClockMax,
            out int voltMin, out int voltMax, out int voltDefault,
            out int powerMin, out int powerMax, out int powerDefault,
            out int tempLimitMin, out int tempLimitMax, out int tempLimitDefault,
            out int fanSpeedMin, out int fanSpeedMax, out int fanSpeedDefault) {
            coreClockMin = 0;
            coreClockMax = 0;
            memoryClockMin = 0;
            memoryClockMax = 0;
            powerMin = 0;
            powerMax = 0;
            powerDefault = 0;
            tempLimitMin = 0;
            tempLimitMax = 0;
            tempLimitDefault = 0;
            fanSpeedMin = 0;
            fanSpeedMax = 0;
            fanSpeedDefault = 0;
            voltMin = 0;
            voltMax = 0;
            voltDefault = 0;
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return;
                }
                ADLODNCapabilitiesX2 info = new ADLODNCapabilitiesX2();
                var r = AdlNativeMethods.ADL2_OverdriveN_CapabilitiesX2_Get(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    Write.DevError($"{nameof(AdlNativeMethods.ADL2_OverdriveN_CapabilitiesX2_Get)} {r}");
                    return;
                }
                coreClockMin = info.sEngineClockRange.iMin * 10;
                coreClockMax = info.sEngineClockRange.iMax * 10;
                memoryClockMin = info.sMemoryClockRange.iMin * 10;
                memoryClockMax = info.sMemoryClockRange.iMax * 10;
                powerMin = info.power.iMin + 100;
                powerMax = info.power.iMax + 100;
                powerDefault = info.power.iDefault + 100;
                tempLimitMin = info.powerTuneTemperature.iMin;
                tempLimitMax = info.powerTuneTemperature.iMax;
                tempLimitDefault = info.powerTuneTemperature.iDefault;
                voltMin = info.svddcRange.iMin;
                voltMax = info.svddcRange.iMax;
                voltDefault = info.svddcRange.iDefault;
                if (info.fanSpeed.iMax == 0) {
                    fanSpeedMin = 0;
                }
                else {
                    fanSpeedMin = info.fanSpeed.iMin * 100 / info.fanSpeed.iMax;
                }
                fanSpeedMax = 100;
                fanSpeedDefault = info.fanSpeed.iDefault;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
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
                    Write.DevError($"{nameof(AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get)} {r}");
                    return false;
                }
                int index = 0;
                for (int i = 0; i < info.aLevels.Length; i++) {
                    if (info.aLevels[i].iEnabled != 0) {
                        index = i;
                    }
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
            if (value <= 0) {
                return false;
            }
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return false;
                }
                ADLODNPerformanceLevelsX2 info = ADLODNPerformanceLevelsX2.Create();
                var r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    Write.DevError($"{nameof(AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get)} {r}");
                    return false;
                }
                info.iMode = AdlConst.ODNControlType_Default;
                r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Set(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    Write.DevError($"{nameof(AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Set)} {r}");
                    return false;
                }
                r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    Write.DevError($"{nameof(AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get)} {r}");
                    return false;
                }
                info.iMode = AdlConst.ODNControlType_Manual;
                int index = 0;
                for (int i = 0; i < info.aLevels.Length; i++) {
                    if (info.aLevels[i].iEnabled == 1) {
                        index = i;
                    }
                }
                info.aLevels[index].iClock = value * 100;
                if (voltage != 0) {
                    info.aLevels[index].iVddc = voltage;
                }
                r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Set(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    Write.DevError($"{nameof(AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Set)} {r}");
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
                    Write.DevError($"{nameof(AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get)} {r}");
                    return false;
                }
                int index = 0;
                for (int i = 0; i < info.aLevels.Length; i++) {
                    if (info.aLevels[i].iEnabled != 0) {
                        index = i;
                    }
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
            if (value <= 0) {
                return false;
            }
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return false;
                }
                ADLODNPerformanceLevelsX2 info = ADLODNPerformanceLevelsX2.Create();
                var r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    Write.DevError($"{nameof(AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get)} {r}");
                    return false;
                }
                info.iMode = AdlConst.ODNControlType_Default;
                r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Set(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    Write.DevError($"{nameof(AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Set)} {r}");
                    return false;
                }
                r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    Write.DevError($"{nameof(AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get)} {r}");
                    return false;
                }
                info.iMode = AdlConst.ODNControlType_Manual;
                int index = 0;
                for (int i = 0; i < info.aLevels.Length; i++) {
                    if (info.aLevels[i].iEnabled == 1) {
                        index = i;
                    }
                }
                info.aLevels[index].iClock = value * 100;
                if (voltage != 0) {
                    info.aLevels[index].iVddc = voltage;
                }
                r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Set(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    Write.DevError($"{nameof(AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Set)} {r}");
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
                    Write.DevError($"{nameof(AdlNativeMethods.ADL_Adapter_MemoryInfo_Get)} {r}");
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
                    Write.DevError($"{nameof(AdlNativeMethods.ADL_Overdrive5_Temperature_Get)} {r}");
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
                    Write.DevError($"{nameof(AdlNativeMethods.ADL_Overdrive5_FanSpeed_Get)} {r}");
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
                    if (r < AdlStatus.ADL_OK) {
                        Write.DevError($"{nameof(AdlNativeMethods.ADL2_Overdrive5_FanSpeedToDefault_Set)} {r}");
                        return false;
                    }
                    return true;
                }
                ADLFanSpeedValue info = new ADLFanSpeedValue {
                    SpeedType = AdlConst.ADL_DL_FANCTRL_SPEED_TYPE_PERCENT
                };
                r = AdlNativeMethods.ADL_Overdrive5_FanSpeed_Get(adapterIndex, 0, ref info);
                if (r < AdlStatus.ADL_OK) {
                    Write.DevError($"{nameof(AdlNativeMethods.ADL_Overdrive5_FanSpeed_Get)} {r}");
                    return false;
                }
                info.FanSpeed = value;
                r = AdlNativeMethods.ADL_Overdrive5_FanSpeed_Set(adapterIndex, 0, ref info);
                if (r < AdlStatus.ADL_OK) {
                    Write.DevError($"{nameof(AdlNativeMethods.ADL_Overdrive5_FanSpeed_Set)} {r}");
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
                    Write.DevError($"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get)} {r}");
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
                    Write.DevError($"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get)} {r}");
                    return false;
                }
#if DEBUG
                Write.DevWarn($"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get)} result={r},iMode={info.iMode},iTDPLimit={info.iTDPLimit},iMaxOperatingTemperature={info.iMaxOperatingTemperature}");
#endif
                info.iMode = AdlConst.ODNControlType_Manual;
                info.iTDPLimit = value - 100;
                r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Set(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    Write.DevError($"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Set)} {r}");
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
                    Write.DevError($"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get)} {r}");
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
                    Write.DevError($"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get)} {r}");
                    return false;
                }
                bool isAutoModel = value == 0;
                info.iMode = isAutoModel ? AdlConst.ODNControlType_Auto : AdlConst.ODNControlType_Manual;
                info.iMaxOperatingTemperature = isAutoModel ? -1 : value;
                r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Set(context, adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    Write.DevError($"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Set)} {r}");
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
                    Write.DevError($"{nameof(AdlNativeMethods.ADL2_Overdrive6_CurrentPower_Get)} {r}");
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
