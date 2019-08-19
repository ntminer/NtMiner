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
                DeviceNumber = - 1
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
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL_Main_Control_Create {r}");
                }
                if (r == AdlStatus.OK) {
                    int numberOfAdapters = 0;
                    r = AdlNativeMethods.ADL_Adapter_NumberOfAdapters_Get(ref numberOfAdapters);
                    if (r != AdlStatus.OK) {
                        Write.DevWarn($"ADL_Adapter_NumberOfAdapters_Get {r}");
                    }
                    if (numberOfAdapters > 0) {
                        ADLAdapterInfo[] adapterInfo = new ADLAdapterInfo[numberOfAdapters];
                        if (AdlNativeMethods.ADL_Adapter_AdapterInfo_Get(adapterInfo) == AdlStatus.OK) {
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
                    if (r != AdlStatus.OK) {
                        Write.DevWarn($"ADL2_Main_Control_Create {r}");
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
            ADLVersionsInfoX2 lpVersionInfo = new ADLVersionsInfoX2();
            try {
                var r = AdlNativeMethods.ADL2_Graphics_VersionsX2_Get(context, ref lpVersionInfo);
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL2_Graphics_VersionsX2_Get {r}");
                }
                return lpVersionInfo.strCrimsonVersion;
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

        public void GetClockRangeByIndex(
            int gpuIndex, 
            out int coreClockMin, out int coreClockMax, 
            out int memoryClockMin, out int memoryClockMax, 
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
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return;
                }
                ADLODNCapabilitiesX2 lpODCapabilities = new ADLODNCapabilitiesX2();
                var r = AdlNativeMethods.ADL2_OverdriveN_CapabilitiesX2_Get(context, adapterIndex, ref lpODCapabilities);
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL2_OverdriveN_CapabilitiesX2_Get {r}");
                }
                coreClockMin = lpODCapabilities.sEngineClockRange.iMin * 10;
                coreClockMax = lpODCapabilities.sEngineClockRange.iMax * 10;
                memoryClockMin = lpODCapabilities.sMemoryClockRange.iMin * 10;
                memoryClockMax = lpODCapabilities.sMemoryClockRange.iMax * 10;
                powerMin = lpODCapabilities.power.iMin + 100;
                powerMax = lpODCapabilities.power.iMax + 100;
                powerDefault = lpODCapabilities.power.iDefault + 100;
                tempLimitMin = lpODCapabilities.powerTuneTemperature.iMin;
                tempLimitMax = lpODCapabilities.powerTuneTemperature.iMax;
                tempLimitDefault = lpODCapabilities.powerTuneTemperature.iDefault;
                if (lpODCapabilities.fanSpeed.iMax == 0) {
                    fanSpeedMin = 0;
                }
                else {
                    fanSpeedMin = lpODCapabilities.fanSpeed.iMin * 100 / lpODCapabilities.fanSpeed.iMax;
                }
                fanSpeedMax = 100;
                fanSpeedDefault = lpODCapabilities.fanSpeed.iDefault;
#if DEBUG
                Write.DevWarn($"ADL2_OverdriveN_CapabilitiesX2_Get result {r} coreClockMin={coreClockMin},coreClockMax={coreClockMax},memoryClockMin={memoryClockMin},memoryClockMax={memoryClockMax},powerMin={powerMin},powerMax={powerMax},powerDefault={powerDefault},tempLimitMin={tempLimitMin},tempLimitMax={tempLimitMax},tempLimitDefault={tempLimitDefault},fanSpeedMin={fanSpeedMin},fanSpeedMax={fanSpeedMax},fanSpeedDefault={fanSpeedDefault}");
#endif
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        public ulong GetTotalMemoryByIndex(int gpuIndex) {
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return 0;
                }
                ADLMemoryInfo adlt = new ADLMemoryInfo();
                var r = AdlNativeMethods.ADL_Adapter_MemoryInfo_Get(adapterIndex, ref adlt);
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL_Adapter_MemoryInfo_Get {r}");
                }
                if (r == AdlStatus.OK) {
                    return adlt.MemorySize;
                }
                else {
                    return 0;
                }
            }
            catch {
                return 0;
            }
        }

        public int GetMemoryClockByIndex(int gpuIndex) {
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return 0;
                }
                ADLODNPerformanceLevelsX2 lpODPerformanceLevels = ADLODNPerformanceLevelsX2.Create();
                var r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get(context, adapterIndex, ref lpODPerformanceLevels);
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL2_OverdriveN_MemoryClocksX2_Get {r}");
                }
                int index = 0;
                for (int i = 0; i < lpODPerformanceLevels.aLevels.Length; i++) {
                    if (lpODPerformanceLevels.aLevels[i].iEnabled != 0) {
                        index = i;
                    }
                }
                return lpODPerformanceLevels.aLevels[index].iClock * 10;
            }
            catch {
                return 0;
            }
        }

        public void SetMemoryClockByIndex(int gpuIndex, int value) {
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return;
                }
                ADLODNCapabilitiesX2 lpODCapabilities = new ADLODNCapabilitiesX2();
                var r = AdlNativeMethods.ADL2_OverdriveN_CapabilitiesX2_Get(context, adapterIndex, ref lpODCapabilities);
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL2_OverdriveN_CapabilitiesX2_Get {r}");
                    return;
                }
                ADLODNPerformanceLevelsX2 lpODPerformanceLevels = ADLODNPerformanceLevelsX2.Create();
                r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get(context, adapterIndex, ref lpODPerformanceLevels);
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL2_OverdriveN_MemoryClocksX2_Get {r}");
                }
                lpODPerformanceLevels.iMode = AdlConst.ODNControlType_Default;
                r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Set(context, adapterIndex, ref lpODPerformanceLevels);
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL2_OverdriveN_MemoryClocksX2_Set {r}");
                }
                r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get(context, adapterIndex, ref lpODPerformanceLevels);
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL2_OverdriveN_MemoryClocksX2_Get {r}");
                }
