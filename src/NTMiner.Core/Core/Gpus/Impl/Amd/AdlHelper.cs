using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace NTMiner.Core.Gpus.Impl.Amd {
    public class AdlHelper {
        public struct ATIGPU {
            public int AdapterIndex { get; set; }
            public int BusNumber { get; set; }
            public int DeviceNumber { get; set; }
            public string AdapterName { get; set; }
        }

        private System.IntPtr context;
        private List<ATIGPU> _gpuNames = new List<ATIGPU>();
        public bool Init() {
            try {
                if (System.Environment.Is64BitOperatingSystem) {
                    Windows.NativeMethods.SetDllDirectory(SpecialPath.ThisSystem32Dir);
                }
                else {
                    Windows.NativeMethods.SetDllDirectory(SpecialPath.ThisSysWOW64Dir);
                }
                int status = ADL.ADL_Main_Control_Create(1);
                Write.DevDebug("AMD Display Library");
                Write.DevDebug("Status: " + (status == ADL.ADL_OK ? "OK" : status.ToString(CultureInfo.InvariantCulture)));
                if (status == ADL.ADL_OK) {
                    int numberOfAdapters = 0;
                    ADL.ADL_Adapter_NumberOfAdapters_Get(ref numberOfAdapters);
                    if (numberOfAdapters > 0) {
                        ADLAdapterInfo[] adapterInfo = new ADLAdapterInfo[numberOfAdapters];
                        if (ADL.ADL_Adapter_AdapterInfo_Get(adapterInfo) == ADL.ADL_OK) {
                            for (int i = 0; i < numberOfAdapters; i++) {
                                int isActive;
                                ADL.ADL_Adapter_Active_Get(adapterInfo[i].AdapterIndex, out isActive);
                                if (!string.IsNullOrEmpty(adapterInfo[i].UDID) && adapterInfo[i].VendorID == ADL.ATI_VENDOR_ID) {
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
                    ADL.ADL2_Main_Control_Create(ADL.Main_Memory_Alloc, 1, ref context);
                }
                Write.DevDebug(string.Join(",", _gpuNames.Select(a => a.AdapterIndex)));
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
                int result = ADL.ADL2_Graphics_VersionsX2_Get(context, ref lpVersionInfo);
#if DEBUG
                Write.DevDebug($"result={result},strDriverVer={lpVersionInfo.strDriverVer},strCatalystVersion={lpVersionInfo.strCatalystVersion},strCrimsonVersion={lpVersionInfo.strCrimsonVersion},strCatalystWebLink={lpVersionInfo.strCatalystWebLink}");
#endif
                return lpVersionInfo.strCrimsonVersion;
            }
            catch {
                return "0.0";
            }
        }

        // 将GPUIndex转换为AdapterIndex
        private static int GpuIndexToAdapterIndex(List<ATIGPU> gpuNames, int gpuIndex) {
            if (gpuIndex >= gpuNames.Count) {
                return 0;
            }
            return gpuNames[gpuIndex].AdapterIndex;
        }

        public string GetGpuName(int gpuIndex) {
            try {
                if (gpuIndex >= _gpuNames.Count) {
                    return string.Empty;
                }
                return _gpuNames[gpuIndex].AdapterName;
            }
            catch {
                return string.Empty;
            }
        }

        public ulong GetTotalMemoryByIndex(int gpuIndex) {
            try {
                int adapterIndex = GpuIndexToAdapterIndex(_gpuNames, gpuIndex);
                ADLMemoryInfo adlt = new ADLMemoryInfo();
                if (ADL.ADL_Adapter_MemoryInfo_Get(adapterIndex, ref adlt) == ADL.ADL_OK) {
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
                int adapterIndex = GpuIndexToAdapterIndex(_gpuNames, gpuIndex);
                ADLODNPerformanceLevelsX2 lpODPerformanceLevels = new ADLODNPerformanceLevelsX2();
                ADLODNPerformanceLevelX2[] aLevels = new ADLODNPerformanceLevelX2[ADL.ADL_PERFORMANCE_LEVELS];
                for (int i = 0; i < aLevels.Length; i++) {
                    aLevels[i] = new ADLODNPerformanceLevelX2();
                }
                lpODPerformanceLevels.aLevels = aLevels;
                lpODPerformanceLevels.iSize = Marshal.SizeOf(typeof(ADLODNPerformanceLevelsX2));
                lpODPerformanceLevels.iNumberOfPerformanceLevels = ADL.ADL_PERFORMANCE_LEVELS;
                var result = ADL.ADL2_OverdriveN_MemoryClocksX2_Get(context, adapterIndex, ref lpODPerformanceLevels);
                return lpODPerformanceLevels.aLevels[1].iClock * 10;
            }
            catch {
                return 0;
            }
        }

        public void SetMemoryClockByIndex(int gpuIndex, int value) {
            try {
                int adapterIndex = GpuIndexToAdapterIndex(_gpuNames, gpuIndex);
                ADLODNPerformanceLevelsX2 lpODPerformanceLevels = new ADLODNPerformanceLevelsX2();
                ADLODNPerformanceLevelX2[] aLevels = new ADLODNPerformanceLevelX2[ADL.ADL_PERFORMANCE_LEVELS];
                for (int i = 0; i < aLevels.Length; i++) {
                    aLevels[i] = new ADLODNPerformanceLevelX2();
                }
                lpODPerformanceLevels.aLevels = aLevels;
                lpODPerformanceLevels.iSize = Marshal.SizeOf(typeof(ADLODNPerformanceLevelsX2));
                lpODPerformanceLevels.iNumberOfPerformanceLevels = ADL.ADL_PERFORMANCE_LEVELS;
                var result = ADL.ADL2_OverdriveN_MemoryClocksX2_Get(context, adapterIndex, ref lpODPerformanceLevels);
#if DEBUG
                Write.DevDebug("ADL2_OverdriveN_MemoryClocksX2_Get result=" + result);
                foreach (var item in lpODPerformanceLevels.aLevels) {
                    Write.DevDebug($"iClock={item.iClock},iControl={item.iControl},iEnabled={item.iEnabled},iVddc={item.iVddc}");
                }
#endif
                lpODPerformanceLevels.iMode = ADL.ODNControlType_Manual;
                for (int i = 0; i < 2; i++) {
                    var level = lpODPerformanceLevels.aLevels[i];
                    level.iEnabled = 1;
                }
                lpODPerformanceLevels.aLevels[1].iClock = value * 100;
                result = ADL.ADL2_OverdriveN_MemoryClocksX2_Set(context, adapterIndex, ref lpODPerformanceLevels);
                Write.DevDebug($"ADL2_OverdriveN_MemoryClocksX2_Set({value * 100}) result " + result);
            }
            catch {
            }
        }

        public int GetSystemClockByIndex(int gpuIndex) {
            try {
                int adapterIndex = GpuIndexToAdapterIndex(_gpuNames, gpuIndex);
                ADLODNPerformanceLevelsX2 lpODPerformanceLevels = new ADLODNPerformanceLevelsX2();
                ADLODNPerformanceLevelX2[] aLevels = new ADLODNPerformanceLevelX2[ADL.ADL_PERFORMANCE_LEVELS];
                for (int i = 0; i < aLevels.Length; i++) {
                    aLevels[i] = new ADLODNPerformanceLevelX2();
                }
                lpODPerformanceLevels.aLevels = aLevels;
                lpODPerformanceLevels.iSize = Marshal.SizeOf(typeof(ADLODNPerformanceLevelsX2));
                lpODPerformanceLevels.iNumberOfPerformanceLevels = ADL.ADL_PERFORMANCE_LEVELS;
                var result = ADL.ADL2_OverdriveN_SystemClocksX2_Get(context, adapterIndex, ref lpODPerformanceLevels);
#if DEBUG
                Write.DevDebug("ADL2_OverdriveN_SystemClocksX2_Get result=" + result);
                foreach (var item in lpODPerformanceLevels.aLevels) {
                    Write.DevDebug($"iClock={item.iClock},iControl={item.iControl},iEnabled={item.iEnabled},iVddc={item.iVddc}");
                }
#endif
                return lpODPerformanceLevels.aLevels[aLevels.Length - 1].iClock * 10;
            }
            catch {
                return 0;
            }
        }

        public int GetTemperatureByIndex(int gpuIndex) {
            try {
                int adapterIndex = GpuIndexToAdapterIndex(_gpuNames, gpuIndex);
                ADLTemperature adlt = new ADLTemperature();
                if (ADL.ADL_Overdrive5_Temperature_Get(adapterIndex, 0, ref adlt) == ADL.ADL_OK) {
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
                int adapterIndex = GpuIndexToAdapterIndex(_gpuNames, gpuIndex);
                ADLFanSpeedValue adlf = new ADLFanSpeedValue();
                adlf.SpeedType = ADL.ADL_DL_FANCTRL_SPEED_TYPE_PERCENT;
                if (ADL.ADL_Overdrive5_FanSpeed_Get(adapterIndex, 0, ref adlf) == ADL.ADL_OK) {
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
                int adapterIndex = GpuIndexToAdapterIndex(_gpuNames, gpuIndex);
                ADLFanSpeedValue adlf = new ADLFanSpeedValue();
                adlf.SpeedType = ADL.ADL_DL_FANCTRL_SPEED_TYPE_PERCENT;
                if (ADL.ADL_Overdrive5_FanSpeed_Get(adapterIndex, 0, ref adlf) == ADL.ADL_OK) {
                    adlf.FanSpeed = value;
                    ADL.ADL_Overdrive5_FanSpeed_Set(adapterIndex, 0, ref adlf);
                }
            }
            catch {
            }
        }

        public int GetPowerLimitByIndex(int gpuIndex) {
            int adapterIndex = GpuIndexToAdapterIndex(_gpuNames, gpuIndex);
            ADLODNPowerLimitSetting lpODPowerLimit = new ADLODNPowerLimitSetting();
            try {
                int result = ADL.ADL2_OverdriveN_PowerLimit_Get(context, adapterIndex, ref lpODPowerLimit);
#if DEBUG
                Write.DevDebug($"result={result},iMode={lpODPowerLimit.iMode},iTDPLimit={lpODPowerLimit.iTDPLimit},iMaxOperatingTemperature={lpODPowerLimit.iMaxOperatingTemperature}");
#endif
                if (result == ADL.ADL_OK) {
                    return 100 + lpODPowerLimit.iTDPLimit;
                }
                return 0;
            }
            catch {
                return 0;
            }
        }

        public void SetPowerLimitByIndex(int gpuIndex, int value) {
            int adapterIndex = GpuIndexToAdapterIndex(_gpuNames, gpuIndex);
            ADLODNPowerLimitSetting lpODPowerLimit = new ADLODNPowerLimitSetting();
            try {
                int result = ADL.ADL2_OverdriveN_PowerLimit_Get(context, adapterIndex, ref lpODPowerLimit);
                if (result == ADL.ADL_OK) {
                    lpODPowerLimit.iMode = ADL.ODNControlType_Manual;
                    lpODPowerLimit.iTDPLimit = value - 100;
                    ADL.ADL2_OverdriveN_PowerLimit_Set(context, adapterIndex, ref lpODPowerLimit);
                }
            }
            catch {
            }
        }

        public int GetTempLimitByIndex(int gpuIndex) {
            int adapterIndex = GpuIndexToAdapterIndex(_gpuNames, gpuIndex);
            ADLODNPowerLimitSetting lpODPowerLimit = new ADLODNPowerLimitSetting();
            try {
                int result = ADL.ADL2_OverdriveN_PowerLimit_Get(context, adapterIndex, ref lpODPowerLimit);
                if (result == ADL.ADL_OK) {
                    return lpODPowerLimit.iMaxOperatingTemperature;
                }
                return 0;
            }
            catch {
                return 0;
            }
        }

        public void SetTempLimitByIndex(int gpuIndex, int value) {
            int adapterIndex = GpuIndexToAdapterIndex(_gpuNames, gpuIndex);
            ADLODNPowerLimitSetting lpODPowerLimit = new ADLODNPowerLimitSetting();
            try {
                int result = ADL.ADL2_OverdriveN_PowerLimit_Get(context, adapterIndex, ref lpODPowerLimit);
                if (result == ADL.ADL_OK) {
                    lpODPowerLimit.iMode = ADL.ODNControlType_Manual;
                    lpODPowerLimit.iMaxOperatingTemperature = value;
                    ADL.ADL2_OverdriveN_PowerLimit_Set(context, adapterIndex, ref lpODPowerLimit);
                }
            }
            catch {
            }
        }

        public uint GetPowerUsageByIndex(int gpuIndex) {
            int adapterIndex = GpuIndexToAdapterIndex(_gpuNames, gpuIndex);
            int power = 0;
            try {
                if (ADL.ADL2_Overdrive6_CurrentPower_Get(context, adapterIndex, 0, ref power) == ADL.ADL_OK) {
                    return (uint)(power / 256.0);
                }
            }
            catch {
            }
            return 0;
        }
    }
}
