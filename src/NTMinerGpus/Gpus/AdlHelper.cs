using NTMiner.Gpus.Adl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace NTMiner.Gpus {
    public class AdlHelper : IGpuHelper {
        public AdlHelper() {
            Init();
        }

        private IntPtr _context = IntPtr.Zero;
        private List<ATIGPU> _gpuNames = new List<ATIGPU>();
        private bool Init() {
            try {
                var adlStatus = AdlNativeMethods.ADLMainControlCreate(out _context);
                if (adlStatus >= AdlStatus.ADL_OK) {
                    int numberOfAdapters = 0;
                    adlStatus = AdlNativeMethods.ADL_Adapter_NumberOfAdapters_Get(ref numberOfAdapters);
                    if (adlStatus < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL_Adapter_NumberOfAdapters_Get)} {adlStatus.ToString()}");
                    }
                    if (numberOfAdapters > 0) {
                        ADLAdapterInfo[] adapterInfoes = new ADLAdapterInfo[numberOfAdapters];
                        if (AdlNativeMethods.ADLAdapterAdapterInfoGet(adapterInfoes) >= AdlStatus.ADL_OK) {
                            for (int i = 0; i < numberOfAdapters; i++) {
                                var adapterInfo = adapterInfoes[i];
                                NTMinerConsole.DevDebug(() => adapterInfo.ToString());
                                if (!string.IsNullOrEmpty(adapterInfo.UDID) && adapterInfo.VendorID == AdlConst.ATI_VENDOR_ID) {
                                    bool found = false;
                                    foreach (ATIGPU gpu in _gpuNames) {
                                        if (gpu.BusNumber == adapterInfo.BusNumber && gpu.DeviceNumber == adapterInfo.DeviceNumber) {
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found) {
                                        int adapterIndex = adapterInfo.AdapterIndex;
                                        int overdriveVersion = 0;
                                        try {
                                            if (AdlNativeMethods.ADL_Overdrive_Caps(adapterIndex, out _, out _, out overdriveVersion) != AdlStatus.ADL_OK) {
                                                overdriveVersion = -1;
                                            }
                                        }
                                        catch (Exception ex) {
                                            Logger.ErrorDebugLine(ex);
                                        }
                                        ADLODNCapabilitiesX2 info = new ADLODNCapabilitiesX2();
                                        try {
                                            var r = AdlNativeMethods.ADL2_OverdriveN_CapabilitiesX2_Get(_context, adapterIndex, ref info);
                                            if (r < AdlStatus.ADL_OK) {
                                                NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_CapabilitiesX2_Get)} {r.ToString()}");
                                            }
                                        }
                                        catch (Exception ex) {
                                            Logger.ErrorDebugLine(ex);
                                        }
                                        int maxLevels = info.iMaximumNumberOfPerformanceLevels;
                                        int fanSpeedMin = 0;
                                        int fanSpeedMax = 0;
                                        ADLFanSpeedInfo afsi = new ADLFanSpeedInfo();
                                        try {
                                            var r = AdlNativeMethods.ADL_Overdrive5_FanSpeedInfo_Get(adapterIndex, 0, ref afsi);
                                            if (r < AdlStatus.ADL_OK) {
                                                NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL_Overdrive5_FanSpeedInfo_Get)} {r.ToString()}");
                                            }
                                            else {
                                                fanSpeedMax = afsi.MaxPercent;
                                                fanSpeedMin = afsi.MinPercent;
                                            }
                                        }
                                        catch (Exception ex) {
                                            Logger.ErrorDebugLine(ex);
                                        }
                                        ADLODNPerformanceLevelsX2 systemClockX2 = ADLODNPerformanceLevelsX2.Create();
                                        systemClockX2.iNumberOfPerformanceLevels = maxLevels;
                                        try {
                                            var r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get(_context, adapterIndex, ref systemClockX2);
                                            if (r < AdlStatus.ADL_OK) {
                                                NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get)} {r.ToString()}");
                                            }
                                        }
                                        catch (Exception ex) {
                                            Logger.ErrorDebugLine(ex);
                                        }
                                        int gpuLevel = 0;
                                        int memoryLevel = 0;
                                        for (int j = 0; j < systemClockX2.aLevels.Length; j++) {
                                            if (systemClockX2.aLevels[j].iEnabled != 0) {
                                                gpuLevel = j + 1;
                                            }
                                        }
                                        ADLODNPerformanceLevelsX2 memoryClockX2 = ADLODNPerformanceLevelsX2.Create();
                                        memoryClockX2.iNumberOfPerformanceLevels = maxLevels;
                                        try {
                                            var r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get(_context, adapterIndex, ref memoryClockX2);
                                            if (r < AdlStatus.ADL_OK) {
                                                NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get)} {r.ToString()}");
                                            }
                                        }
                                        catch (Exception ex) {
                                            Logger.ErrorDebugLine(ex);
                                        }
                                        for (int j = 0; j < memoryClockX2.aLevels.Length; j++) {
                                            if (memoryClockX2.aLevels[j].iEnabled != 0) {
                                                memoryLevel = j + 1;
                                            }
                                        }
                                        int powerMin = info.power.iMin + 100;
                                        int powerMax = info.power.iMax + 100;
                                        int powerDefault = info.power.iDefault + 100;
                                        int voltMin = info.svddcRange.iMin;// 0
                                        int voltMax = info.svddcRange.iMax;// 0
                                        int voltDefault = info.svddcRange.iDefault; // 0
                                        int tempLimitMin = info.powerTuneTemperature.iMin;
                                        int tempLimitMax = info.powerTuneTemperature.iMax;
                                        int tempLimitDefault = info.powerTuneTemperature.iDefault;
                                        int coreClockMin = info.sEngineClockRange.iMin * 10;
                                        int coreClockMax = info.sEngineClockRange.iMax * 10;
                                        int memoryClockMin = info.sMemoryClockRange.iMin * 10;
                                        int memoryClockMax = info.sMemoryClockRange.iMax * 10;
                                        bool apiSupported = gpuLevel > 0 && memoryLevel > 0;
                                        ADLOD8InitSetting odInitSetting = ADLOD8InitSetting.Create();
                                        if (!apiSupported) {
                                            try {
                                                var r = AdlNativeMethods.ADL2_Overdrive_Caps(_context, adapterIndex, out int iSupported, out int iEnabled, out int iVersion);
                                                if (r < AdlStatus.ADL_OK) {
                                                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_Overdrive_Caps)} {r.ToString()}");
                                                }
                                                else {
                                                    if (iVersion == 8) {
                                                        if (GetOD8InitSetting(adapterIndex, out odInitSetting)) {
                                                            apiSupported = true;
                                                            gpuLevel = 3;
                                                            memoryLevel = 0;
                                                            maxLevels = 3;

                                                            powerMin = 0;
                                                            powerMax = 0;
                                                            powerDefault = 0;
                                                            voltMin = 0;
                                                            voltMax = 0;
                                                            voltDefault = 0;
                                                            tempLimitMin = 0;
                                                            tempLimitMax = 0;
                                                            tempLimitDefault = 0;
                                                            coreClockMin = 0;
                                                            coreClockMax = 0;
                                                            memoryClockMin = 0;
                                                            memoryClockMax = 0;
                                                            if ((odInitSetting.overdrive8Capabilities & (int)ADLOD8FeatureControl.ADL_OD8_GFXCLK_LIMITS) == (int)ADLOD8FeatureControl.ADL_OD8_GFXCLK_LIMITS ||
                                                                (odInitSetting.overdrive8Capabilities & (int)ADLOD8FeatureControl.ADL_OD8_GFXCLK_CURVE) == (int)ADLOD8FeatureControl.ADL_OD8_GFXCLK_CURVE) {
                                                                coreClockMin = odInitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_FMIN].minValue;
                                                                coreClockMax = odInitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_FMAX].maxValue;
                                                                memoryClockMax = odInitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_UCLK_FMAX].maxValue;
                                                            }
#if DEBUG
                                                            Logger.Debug(odInitSetting.ToString());
#endif
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception ex) {
                                                Logger.ErrorDebugLine(ex);
                                            }
                                        }
                                        if (fanSpeedMax <= 0) {
                                            fanSpeedMax = 100;
                                        }
                                        if (powerMax <= 0) {
                                            powerMax = 100;
                                        }
                                        _gpuNames.Add(new ATIGPU {
                                            AdapterName = adapterInfo.AdapterName.Trim(),
                                            AdapterIndex = adapterIndex,
                                            BusNumber = adapterInfo.BusNumber,
                                            DeviceNumber = adapterInfo.DeviceNumber,
                                            OverdriveVersion = overdriveVersion,
                                            MaxLevels = maxLevels,
                                            ApiSupported = apiSupported,
                                            GpuLevels = gpuLevel,
                                            MemoryLevels = memoryLevel,
                                            CoreClockMin = coreClockMin,
                                            CoreClockMax = coreClockMax,
                                            MemoryClockMin = memoryClockMin,
                                            MemoryClockMax = memoryClockMax,
                                            PowerMin = powerMin,
                                            PowerMax = powerMax,
                                            PowerDefault = powerDefault,
                                            TempLimitMin = tempLimitMin,
                                            TempLimitMax = tempLimitMax,
                                            TempLimitDefault = tempLimitDefault,
                                            VoltMin = voltMin,
                                            VoltMax = voltMax,
                                            VoltDefault = voltDefault,
                                            FanSpeedMax = fanSpeedMax,
                                            FanSpeedMin = fanSpeedMin,
                                            ADLOD8InitSetting = odInitSetting
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                _gpuNames = _gpuNames.OrderBy(a => a.BusNumber).ToList();
                NTMinerConsole.DevDebug(() => string.Join(",", _gpuNames.Select(a => a.AdapterIndex)));
            }
            catch (Exception ex) {
                Logger.ErrorDebugLine(ex);
                return false;
            }

            return true;
        }

        private bool GetOD8CurrentSetting(int adapterIndex, out Odn8Settings odn8Settings) {
            odn8Settings = Odn8Settings.Create();
            try {
                int lpNumberOfFeatures = (int)ADLOD8SettingId.OD8_COUNT;
                var r = AdlNativeMethods.ADL2_Overdrive8_Current_SettingX2_Get(_context, adapterIndex, ref lpNumberOfFeatures, out IntPtr lppCurrentSettingList);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_Overdrive8_Current_SettingX2_Get)} {r.ToString()}");
                }
                if (lppCurrentSettingList != IntPtr.Zero) {
                    odn8Settings = (Odn8Settings)Marshal.PtrToStructure(lppCurrentSettingList, typeof(Odn8Settings));
                    Marshal.FreeHGlobal(lppCurrentSettingList);
                }
                return lppCurrentSettingList != IntPtr.Zero;
            }
            catch (Exception ex) {
                Logger.ErrorDebugLine(ex);
                return false;
            }
        }

        private bool GetOD8CurrentSetting(int adapterIndex, out ADLOD8CurrentSetting odCurrentSetting) {
            odCurrentSetting = ADLOD8CurrentSetting.Create();
            try {
                int lpNumberOfFeatures = (int)ADLOD8SettingId.OD8_COUNT;
                var r = AdlNativeMethods.ADL2_Overdrive8_Current_SettingX2_Get(_context, adapterIndex, ref lpNumberOfFeatures, out IntPtr lppCurrentSettingList);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_Overdrive8_Current_SettingX2_Get)} {r.ToString()}");
                }
                if (r == AdlStatus.ADL_OK) {
                    odCurrentSetting.count = lpNumberOfFeatures > (int)ADLOD8SettingId.OD8_COUNT ? (int)ADLOD8SettingId.OD8_COUNT : lpNumberOfFeatures;
                    int[] settingList = new int[odCurrentSetting.count];
                    int elementSize = Marshal.SizeOf(typeof(int));
                    for (int i = 0; i < settingList.Length; i++) {
                        settingList[i] = (int)Marshal.PtrToStructure((IntPtr)((long)lppCurrentSettingList + i * elementSize), typeof(int));
                    }
                    for (int i = 0; i < odCurrentSetting.count; i++) {
                        odCurrentSetting.Od8SettingTable[i] = settingList[i];
                    }
                }
                return r == AdlStatus.ADL_OK;
            }
            catch (Exception ex) {
                Logger.ErrorDebugLine(ex);
                return false;
            }
        }

        private bool GetOD8InitSetting(int adapterIndex, out ADLOD8InitSetting odInitSetting) {
            odInitSetting = ADLOD8InitSetting.Create();
            try {
                int elementSize = Marshal.SizeOf(typeof(ADLOD8SingleInitSetting));
                int overdrive8Capabilities;
                int numberOfFeatures = (int)ADLOD8SettingId.OD8_COUNT;
                var r = AdlNativeMethods.ADL2_Overdrive8_Init_SettingX2_Get(_context, adapterIndex, out overdrive8Capabilities, ref numberOfFeatures, out IntPtr lpInitSettingList);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_Overdrive8_Init_SettingX2_Get)} {r.ToString()}");
                }
                ADLOD8SingleInitSetting[] od8initSettingList = new ADLOD8SingleInitSetting[numberOfFeatures];
                if (lpInitSettingList != IntPtr.Zero) {
                    for (int i = 0; i < od8initSettingList.Length; i++) {
                        od8initSettingList[i] = (ADLOD8SingleInitSetting)Marshal.PtrToStructure((IntPtr)((long)lpInitSettingList + i * elementSize), typeof(ADLOD8SingleInitSetting));
                    }
                    Marshal.FreeHGlobal(lpInitSettingList);
                }
                if (r == AdlStatus.ADL_OK) {
                    odInitSetting.count = numberOfFeatures > (int)ADLOD8SettingId.OD8_COUNT ? (int)ADLOD8SettingId.OD8_COUNT : numberOfFeatures;
                    odInitSetting.overdrive8Capabilities = overdrive8Capabilities;
                    for (int i = 0; i < odInitSetting.count; i++) {
                        odInitSetting.od8SettingTable[i].defaultValue = od8initSettingList[i].defaultValue;
                        odInitSetting.od8SettingTable[i].featureID = od8initSettingList[i].featureID;
                        odInitSetting.od8SettingTable[i].maxValue = od8initSettingList[i].maxValue;
                        odInitSetting.od8SettingTable[i].minValue = od8initSettingList[i].minValue;
                    }
                }