#if DEBUG
                foreach (var item in lpODPerformanceLevels.aLevels) {
                    Write.DevWarn($"iClock={item.iClock},iControl={item.iControl},iEnabled={item.iEnabled},iVddc={item.iVddc}");
                }
#endif
                if (r == AdlStatus.OK) {
                    if (value <= 0) {
                        return;
                    }
                    else {
                        lpODPerformanceLevels.iMode = AdlConst.ODNControlType_Manual;
                        int index = 0;
                        for (int i = 0; i < lpODPerformanceLevels.aLevels.Length; i++) {
                            if (lpODPerformanceLevels.aLevels[i].iEnabled == 1) {
                                index = i;
                            }
                        }
                        lpODPerformanceLevels.aLevels[index].iClock = value * 100;
                    }
                    r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Set(context, adapterIndex, ref lpODPerformanceLevels);
                    if (r != AdlStatus.OK) {
                        Write.DevWarn($"ADL2_OverdriveN_MemoryClocksX2_Set {r}");
                    }
                }
            }
            catch {
            }
        }

        public int GetSystemClockByIndex(int gpuIndex) {
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return 0;
                }
                ADLODNPerformanceLevelsX2 lpODPerformanceLevels = ADLODNPerformanceLevelsX2.Create();
                var r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get(context, adapterIndex, ref lpODPerformanceLevels);
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL2_OverdriveN_SystemClocksX2_Get {r}");
                }
                int index = 0;
                for (int i = 0; i < lpODPerformanceLevels.aLevels.Length; i++) {
                    if (lpODPerformanceLevels.aLevels[i].iEnabled != 0) {
                        index = i;
                    }
                }
                return lpODPerformanceLevels.aLevels[index].iClock * 10;
            }
            catch {
                return 0;
            }
        }

        public void SetSystemClockByIndex(int gpuIndex, int value) {
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return;
                }
                ADLODNCapabilitiesX2 lpODCapabilities = new ADLODNCapabilitiesX2();
                var r = AdlNativeMethods.ADL2_OverdriveN_CapabilitiesX2_Get(context, adapterIndex, ref lpODCapabilities);
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL2_OverdriveN_SystemClocksX2_Get {r}");
                    return;
                }
                ADLODNPerformanceLevelsX2 lpODPerformanceLevels = ADLODNPerformanceLevelsX2.Create();
                r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get(context, adapterIndex, ref lpODPerformanceLevels);
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL2_OverdriveN_SystemClocksX2_Get {r}");
                }
                lpODPerformanceLevels.iMode = AdlConst.ODNControlType_Default;
                r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Set(context, adapterIndex, ref lpODPerformanceLevels);
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL2_OverdriveN_SystemClocksX2_Set {r}");
                }
                r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get(context, adapterIndex, ref lpODPerformanceLevels);
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL2_OverdriveN_SystemClocksX2_Get {r}");
                }
#if DEBUG
                foreach (var item in lpODPerformanceLevels.aLevels) {
                    Write.DevWarn($"iClock={item.iClock},iControl={item.iControl},iEnabled={item.iEnabled},iVddc={item.iVddc}");
                }
#endif
                if (r == AdlStatus.OK) {
                    if (value <= 0) {
                        return;
                    }
                    else {
                        lpODPerformanceLevels.iMode = AdlConst.ODNControlType_Manual;
                        int index = 0;
                        for (int i = 0; i < lpODPerformanceLevels.aLevels.Length; i++) {
                            if (lpODPerformanceLevels.aLevels[i].iEnabled == 1) {
                                index = i;
                            }
                        }
                        lpODPerformanceLevels.aLevels[index].iClock = value * 100;
                    }
                    r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Set(context, adapterIndex, ref lpODPerformanceLevels);
                    if (r != AdlStatus.OK) {
                        Write.DevWarn($"ADL2_OverdriveN_SystemClocksX2_Set {r}");
                    }
                }
            }
            catch {
            }
        }

        public int GetTemperatureByIndex(int gpuIndex) {
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return 0;
                }
                ADLTemperature adlt = new ADLTemperature();
                var r = AdlNativeMethods.ADL_Overdrive5_Temperature_Get(adapterIndex, 0, ref adlt);
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL_Overdrive5_Temperature_Get {r}");
                }
                if (r == AdlStatus.OK) {
                    return (int)(0.001f * adlt.Temperature);
                }
                else {
                    return 0;
                }
            }
            catch {
                return 0;
            }
        }

        public uint GetFanSpeedByIndex(int gpuIndex) {
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return 0;
                }
                ADLFanSpeedValue adlf = new ADLFanSpeedValue {
                    SpeedType = AdlConst.ADL_DL_FANCTRL_SPEED_TYPE_PERCENT
                };
                var r = AdlNativeMethods.ADL_Overdrive5_FanSpeed_Get(adapterIndex, 0, ref adlf);
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL_Overdrive5_FanSpeed_Get {r}");
                }
                if (r == AdlStatus.OK) {
                    return (uint)adlf.FanSpeed;
                }
                else {
                    return 0;
                }
            }
            catch {
                return 0;
            }
        }

        public void SetFunSpeedByIndex(int gpuIndex, int value) {
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return;
                }
                ADLFanSpeedValue adlf = new ADLFanSpeedValue {
                    SpeedType = AdlConst.ADL_DL_FANCTRL_SPEED_TYPE_PERCENT
                };
                var r = AdlNativeMethods.ADL_Overdrive5_FanSpeed_Get(adapterIndex, 0, ref adlf);
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL_Overdrive5_FanSpeed_Get {r}");
                }
                if (r == AdlStatus.OK) {
                    adlf.FanSpeed = value;
                    r = AdlNativeMethods.ADL_Overdrive5_FanSpeed_Set(adapterIndex, 0, ref adlf);
                    if (r != AdlStatus.OK) {
                        Write.DevWarn($"ADL_Overdrive5_FanSpeed_Set {r}");
                    }
                }
            }
            catch(Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        public int GetPowerLimitByIndex(int gpuIndex) {
            if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                return 0;
            }
            ADLODNPowerLimitSetting lpODPowerLimit = new ADLODNPowerLimitSetting();
            try {
                var r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get(context, adapterIndex, ref lpODPowerLimit);
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL2_OverdriveN_PowerLimit_Get {r}");
                }
                if (r == AdlStatus.OK) {
                    return 100 + lpODPowerLimit.iTDPLimit;
                }
                return 0;
            }
            catch {
                return 0;
            }
        }

        public void SetPowerLimitByIndex(int gpuIndex, int value) {
            if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                return;
            }
            ADLODNPowerLimitSetting lpODPowerLimit = new ADLODNPowerLimitSetting();
            try {
                var r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get(context, adapterIndex, ref lpODPowerLimit);
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL2_OverdriveN_PowerLimit_Get {r}");
                }