#if DEBUG
                Logger.Debug($"od8initSettingList={VirtualRoot.JsonSerializer.Serialize(odInitSetting)}");
#endif
                return r == AdlStatus.ADL_OK;
            }
            catch (Exception ex) {
                Logger.ErrorDebugLine(ex);
                return false;
            }
        }

        public void Close() {
            try {
                AdlNativeMethods.ADL_Main_Control_Destroy?.Invoke();
            }
            catch {
            }
            try {
                if (_context != IntPtr.Zero) {
                    AdlNativeMethods.ADL2_Main_Control_Destroy?.Invoke(_context);
                    _context = IntPtr.Zero;
                }
            }
            catch {
            }
        }

        public int GpuCount {
            get { return _gpuNames.Count; }
        }

        public Version GetDriverVersion() {
            try {
                ADLVersionsInfoX2 info = new ADLVersionsInfoX2();
                var r = AdlNativeMethods.ADL2_Graphics_VersionsX2_Get(_context, ref info);
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

        public bool TryGetAtiGpu(int gpuIndex, out ATIGPU gpu) {
            gpu = ATIGPU.Empty;
            if (gpuIndex < 0 || gpuIndex >= _gpuNames.Count) {
                return false;
            }
            gpu = _gpuNames[gpuIndex];
            return true;
        }

        public ATIGPU GetAtiGPU(int gpuIndex) {
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
                if (!TryGetAtiGpu(gpuIndex, out ATIGPU gpuInfo)) {
                    return result;
                }
                GetTempLimitAndPowerLimit(gpuIndex, out int powerLimit, out int tempLimit);
                GetClockAndVolt(gpuIndex, out int memoryClock, out int memoryiVddc, out int coreClock, out int coreiVddc);
                result.PowerCurr = powerLimit;
                result.TempCurr = tempLimit;
                result.MemoryClockDelta = memoryClock;
                result.MemoryVoltage = memoryiVddc;
                result.CoreClockDelta = coreClock;
                result.CoreVoltage = coreiVddc;
                result.CoreClockMin = gpuInfo.CoreClockMin;
                result.CoreClockMax = gpuInfo.CoreClockMax;
                result.MemoryClockMin = gpuInfo.MemoryClockMin;
                result.MemoryClockMax = gpuInfo.MemoryClockMax;
                result.PowerMin = gpuInfo.PowerMin;
                result.PowerMax = gpuInfo.PowerMax;
                result.PowerDefault = gpuInfo.PowerDefault;
                result.TempLimitMin = gpuInfo.TempLimitMin;
                result.TempLimitMax = gpuInfo.TempLimitMax;
                result.TempLimitDefault = gpuInfo.TempLimitDefault;
                result.VoltMin = gpuInfo.VoltMin;
                result.VoltMax = gpuInfo.VoltMax;
                result.VoltDefault = gpuInfo.VoltDefault;
                result.FanSpeedMin = gpuInfo.FanSpeedMin;
                result.FanSpeedMax = gpuInfo.FanSpeedMax;
#if DEBUG
                NTMinerConsole.DevDebug(() => $"GetClockRange {result.ToString()}");
#endif
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            return result;
        }

        public void GetClockAndVolt(int gpuIndex, out int memoryClock, out int memoryiVddc, out int coreClock, out int coreiVddc) {
            memoryClock = 0;
            memoryiVddc = 0;
            coreClock = 0;
            coreiVddc = 0;
            try {
                if (!TryGetAtiGpu(gpuIndex, out ATIGPU gpu)) {
                    return;
                }
                if (gpu.OverdriveVersion < 8) {
                    try {
                        ADLODNPerformanceLevelsX2 info = ADLODNPerformanceLevelsX2.Create();
                        var r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get(_context, gpu.AdapterIndex, ref info);
                        if (r < AdlStatus.ADL_OK) {
                            NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get)} {r.ToString()}");
                        }
                        var index = gpu.GpuLevels - 1;
                        coreClock = info.aLevels[index].iClock * 10;
                        coreiVddc = info.aLevels[index].iVddc;
                    }
                    catch {
                    }
                    try {
                        ADLODNPerformanceLevelsX2 info = ADLODNPerformanceLevelsX2.Create();
                        var r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get(_context, gpu.AdapterIndex, ref info);
                        if (r < AdlStatus.ADL_OK) {
                            NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get)} {r.ToString()}");
                        }
                        int index = gpu.MemoryLevels - 1;
                        memoryClock = info.aLevels[index].iClock * 10;
                        memoryiVddc = info.aLevels[index].iVddc;
                    }
                    catch {
                    }
                }
                else {
                    if (GetOD8CurrentSetting(gpu.AdapterIndex, out Odn8Settings odn8Settings)) {
                        coreClock = odn8Settings.GpuP[2].Clock * 1000;
                        coreiVddc = odn8Settings.GpuP[2].Voltage;
                        memoryClock = odn8Settings.MemMax * 1000;
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }


        public void GetPowerFanTemp(int gpuIndex, out uint power, out uint fanSpeed, out int temp, out int memoryiVddc) {
            power = 0;
            fanSpeed = 0;
            temp = 0;
            memoryiVddc = 0;
            if (!TryGetAtiGpu(gpuIndex, out ATIGPU gpu)) {
                return;
            }
            try {
                if (gpu.OverdriveVersion < 8) {
                    temp = GetTemperature(gpuIndex);
                    power = GetPowerUsage(gpuIndex);
                    fanSpeed = GetFanSpeed(gpuIndex);
                }
                else {
                    ADLPMLogDataOutput logDataOutput = ADLPMLogDataOutput.Create();
                    var r = AdlNativeMethods.ADL2_New_QueryPMLogData_Get(_context, gpu.AdapterIndex, ref logDataOutput);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_New_QueryPMLogData_Get)} {r.ToString()}");
                    }
                    int i = (int)ADLSensorType.PMLOG_ASIC_POWER;
                    if (i < logDataOutput.Sensors.Length && logDataOutput.Sensors[i].Supported) {
                        power = (uint)logDataOutput.Sensors[i].Value;
                    }
                    i = (int)ADLSensorType.PMLOG_FAN_PERCENTAGE;
                    if (i < logDataOutput.Sensors.Length && logDataOutput.Sensors[i].Supported) {
                        fanSpeed = (uint)logDataOutput.Sensors[i].Value;
                    }
                    i = (int)ADLSensorType.PMLOG_TEMPERATURE_EDGE;
                    if (i < logDataOutput.Sensors.Length && logDataOutput.Sensors[i].Supported) {
                        temp = logDataOutput.Sensors[i].Value;
                    }
                    i = (int)ADLSensorType.PMLOG_MEM_VOLTAGE;
                    if (i < logDataOutput.Sensors.Length && logDataOutput.Sensors[i].Supported) {
                        memoryiVddc = logDataOutput.Sensors[i].Value;
                    }
                }
            }
            catch {
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
                if (!TryGetAtiGpu(gpuIndex, out ATIGPU gpu)) {
                    return false;
                }
                bool isReset = value == 0 && voltage == 0;
                if (gpu.OverdriveVersion < 8) {
                    ADLODNPerformanceLevelsX2 info = ADLODNPerformanceLevelsX2.Create();
                    var r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get(_context, gpu.AdapterIndex, ref info);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get)} {r.ToString()}");
                        return false;
                    }
                    info.iMode = AdlConst.ODNControlType_Default;
                    r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Set(_context, gpu.AdapterIndex, ref info);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Set)} {r.ToString()}");
                        return false;
                    }
                    if (isReset) {
                        return true;
                    }
                    r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get(_context, gpu.AdapterIndex, ref info);
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
                    NTMinerConsole.DevDebug(() => $"{nameof(SetCoreClock)} PState {index.ToString()} value={value.ToString()} voltage={voltage.ToString()}");
                    if (value != 0) {
                        info.aLevels[index].iClock = value * 100;
                    }
                    if (voltage != 0) {
                        info.aLevels[index].iVddc = voltage;
                    }
                    r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Set(_context, gpu.AdapterIndex, ref info);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Set)} {r.ToString()}");
                        return false;
                    }
                }
                else {
                    if (GetOD8CurrentSetting(gpu.AdapterIndex, out ADLOD8CurrentSetting odCurrentSetting)) {
                        SetOD8Range(gpu.ADLOD8InitSetting, odCurrentSetting, gpu.AdapterIndex, ADLOD8SettingId.OD8_GFXCLK_FMAX, isReset, value * 100);
                        SetOD8Range(gpu.ADLOD8InitSetting, odCurrentSetting, gpu.AdapterIndex, ADLOD8SettingId.OD8_GFXCLK_VOLTAGE3, isReset, voltage);
                    }
                }
                return true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return false;
            }
        }

        private bool SetOD8Range(ADLOD8InitSetting odInitSetting, ADLOD8CurrentSetting odCurrentSetting, int adapterIndex, ADLOD8SettingId settingId, bool reset, int value) {
            try {
                ADLOD8SetSetting odSetSetting = ADLOD8SetSetting.Create();
                for (int i = (int)ADLOD8SettingId.OD8_GFXCLK_FREQ1; i <= (int)ADLOD8SettingId.OD8_UCLK_FMAX; i++) {
                    odSetSetting.od8SettingTable[i].requested = 1;
                    odSetSetting.od8SettingTable[i].value = odCurrentSetting.Od8SettingTable[i];
                }
                for (int i = (int)ADLOD8SettingId.OD8_FAN_CURVE_TEMPERATURE_1; i <= (int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_5; i++) {
                    odSetSetting.od8SettingTable[i].reset = settingId <= ADLOD8SettingId.OD8_FAN_CURVE_SPEED_5 && settingId >= ADLOD8SettingId.OD8_FAN_CURVE_TEMPERATURE_1 ? 0 : 1;
                    odSetSetting.od8SettingTable[i].requested = 1;
                    odSetSetting.od8SettingTable[i].value = odCurrentSetting.Od8SettingTable[i];
                }
                odSetSetting.od8SettingTable[(int)settingId].requested = 1;
                if (!reset) {
                    odSetSetting.od8SettingTable[(int)settingId].value = value;
                    if (ADLOD8SettingId.OD8_GFXCLK_FMAX == settingId) {
                        odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_FREQ3].value = value;
                    }
                    else if (ADLOD8SettingId.OD8_GFXCLK_FMIN == settingId) {
                        odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_FREQ1].value = value;
                    }
                }
                else {
                    odSetSetting.od8SettingTable[(int)settingId].reset = reset ? 1 : 0;
                    odSetSetting.od8SettingTable[(int)settingId].value = odInitSetting.od8SettingTable[(int)settingId].defaultValue;
                }
                var r = AdlNativeMethods.ADL2_Overdrive8_Setting_Set(_context, adapterIndex, ref odSetSetting, out odCurrentSetting);
                if (r != AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_Overdrive8_Setting_Set)} {r.ToString()}");
                }
                return r == AdlStatus.ADL_OK;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
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
                if (!TryGetAtiGpu(gpuIndex, out ATIGPU gpu)) {
                    return false;
                }
                bool isReset = value == 0 && voltage == 0;
                if (gpu.OverdriveVersion < 8) {
                    ADLODNPerformanceLevelsX2 info = ADLODNPerformanceLevelsX2.Create();
                    var r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get(_context, gpu.AdapterIndex, ref info);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get)} {r.ToString()}");
                        return false;
                    }
                    info.iMode = AdlConst.ODNControlType_Default;
                    r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Set(_context, gpu.AdapterIndex, ref info);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Set)} {r.ToString()}");
                        return false;
                    }
                    if (isReset) {
                        return true;
                    }
                    r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get(_context, gpu.AdapterIndex, ref info);
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
                    NTMinerConsole.DevDebug(() => $"{nameof(SetMemoryClock)} PState {index.ToString()} value={value.ToString()} voltage={voltage.ToString()}");
                    if (value != 0) {
                        info.aLevels[index].iClock = value * 100;
                    }
                    if (voltage != 0) {
                        info.aLevels[index].iVddc = voltage;
                    }
                    r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Set(_context, gpu.AdapterIndex, ref info);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Set)} {r.ToString()}");
                        return false;
                    }
                }
                else {
                    if (GetOD8CurrentSetting(gpu.AdapterIndex, out ADLOD8CurrentSetting odCurrentSetting)) {
                        SetOD8Range(gpu.ADLOD8InitSetting, odCurrentSetting, gpu.AdapterIndex, ADLOD8SettingId.OD8_UCLK_FMAX, isReset, value * 100);
                    }
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

        public bool SetFanSpeed(int gpuIndex, int value, bool isAutoMode) {
            try {
                if (!TryGetAtiGpu(gpuIndex, out ATIGPU gpu)) {
                    return false;
                }
                if (gpu.OverdriveVersion < 8) {
                    AdlStatus r;
                    if (isAutoMode) {
                        try {
                            r = AdlNativeMethods.ADL2_Overdrive5_FanSpeedToDefault_Set(_context, gpu.AdapterIndex, 0);
                            if (r != AdlStatus.ADL_OK) {
                                NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_Overdrive5_FanSpeedToDefault_Set)} {r.ToString()}");
                            }
                        }
                        catch (Exception e) {
                            r = AdlStatus.ADL_ERR;
                            Logger.ErrorDebugLine(e);
                        }
                        if (r != AdlStatus.ADL_OK) {
                            try {
                                r = AdlNativeMethods.ADL_Overdrive5_FanSpeedToDefault_Set(gpu.AdapterIndex, 0);
                                if (r != AdlStatus.ADL_OK) {
                                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL_Overdrive5_FanSpeedToDefault_Set)} {r.ToString()}");
                                }
                            }
                            catch (Exception e) {
                                Logger.ErrorDebugLine(e);
                            }
                        }
                        if (r != AdlStatus.ADL_OK) {
                            try {
                                r = AdlNativeMethods.ADL2_Overdrive6_FanSpeed_Reset(_context, gpu.AdapterIndex);
                                if (r != AdlStatus.ADL_OK) {
                                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_Overdrive6_FanSpeed_Reset)} {r.ToString()}");
                                }
                            }
                            catch (Exception e) {
                                Logger.ErrorDebugLine(e);
                            }
                        }
                        return true;
                    }
                    ADLFanSpeedValue info = new ADLFanSpeedValue {
                        SpeedType = AdlConst.ADL_DL_FANCTRL_SPEED_TYPE_PERCENT,
                        Flags = AdlConst.ADL_DL_FANCTRL_FLAG_USER_DEFINED_SPEED
                    };
                    r = AdlNativeMethods.ADL_Overdrive5_FanSpeed_Get(gpu.AdapterIndex, 0, ref info);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL_Overdrive5_FanSpeed_Get)} {r.ToString()}");
                        return false;
                    }
                    info.FanSpeed = value;
                    r = AdlNativeMethods.ADL_Overdrive5_FanSpeed_Set(gpu.AdapterIndex, 0, ref info);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL_Overdrive5_FanSpeed_Set)} {r.ToString()}");
                        return false;
                    }
                }
                else {
                    if (GetOD8CurrentSetting(gpu.AdapterIndex, out ADLOD8CurrentSetting odCurrentSetting)) {
                        SetOD8Range(gpu.ADLOD8InitSetting, odCurrentSetting, gpu.AdapterIndex, ADLOD8SettingId.OD8_FAN_MIN_SPEED, isAutoMode, value);
                        SetOD8Range(gpu.ADLOD8InitSetting, odCurrentSetting, gpu.AdapterIndex, ADLOD8SettingId.OD8_FAN_ACOUSTIC_LIMIT, isAutoMode, value);
                    }
                }
                return true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return false;
            }
        }

        public void GetTempLimitAndPowerLimit(int gpuIndex, out int powerLimit, out int tempLimit) {
            powerLimit = 0;
            tempLimit = 0;
            if (!TryGetAtiGpu(gpuIndex, out ATIGPU gpu)) {
                return;
            }
            if (gpu.OverdriveVersion < 8) {
                ADLODNPowerLimitSetting info = new ADLODNPowerLimitSetting();
                try {
                    var r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get(_context, gpu.AdapterIndex, out info);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get)} {r.ToString()}");
                    }
                    else {
                        powerLimit = 100 + info.iTDPLimit;
                        tempLimit = info.iMaxOperatingTemperature;
                    }
                }
                catch {
                }
            }
            else {
                if (GetOD8CurrentSetting(gpu.AdapterIndex, out Odn8Settings odn8Settings)) {
                    powerLimit = 100 + odn8Settings.PowerTarget;
                    // 貌似没有tempLimit
                }
                else {
                }
            }
        }

        public bool SetPowerLimit(int gpuIndex, int value) {
            if (!TryGetAtiGpu(gpuIndex, out ATIGPU gpu)) {
                return false;
            }
            try {
                if (gpu.OverdriveVersion < 8) {
                    ADLODNPowerLimitSetting info = new ADLODNPowerLimitSetting();
                    var r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get(_context, gpu.AdapterIndex, out info);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get)} {r.ToString()}");
                        return false;
                    }
                    info.iMode = AdlConst.ODNControlType_Manual;
                    info.iTDPLimit = value - 100;
                    r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Set(_context, gpu.AdapterIndex, ref info);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Set)} {r.ToString()}");
                        return false;
                    }
                }
                else {
                    if (GetOD8CurrentSetting(gpu.AdapterIndex, out ADLOD8CurrentSetting odCurrentSetting)) {
                        SetOD8Range(gpu.ADLOD8InitSetting, odCurrentSetting, gpu.AdapterIndex, ADLOD8SettingId.OD8_POWER_PERCENTAGE, value == 0, value - 100);
                    }
                }
                return true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return false;
            }
        }

        public bool SetTempLimit(int gpuIndex, int value) {
            if (!TryGetAtiGpu(gpuIndex, out ATIGPU gpu)) {
                return false;
            }
            try {
                bool isAutoModel = value == 0;
                if (gpu.OverdriveVersion < 8) {
                    ADLODNPowerLimitSetting info = new ADLODNPowerLimitSetting();
                    var r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get(_context, gpu.AdapterIndex, out info);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get)} {r.ToString()}");
                        return false;
                    }
                    info.iMode = isAutoModel ? AdlConst.ODNControlType_Auto : AdlConst.ODNControlType_Manual;
                    info.iMaxOperatingTemperature = isAutoModel ? 0 : value;
                    r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Set(_context, gpu.AdapterIndex, ref info);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Set)} {r.ToString()}");
                        return false;
                    }
                }
                else {
                    if (GetOD8CurrentSetting(gpu.AdapterIndex, out ADLOD8CurrentSetting odCurrentSetting)) {
                        SetOD8Range(gpu.ADLOD8InitSetting, odCurrentSetting, gpu.AdapterIndex, ADLOD8SettingId.OD8_OPERATING_TEMP_MAX, isAutoModel, value);
                    }
                }
                return true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return false;
            }
        }

        private uint GetFanSpeed(int gpuIndex) {
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

        private uint GetPowerUsage(int gpuIndex) {
            if (!TryGetAtiGpu(gpuIndex, out ATIGPU gpu)) {
                return 0;
            }
            try {
                int power = 0;
                if (gpu.OverdriveVersion >= 6) {
                    var r = AdlNativeMethods.ADL2_Overdrive6_CurrentPower_Get(_context, gpu.AdapterIndex, ADLODNCurrentPowerType.TOTAL_POWER, out power);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_Overdrive6_CurrentPower_Get)} {r.ToString()}");
                        return 0;
                    }
                    return (uint)(power / 256.0);
                }
            }
            catch {
            }
            return 0;
        }

        private int GetTemperature(int gpuIndex) {
            try {
                if (!TryGetAtiGpu(gpuIndex, out ATIGPU gpu)) {
                    return 0;
                }
                if (gpu.OverdriveVersion >= 7) {
                    var r = AdlNativeMethods.ADL2_OverdriveN_Temperature_Get(_context, gpu.AdapterIndex, ADLODNTemperatureType.CORE, out int temperature);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_Temperature_Get)} {r.ToString()}");
                        return 0;
                    }
                    return (int)(0.001f * temperature);
                }
                else {
                    ADLTemperature info = new ADLTemperature();
                    var r = AdlNativeMethods.ADL_Overdrive5_Temperature_Get(gpu.AdapterIndex, 0, ref info);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL_Overdrive5_Temperature_Get)} {r.ToString()}");
                        return 0;
                    }
                    return (int)(0.001f * info.Temperature);
                }
            }
            catch {
            }
            return 0;
        }
    }
}