#if DEBUG
                Write.DevWarn($"ADL2_OverdriveN_PowerLimit_Get result={r},iMode={lpODPowerLimit.iMode},iTDPLimit={lpODPowerLimit.iTDPLimit},iMaxOperatingTemperature={lpODPowerLimit.iMaxOperatingTemperature}");
#endif
                if (r == AdlStatus.OK) {
                    lpODPowerLimit.iMode = AdlConst.ODNControlType_Manual;
                    lpODPowerLimit.iTDPLimit = value - 100;
                    r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Set(context, adapterIndex, ref lpODPowerLimit);
                    if (r != AdlStatus.OK) {
                        Write.DevWarn($"ADL2_OverdriveN_PowerLimit_Set {r}");
                    }
                }
            }
            catch(Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        public int GetTempLimitByIndex(int gpuIndex) {
            if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                return 0;
            }
            ADLODNPowerLimitSetting lpODPowerLimit = new ADLODNPowerLimitSetting();
            try {
                var r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get(context, adapterIndex, ref lpODPowerLimit);
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL2_OverdriveN_PowerLimit_Get {r}");
                }
                if (r == AdlStatus.OK) {
                    return lpODPowerLimit.iMaxOperatingTemperature;
                }
                return 0;
            }
            catch {
                return 0;
            }
        }

        public void SetTempLimitByIndex(int gpuIndex, int value) {
            if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                return;
            }
            ADLODNPowerLimitSetting lpODPowerLimit = new ADLODNPowerLimitSetting();
            try {
                var r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get(context, adapterIndex, ref lpODPowerLimit);
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL2_OverdriveN_PowerLimit_Get {r}");
                }
                if (r == AdlStatus.OK) {
                    if (value == 0) {
                        ADLODNCapabilitiesX2 lpODCapabilities = new ADLODNCapabilitiesX2();
                        r = AdlNativeMethods.ADL2_OverdriveN_CapabilitiesX2_Get(context, adapterIndex, ref lpODCapabilities);
                        if (r != AdlStatus.OK) {
                            Write.DevWarn($"ADL2_OverdriveN_CapabilitiesX2_Get {r}");
                        }
                        value = lpODCapabilities.powerTuneTemperature.iDefault;
                    }
                    lpODPowerLimit.iMode = AdlConst.ODNControlType_Manual;
                    lpODPowerLimit.iMaxOperatingTemperature = value;
                    r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Set(context, adapterIndex, ref lpODPowerLimit);
                    if (r != AdlStatus.OK) {
                        Write.DevWarn($"ADL2_OverdriveN_PowerLimit_Set {r}");
                    }
                }
            }
            catch(Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        public uint GetPowerUsageByIndex(int gpuIndex) {
            if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                return 0;
            }
            int power = 0;
            try {
                var r = AdlNativeMethods.ADL2_Overdrive6_CurrentPower_Get(context, adapterIndex, 0, ref power);
                if (r != AdlStatus.OK) {
                    Write.DevWarn($"ADL2_Overdrive6_CurrentPower_Get {r}");
                }
                if (r == AdlStatus.OK) {
                    return (uint)(power / 256.0);
                }
            }
            catch {
            }
            return 0;
        }
    }
}
